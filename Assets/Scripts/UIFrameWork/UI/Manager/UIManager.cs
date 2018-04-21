using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UIManager {
    protected List<int> registerdWindowIds = new List<int>(); // 注册的UI

    protected Dictionary<int, UIControllerBase> dicWindowsCache;  //所有使用过的UI
    protected Dictionary<int, UIControllerBase> dicShownWindows;//正被打开的UI

    protected Stack<NavigationData> backSequence;
    protected UIControllerBase curNavigationWindow = null; // 当前显示的可导航UI
    protected UIControllerBase lastNavigationWindow = null; // 上次显示的导航UI

    public Transform UINormalWindowRoot; //根节点

    private static UIManager _instance;

    public static UIManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new UIManager();
                _instance.Init();
            }
            return _instance;
        }
    }

    public void Init()
    {
        if (dicWindowsCache == null)
            dicWindowsCache = new Dictionary<int, UIControllerBase>();

        if (dicShownWindows == null)
            dicShownWindows = new Dictionary<int, UIControllerBase>();

        if (backSequence == null)
            backSequence = new Stack<NavigationData>();

        //UINormalWindowRoot = GameObject.Find("RootCanvas").transform;
        //GameObject.DontDestroyOnLoad(UINormalWindowRoot);
    }

    public void Update()
    {
        UpdateShowUI();
    }

    private void UpdateShowUI()
    {
        if (dicShownWindows.Count == 0) return;

        Dictionary<int, UIControllerBase>.Enumerator iter = dicShownWindows.GetEnumerator();
        try
        {
            while (iter.MoveNext())
            {
                iter.Current.Value.UpdateUI();
            }
        }
        finally
        {
            iter.Dispose();
        }

    }
    #region 显示UI

    public void ShowWindow(WindowID id, WindowContextDataBase showContextData = null)
    {
        UIControllerBase baseWindow = PrepareUI(id, showContextData);
        if (baseWindow != null)
        {
            Show(baseWindow, id, showContextData);
        }
    }
    
    private UIControllerBase PrepareUI(WindowID id, WindowContextDataBase showContextData = null)
    {
        if (!this.IsWindowRegistered(id))
            return null;

        if (dicShownWindows.ContainsKey((int)id))
            return null;

        UIControllerBase baseController = GetGameWindowFromCache(id);

        //UI没有在场景里打开过, Instantiate New One
        bool newAdded = false;
        if (baseController == null)
        {
            newAdded = true;
            if (UIResourceDefine.windowPrefabPath.ContainsKey((int)id))
            {

                //实例化UI
                UIViewBase uiView = GameUIUtility.InstantiateUI(id);

                //保存Controller
                baseController = uiView.Controller;
                dicWindowsCache[(int)id] = baseController;

                //初始化
                baseController.OnInit();

                //设置UI根
                Transform targetRoot = GameUIUtility.GetUIRoot();//UINormalWindowRoot;
                GameUIUtility.AddChildToTarget(targetRoot, uiView.gameObject.transform);

                //设置UI Camera
                Canvas uiCanvas = uiView.gameObject.GetComponent<Canvas>();
                uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
                {
                    GameObject uiCamera = GameObject.Find("UICamera");
                    if(uiCamera != null)uiCanvas.worldCamera = uiCamera.GetComponent<Camera>();
                }

                //添加通用背景
                AddColliderBgForWindow(uiView);
            }
            else
            {
               Debug.LogError("UI ID对应的Prefab位置在找不到, 使用工具重新生成 "  + id);
               return null;
            }
        }

        if (baseController == null)
            Debug.LogError("[UI instance is null.]" + id.ToString());

        //设置导航信息, 如果是导航类型的UI会把当前显示的UI关闭并且保存到BackSequenceStack里面
        //下次返回的时候, 从Stack里面回复.
        if (showContextData == null || (showContextData != null && showContextData.executeNavLogic))
        {
            ExecuteNavigationLogic(baseController);
        }
        //层级管理
        //AdjustBaseWindowDepth(baseController);

        return baseController;
    }

    private void Show(UIControllerBase baseWindow, WindowID id, WindowContextDataBase contextData = null)
    {
        baseWindow.ShowWindow(contextData);
        dicShownWindows[(int)id] = baseWindow;
        if (baseWindow.UIConfigData.navigationMode == UIWindowNavigationMode.NormalNavigation)
        {
            lastNavigationWindow = curNavigationWindow;
            curNavigationWindow = baseWindow;
            Debug.Log("<color=magenta>### current Navigation window </color>" + baseWindow.ID.ToString());
        }
    }

    /// <summary>
    /// 添加通用的背景UI
    /// </summary>
    private void AddColliderBgForWindow(UIViewBase baseWindow)
    {
        UIWindowColliderMode colliderMode = baseWindow.windowData.colliderMode;
        if (colliderMode == UIWindowColliderMode.None)
            return;

        GameObject bgObj = null;
        if (colliderMode == UIWindowColliderMode.Normal)
            bgObj = GameUIUtility.AddColliderBgToTarget(baseWindow.gameObject, true);
        if (colliderMode == UIWindowColliderMode.WithBg)
            bgObj = GameUIUtility.AddColliderBgToTarget(baseWindow.gameObject, false, "");

        baseWindow.Controller.OnAddColliderBg(bgObj);
    }

    #endregion

    #region 关闭
    /// <summary>
    /// Hide target window
    /// </summary>
    public virtual void HideWindow(WindowID id, Action onCompleted = null)
    {
        if (!IsWindowRegistered(id))
            return;

        if (!dicShownWindows.ContainsKey((int)id))
            return;

        if (dicShownWindows.ContainsKey((int)id))
        {
            //关闭后 从显示字典删除
            Action competedAction = delegate
            {
                dicShownWindows.Remove((int)id);
            };

            if (onCompleted != null)
            {
                competedAction += onCompleted;
            }

            dicShownWindows[(int)id].HideWindow(competedAction);
        }
    }


    // Cached list (avoid always new List<WindowID>)
    private List<WindowID> listCached = new List<WindowID>();
    //关闭所有显示的UI
    public void HideAllShownWindow(bool includeFixed = false)
    {
        listCached.Clear();
        foreach (KeyValuePair<int, UIControllerBase> ui in dicShownWindows)
        {
            if (ui.Value.UIConfigData.windowType == UIWindowType.Fixed && !includeFixed)
                continue;
            listCached.Add((WindowID)ui.Key);
            ui.Value.HideWindowDirectly();
        }

        if (listCached.Count > 0)
        {
            for (int i = 0; i < listCached.Count; i++)
                dicShownWindows.Remove((int)listCached[i]);
        }
    }


    #endregion

    #region 销毁
    /// <summary>
    /// Destroy all window
    /// </summary>
    public virtual void DestroyAllWindow()
    {
        if (dicWindowsCache != null)
        {
            foreach (KeyValuePair<int, UIControllerBase> ui in dicWindowsCache)
            {
                UIControllerBase baseWindow = ui.Value;
                baseWindow.DestroyWindow();
            }
            dicWindowsCache.Clear();
            dicShownWindows.Clear();
            backSequence.Clear();
        }
    }

    /// <summary>
    /// Destroy all window
    /// </summary>
    private List<int> destoryCacheList = new List<int>(); 
    public virtual void DestroyAllWindow(List<UIWindowType> exceptionList)
    {
        destoryCacheList.Clear();
        if (dicWindowsCache != null)
        {
            //选择需要清理的UI, 排除exceptionList里面的类型
            foreach (KeyValuePair<int, UIControllerBase> ui in dicWindowsCache)
            {
                UIControllerBase baseWindow = ui.Value;
                if (exceptionList != null && exceptionList.Contains(baseWindow.UIConfigData.windowType))
                {
                    continue;
                }
                destoryCacheList.Add((int)baseWindow.ID);
                baseWindow.DestroyWindow();
            }

            //清理UI
            if (destoryCacheList.Count > 0)
            {
                for (int i = 0; i < destoryCacheList.Count; i++)
                {
                    int windowsID = destoryCacheList[i];
                    dicWindowsCache.Remove(windowsID);
                    if (dicShownWindows.ContainsKey(windowsID))
                    {
                        dicShownWindows.Remove(windowsID);
                    }
                }
            }
            backSequence.Clear();
        }
    }
    #endregion

    #region UI导航
    #region 保存导航
    private void ExecuteNavigationLogic(UIControllerBase baseWindow)
    {
        WindowConfigData windowData = baseWindow.UIConfigData;

        //1. 是导航UI 就保存当前显示的UI后 保存他们信息
        if (baseWindow.IsNavigationTypeUI)
        {
            //保存当前UI信息
            PushCurrentUIInfoToStack(baseWindow);
        }
        else if (baseWindow.UIConfigData.showMode == UIWindowShowMode.HideOtherWindow)
        {//导航类型的UI打开也会关闭所有UI, 避免冲突. 
            HideAllShownWindow();
        }

        //2. 主UI就重置导航信息Stack, 主UI是所有导航的根
        if (baseWindow.UIConfigData.forceClearNavigation)
        {
            Debug.Log("<color=cyan>## [进入主UI, reset the backSequenceData for the navigation system.]##</color>");
            ClearBackSequence();
        }
        else
        {
            //3. 如果一个UI经过导航,到了关卡UI, 进入战斗后战斗UI不保存导航信息, 战斗失败可以选择回到关卡UI或者装备强化UI
            //选择关卡UI需要继续原来的导航, 选择进入强化UI就需要清除所有的导航信息.
            //if( condition from config, if need to check navigation)
            //CheckBackSequenceData(baseWindow);
        }
    }

    private void PushCurrentUIInfoToStack(UIControllerBase targetWindow)
    {
        WindowConfigData configData = targetWindow.UIConfigData;
        bool dealBackSequence = true;
        if (dicShownWindows.Count > 0 && dealBackSequence)
        {
            List<WindowID> removedKey = null;
            List<UIControllerBase> sortedHiddenWindows = new List<UIControllerBase>();//当前所有的UI, 除了Fiex类型的

            //如果当前打开的UI是关闭其他UI的类型 != UIWindowShowMode.DoNothing
            //保存当前显示的UI信息, UI类型是Fixed的除外,这种一直存在的UI不关闭
            NavigationData backData = new NavigationData();
            foreach (KeyValuePair<int, UIControllerBase> window in dicShownWindows)
            {
                if (configData.showMode != UIWindowShowMode.DoNothing)
                {
                    if (window.Value.UIConfigData.windowType == UIWindowType.Fixed)
                        continue;

                    if (removedKey == null)
                        removedKey = new List<WindowID>();
                    removedKey.Add((WindowID)window.Key);
                    window.Value.HideWindowDirectly();
                }

                if (window.Value.UIConfigData.windowType != UIWindowType.Fixed)
                    sortedHiddenWindows.Add(window.Value);
            }

            //如果当前是强关其他UI类型的, 关闭其他UI
            if (removedKey != null)
            {
                for (int i = 0; i < removedKey.Count; i++)
                    dicShownWindows.Remove((int)removedKey[i]);
            }

            // Push new navigation data
            if (configData.navigationMode == UIWindowNavigationMode.NormalNavigation)
            {
                // 按Depth从小到大排序后 保存
                sortedHiddenWindows.Sort(this.CompareUIDepthFun);
                List<WindowID> navHiddenWindows = new List<WindowID>();
                for (int i = 0; i < sortedHiddenWindows.Count; i++)
                {
                    WindowID pushWindowId = sortedHiddenWindows[i].ID;
                    navHiddenWindows.Add(pushWindowId);
                }
                backData.showingUI = targetWindow;
                backData.beingHidedUI = navHiddenWindows;
                backSequence.Push(backData);
            }
        }
    }

    private void ClearBackSequence()
    {
        if (backSequence != null)
            backSequence.Clear();
    }

    // 如果当前存在BackSequence数据
    // 1.栈顶界面不是当前要显示的界面需要清空BackSequence(导航被重置), 
    // [精炼失败/战斗UI 可以选择去向3个不同的UI, 不要求点击返回后回到依然精炼UI, 精炼失败UI不保存导航信息, ]
    // 2.栈顶界面是当前显示界面,如果类型为(NeedBack)则需要显示所有backShowTargets界面

    // 栈顶不是即将显示界面(导航序列被打断)
    // 如果当前导航队列顶部元素和当前显示的界面一致，表示和当前的导航数衔接上，后续导航直接使用导航数据
    // 不一致则表示，导航已经失效，下次点击返回按钮，我们直接根据window的preWindowId确定跳转到哪一个界面
    private void CheckBackSequenceData(UIControllerBase baseWindow)
    {
        if (baseWindow.IsNavigationTypeUI)
        {
            if (backSequence.Count > 0)
            {
                NavigationData backData = backSequence.Peek();
                if (backData.showingUI != null)
                {
                    if (backData.showingUI.ID != baseWindow.ID)
                    {
                        ClearBackSequence();
                    }
                }
                else
                    Debug.LogError("最近的导航信息中,showingUI is null!");
            }
        }
    }
    #endregion

    #region 执行导航
    protected bool PopNavigationWindow()
    {
        #region 导航信息为空
        if (backSequence.Count == 0)
        {
            if (curNavigationWindow == null)
                return false;

            //如果当前navigation的栈是空, 就检查当前UI的配置PrewindowID, 打开
            WindowID preWindowId = curNavigationWindow.UIConfigData.PreWindowId;
            if (preWindowId != WindowID.WindowID_Invaild)
            {
                Debug.LogWarning(string.Format(string.Format("## Current nav window {0} need show pre window {1}.", curNavigationWindow.ID.ToString(), preWindowId.ToString())));
                HideWindow(curNavigationWindow.ID, delegate
                {
                    //不执行导航
                    WindowContextDataBase showData = new WindowContextDataBase();
                    showData.executeNavLogic = false;
                    ShowWindow(preWindowId, showData);
                });
            }
            else
            {
                Debug.LogWarning("## CurrentShownWindow " + curNavigationWindow.ID + " preWindowId is " + WindowID.WindowID_Invaild);
            }
            return false;
        }
        #endregion

        NavigationData backData = backSequence.Peek();
        if (backData != null)
        {
            // check the current navigation data
            int curId = this.GetCurrentShownWindow();
            if (curId != (int)backData.showingUI.ID)
            {
                Debug.Log("<color=red>当前导航Stack里面信息不对 [backData.showingUI.ID != this.curShownWindowId]</color>");
                return false;
            }

            //关闭当前的UI, 恢复显示stack里面保存的UI
            WindowID showingId = backData.showingUI.ID;
            if (!dicShownWindows.ContainsKey((int)showingId))
                PopNavigationUI(backData);
            else
                HideWindow(showingId, delegate
                {
                    PopNavigationUI(backData);
                });
        }
        return true;
    }

    private void PopNavigationUI(NavigationData nd)
    {

        if (backSequence.Count > 0)
            backSequence.Pop();

        if (nd.beingHidedUI == null)
            return;

        //显示所有的保存的UI
        for (int i = 0; i < nd.beingHidedUI.Count; i++)
        {
            WindowID backId = nd.beingHidedUI[i];
            ShowWindowForNavigation(backId);
            if (i == nd.beingHidedUI.Count - 1)
            {
                UIControllerBase window = GetGameWindowFromCache(backId);
                if (window.UIConfigData.navigationMode == UIWindowNavigationMode.NormalNavigation)
                {
                    this.lastNavigationWindow = this.curNavigationWindow;
                    this.curNavigationWindow = window;
                }
            }
        }
    }

    //重新打开导航UI
    private void ShowWindowForNavigation(WindowID id)
    {
        if (!IsWindowRegistered(id))
              return;

        if (dicShownWindows.ContainsKey((int)id))
            return;

        UIControllerBase baseWindow = GetGameWindowFromCache(id);
        baseWindow.ShowWindow();
        dicShownWindows[(int)baseWindow.ID] = baseWindow;
    }

    #endregion
    #endregion

    #region Get

    public virtual UIControllerBase GetGameWindowFromCache(WindowID id)
    {
        if (!IsWindowRegistered(id))
            return null;

        if (dicWindowsCache.ContainsKey((int)id))
            return dicWindowsCache[(int)id];
        else
            return null;
    }

    //返回当前显示的最上层的UI
    protected virtual int GetCurrentShownWindow()
    {
        List<UIControllerBase> listWnds = this.dicShownWindows.Values.ToList();
        listWnds.Sort(this.CompareUIDepthFun);

        for (int i = listWnds.Count - 1; i >= 0; i--)
        {
            if (listWnds[i].UIConfigData.windowType != UIWindowType.Fixed)
                return (int)(listWnds[i].ID);
        }

        return (int)WindowID.WindowID_Invaild;
    }

    private int CompareUIDepthFun(UIControllerBase left, UIControllerBase right)
    {
        return left.UIConfigData.depth.CompareTo(right.UIConfigData.depth);
    }

    #endregion

    #region tools

    protected bool IsWindowRegistered(WindowID id)
    {
        return true;

        ////暂时不使用, 以后每个场景都配置自己的UI的时候,可以使用
        //bool result = registerdWindowIds.Contains((int)id);

        //if (!result)
        //    Debug.LogError("WindowID ID没有注册: " + id);

        //return result;
    }

    #endregion

    #region 场景切换清理
    public void ChangeScene(int sceneType)
    {
        //清理除了DontDestory类型外的UI
        DestroyAllWindow(new List<UIWindowType>() {UIWindowType.DontDestory});
    }
    #endregion
}
