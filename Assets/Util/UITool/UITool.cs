using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using PureMVC.Patterns;

//using LuaInterface;

/// <summary>
/// UI工具，加载UI，UI分层，UI遮罩
/// 
/// UI不用走池管理
/// </summary>
public class UITool : MonoBehaviour
{
    /// <summary>
    /// 一个加载完毕的UI的数据
    /// </summary>
    class UIToolData
    {
        public uint Id;
        public Canvas MyCanvas;             //UI界面根
        public UIConfigTable ConfigTable;
        public bool IsShowed = false;       //此UI是否显示中
        public UIToolData BgBefore;     //链表，上一个显示的遮罩UI
        public UIToolData BgLater;      //链表，下一个显示的遮罩UI
        public UIToolData(uint id, Transform obj, UIConfigTable configTable)
        {
            Id = id;
            MyCanvas = obj.GetComponent<Canvas>();
            ConfigTable = configTable;
        }
        /// <summary>
        /// 自己从链表中出去
        /// </summary>
        public void LinkedListBreak()
        {
            if (BgLater != null)
            {
                BgLater.BgBefore = BgBefore;
            }
            if (BgBefore != null)
            {
                BgBefore.BgLater = BgLater;
            }
            BgLater = null;
            BgBefore = null;
        }
        /// <summary>
        /// 添加一个自己的前置数据
        /// </summary>
        public void LinkedListAddBefor(UIToolData _data)
        {
            BgBefore = _data;
            if (_data != null)
            {
                if (_data.BgLater != null)
                {
                    _data.BgLater.BgBefore = this;
                }
                BgLater = _data.BgLater;
                _data.BgLater = this;
            }
        }
    }

    //单例
    public static UITool Instance;
    //UI所需
    public Camera UICamera;     //UICamera
    public Canvas Background;   //背景，将此背景移动到需要的canvas下的第一个
    public float BackgroundDis = 0.1f;  //背景在此UI后面多少（不同canvas）
    //UI父物体
    public Transform HUDTrans;          //HUD UI父物体
    public Transform JoystickTrans;     //摇杆UI
    public Transform NormalTrans;       //窗口UI
    public Transform FixationTrans;     //固定UI父物体
    public Transform PopUpTrans;        //普通弹出UI

    internal void HideCSUI(uint uIBackpackInterface, object onBackpackHide)
    {
        throw new NotImplementedException();
    }

    public Transform GuideTrans;        //引导UI
    public Transform LoadingTrans;      //加载UI
    public Transform PopTopTrans;       //最高弹出UI
    //UI深度位置
    int UIMaxDepth = 1000;      //设置UI最大深度
    public float HUDPos = 999;          //HUD UI深度
    public float JoystickPos = 990;     //摇杆层，层高
    public float NormalPos = 980;       //窗口UI深度
    public float FixationPos = 600;     //固定UI深度
    public float PopUpPos = 500;        //普通弹出UI深度
    public float GuidePos = 400;        //引导层高
    public float LoadingPos = 300;      //加载UI深度
    public float PopTopPos = 200;       //最高弹出UI深度

    ConfigTableBase<UIConfigTable> _UIConfig = null;    //UI配置表
    Dictionary<uint, UIToolData> _AllUI = new Dictionary<uint, UIToolData>();   //所有加载完毕的UI
    Dictionary<uint, Action<GameObject>> _Callback = new Dictionary<uint, Action<GameObject>>();    //UI加载所需要的回调
    UIToolData mBgUI = null;        //当前背景属于哪个UI

    void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(this);
    }

    //-----------------公共方法------------------
    #region 公共方法
    public void SetUIConfig(ConfigTableBase<UIConfigTable> uiConfig)
    {
        _UIConfig = uiConfig;
    }
    /// <summary>
    /// CS脚本显示UI
    /// </summary>
    public void ShowCSUI(uint id, Action<GameObject> callback)
    {
        if (callback != null)
        {
            _Callback[id] = callback;
        }
        //看此UI是否已加载
        if (_AllUI.ContainsKey(id))
        {
            LoggerHelper.Debug("一个UI显示出来了：    " + _AllUI[id].ConfigTable.Name);
            //已加载，开始显示
            MyShowUI(id);
            var _obj = _AllUI[id].MyCanvas.gameObject;
            //显示完成后，回调
            Action<GameObject> _callback;
            _Callback.TryGetValue(id, out _callback);
            if (_callback != null)
            {
                _Callback.Remove(id);
                _callback.Invoke(_obj);
            }
            //发送MVC消息
            Facade.Instance.SendNotification(string.Concat(_obj.name, MVCNameConst.UI_UIShow));
        }
        else
        {
            //未加载，开始加载
            if (_UIConfig != null)
            {
                UIConfigTable _table = _UIConfig.GetData(id);
                if (_table != null)
                {
                    PoolTool.GetGameObject(_table.Name, _table.AssetName, LoadUIBack, _table, EPoolAddType.No);
                }
                else
                {
                    Debug.Log("在UIConfigTable中，不存在此ID:" + id);
                }
            }
            else
            {
                Debug.Log("UI配置表未加载！");
            }
        }
    }
    /// <summary>
    /// C#调用
    /// 关闭UI。并传入是否播放关闭动画（优先级高于配置）
    /// </summary>
    public void HideCSUI(uint id, bool isAnim = false, Action<GameObject> callback = null)
    {
        if (_AllUI.ContainsKey(id))
        {
            LoggerHelper.Debug("一个UI隐藏了：    " + _AllUI[id].ConfigTable.Name);
            UIToolData _data = _AllUI[id];
            MyHideUI(_data);        //隐藏UI方法
            //发送MVC消息
            Facade.Instance.SendNotification(string.Concat(_data.ConfigTable.Name,MVCNameConst.UI_UIHide));
            if (callback != null)
            {
                callback.Invoke(_data.MyCanvas.gameObject);
            }
        }
    }

    ///// <summary>
    ///// Lua调用
    ///// 关闭UI。并传入是否播放关闭动画（优先级高于配置）
    ///// </summary>
    //public void HideLuaUI(uint id, bool isAnim, LuaFunction fun)
    //{

    //}
    ///// <summary>
    ///// Lua
    ///// 执行一个界面的UI动画()
    ///// </summary>
    //public void AnimPlay(uint id, LuaFunction fun)
    //{
    //    if (fun != null)
    //    {
    //        fun.Call();
    //    }
    //}

    /// <summary>
    /// 场景切换时调用
    /// 用来隐藏不在此场景展示的UI
    /// </summary>
    public void OnSceneChange(string nextSceneName)
    {
        LoggerHelper.Debug(" 场景切换了，开始关闭部分UI :" + nextSceneName);
        var _list = _AllUI.Values.Where(p => p.IsShowed && (!p.ConfigTable.IsAllScene && !p.ConfigTable.SceneList.Contains(nextSceneName))).ToList();
        for (int i = 0; i < _list.Count; i++)
        {
            //LoggerHelper.Debug(" 场景切换了，要关闭或销毁的UI是 :" + _list[i].ConfigTable.Name);
            var tData = _list[i];
            MyHideUI(tData);        //调用自身UI关闭方法
            //判断此关闭的UI，是否切场景销毁
            if (tData.ConfigTable.DestroyType == 1)
            {
                //PoolTool.RemoveGameObject(_data.MyCanvas.transform, true);     //销毁UI(不走池了)
                DestroyImmediate(tData.MyCanvas.gameObject);
                _AllUI.Remove(tData.Id);    //将彻底隐藏的UI进行销毁
            }
        }
        var tClose = _AllUI.Values.Where(p => p.IsShowed && p.ConfigTable.IsChangeSceneClose).ToList();
        for (int i = 0; i < tClose.Count; i++)
        {
            MyHideUI(tClose[i]);        //调用自身UI关闭方法
        }
    }
    #endregion

    //-----------------内部方法------------------
    #region 内部方法
    /// <summary>
    /// UI加载成功后，会直接调用对应UI的创建成功方法
    /// </summary>
    void LoadUIBack(PoolLoadData _data)
    {
        //将加载完的UI，放在临时存储中
        UIConfigTable tData = _data.data as UIConfigTable;
        UIToolData tToolData = new UIToolData(tData.Id, _data.transform, tData);
        tToolData.MyCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        tToolData.MyCanvas.worldCamera = UICamera;
        //设置父物体
        _AllUI.Add(tToolData.Id, tToolData);
        SetUIParent(tToolData.Id);
        //显示UI
        MyShowUI(tToolData.Id);
        //创建成功后，将obj传过去
        if (_Callback.ContainsKey(tData.Id))
        {
            _Callback[tData.Id].Invoke(_data.transform.gameObject);
            _Callback.Remove(tData.Id);
        }
        //发送MVC消息
        Facade.Instance.SendNotification(string.Concat(_data.transform.gameObject.name, MVCNameConst.UI_UIShow));
    }
    /// <summary>
    /// 显示UI
    /// </summary>
    void MyShowUI(uint id)
    {
        SetUILocation(id);      //设置自己所在层的位置
        //播放此UI打开动画


        UIToolData _data = _AllUI[id];
        if (_data.IsShowed)
        {
            return;
        }
        _data.IsShowed = true;
        _data.MyCanvas.gameObject.SetActive(true);
        if (_data.ConfigTable.IsNeedBackground == 1)
        {
            _data.LinkedListAddBefor(mBgUI);    //新建背景时，需要赋值自己打开时，上个有背景的UI的值
            SetBg(_data);   //设置背景遮罩
        }
    }
    /// <summary>
    /// 隐藏UI
    /// </summary>
    void MyHideUI(UIToolData _data)
    {
        if (!_data.IsShowed)
        {
            return;
        }
        _data.IsShowed = false;         //设置标记
        //播放此UI关闭动画

        //先隐藏UI
        if (_data.ConfigTable.IsHide == 1)
        {
            _data.MyCanvas.gameObject.SetActive(false);
        }
        //调整同层UI深度
        ResetUILocation(_data);
        //还原上个UI的背景
        if (_data.ConfigTable.IsNeedBackground == 1)
        {
            //需要所有UI的深度调整完毕后，背景才能进行还原
            SetBackground(_data);
            //还原完毕后当前UI的前UI背景ID清空
            _data.LinkedListBreak();
        }
        //判断此UI是否关闭销毁
        if (_data.ConfigTable.DestroyType == 2)
        {
            //PoolTool.RemoveGameObject(_data.MyCanvas.transform, true);     //销毁UI(不走池了)
            DestroyImmediate(_data.MyCanvas.gameObject);
            _AllUI.Remove(_data.Id);    //将彻底隐藏的UI进行销毁
        }
    }

    /// <summary>
    /// 设置UI父物体
    /// </summary>
    void SetUIParent(uint id)
    {
        if (_AllUI.ContainsKey(id))
        {
            UIToolData _data = _AllUI[id];
            Transform _trans = _data.MyCanvas.transform;
            switch (_data.ConfigTable.Hierarchy)
            {
                case "HUD":
                    _trans.SetParent(HUDTrans, false);
                    break;
                case "Joystick":
                    //_trans.parent = JoystickTrans;
                    _trans.SetParent(JoystickTrans, false);
                    break;
                case "Normal":
                    //_trans.parent = NormalTrans;
                    _trans.SetParent(NormalTrans, false);
                    break;
                case "Fixation":
                    //_trans.parent = FixationTrans;
                    _trans.SetParent(FixationTrans, false);
                    break;
                case "PopUp":
                    //_trans.parent = PopUpTrans;
                    _trans.SetParent(PopUpTrans, false);
                    break;
                case "Guide":
                    //_trans.parent = GuideTrans;
                    _trans.SetParent(GuideTrans, false);
                    break;
                case "Loading":
                    //_trans.parent = LoadingTrans;
                    _trans.SetParent(LoadingTrans, false);
                    break;
                case "PopTop":
                    //_trans.parent = PopTopTrans;
                    _trans.SetParent(PopTopTrans, false);
                    break;
                default:
                    break;
            }
            _trans.localScale = Vector3.one;
            _trans.localPosition = Vector3.zero;
        }
    }
    /// <summary>
    /// UI打开时，设置自己在同层UI位置
    /// </summary>
    void SetUILocation(uint id)
    {
        if (_AllUI.ContainsKey(id))
        {
            UIToolData _data = _AllUI[id];
            UIToolData[] _items = _AllUI.Values.Where(p => p.IsShowed && p.ConfigTable.Hierarchy == _data.ConfigTable.Hierarchy).ToArray();
            float _minDis = 0;
            float _depthSpeed = 0;
            int _count = _items.Length;
            if (_count != 0)    //此层有UI
            {
                for (int i = 0; i < _count; i++)
                {
                    float _nowDis = _items[i].MyCanvas.planeDistance;
                    if (_minDis == 0 || _nowDis < _minDis)
                    {
                        _minDis = _nowDis;
                        _depthSpeed = _items[i].ConfigTable.DepthSpeed;
                    }
                }
            }
            else    //此层无显示的UI，直接设置为基础层
            {
                switch (_data.ConfigTable.Hierarchy)
                {
                    case "HUD":
                        _minDis = HUDPos;
                        break;
                    case "Joystick":
                        _minDis = JoystickPos;
                        break;
                    case "Fixation":
                        _minDis = FixationPos;
                        break;
                    case "Normal":
                        _minDis = NormalPos;
                        break;
                    case "PopUp":
                        _minDis = PopUpPos;
                        break;
                    case "Guide":
                        _minDis = GuidePos;
                        break;
                    case "Loading":
                        _minDis = LoadingPos;
                        break;
                    case "PopTop":
                        _minDis = PopTopPos;
                        break;
                    default:
                        break;
                }
            }
            _data.MyCanvas.planeDistance = _minDis - _depthSpeed;       //将其设置在最高层
            _data.MyCanvas.sortingOrder = UIMaxDepth - (int)_data.MyCanvas.planeDistance;   //设置orderInLayer实属无奈，错开order是为了防止同order的ScreenSpace-Camera类型canvas点击穿透
        }
    }
    /// <summary>
    /// UI关闭时，重置同层UI的位置
    /// </summary>
    void ResetUILocation(UIToolData data)
    {
        float _baseDis = data.MyCanvas.planeDistance;       //获取当前深度
        float _depthSpeed = data.ConfigTable.DepthSpeed;    //获取此UI占用深度
        //将此层所有显示的UI进行深度排序
        UIToolData[] _items = _AllUI.Values.Where(p => p.IsShowed && p.ConfigTable.Hierarchy == data.ConfigTable.Hierarchy).ToArray();
        int _count = _items.Length;
        //将所有同层的UI的深度，进行调整
        for (int i = 0; i < _count; i++)
        {
            float _nowDis = _items[i].MyCanvas.planeDistance;
            if (_nowDis < _baseDis)
            {
                _items[i].MyCanvas.planeDistance = _nowDis + _depthSpeed;
            }
        }
    }
    /// <summary>
    /// 设置UI遮罩背景
    /// </summary>
    void SetBackground(UIToolData _data)
    {
        if (_data.BgBefore != null)
        {
            //如果当前id对应的UI尚未关闭，则设置到对应的UI下，否则直接隐藏
            if (_data.BgBefore.MyCanvas.gameObject != null && _data.BgBefore.MyCanvas.gameObject.activeSelf)
            {
                SetBg(_data.BgBefore);
            }
            else
            {
                Debug.LogError("出现了一个隐藏的界面，但链表中没有去掉");
            }
        }
        else
        {
            //隐藏背景
            Background.gameObject.SetActive(false);
            mBgUI = null;
        }
    }
    /// <summary>
    /// 设置UI遮罩背景
    /// </summary>
    void SetBg(UIToolData data)
    {
        mBgUI = data;
        Background.planeDistance = data.MyCanvas.planeDistance + BackgroundDis;    //设置遮罩层位置
        Background.sortingOrder = UIMaxDepth - (int)Background.planeDistance;
        Background.gameObject.SetActive(true);
    }
    #endregion
}
