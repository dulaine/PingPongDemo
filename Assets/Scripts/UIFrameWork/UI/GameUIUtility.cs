using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GameUIUtility  {


    public static UIViewBase InstantiateUI(WindowID id)
    {
        string prefabPath = UIResourceDefine.UIPrefabPath + UIResourceDefine.windowPrefabPath[(int) id];
        GameObject prefab = Resources.Load<GameObject>(prefabPath);// ResourceComponent.Instance.LoadAssetSyn(UIGlobalDefine.UIPathPrefix + prefabPath + UIGlobalDefine.PrefabSuffix) as GameObject;// version79.2  Resources.Load<GameObject>(prefabPath);
        if (prefab != null)
        {
            return PreparePrefab(id, prefab);
        }
        
        Debug.LogError("UI路径不存在:" + prefabPath );
        return null;
    }

    //version79.3 
    private static UIViewBase PreparePrefab(WindowID id, object asset)
    {
        //加载成功
        GameObject prefab = asset as GameObject;
        if (prefab != null)
        {
            GameObject uiObject = GameObject.Instantiate(prefab);
            uiObject.SetActive(true);

            //获取UIViewBase脚本或者添加
            UIViewBase uiView = uiObject.GetComponent<UIViewBase>();
            if (uiView == null)
            {
                WindowID UIID = (WindowID)Enum.Parse(typeof(WindowID), id.ToString());
                string UIViewScriptName = UIID.ToString() + UIGlobalDefine.ViewScriptSuffix;
                Debug.LogWarning("Prefab没有添加脚本!!!!" + UIViewScriptName);

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    Type t = assembly.GetType(string.Format("{0}.{1}", UIGlobalDefine.UINameSPace, UIViewScriptName)); //version44.2
                    if (t != null)
                    {
                        uiView = uiObject.AddComponent(t) as UIViewBase;
                        break;
                    }
                }
            }

            if (uiView.ID != id)
            {
                Debug.LogError(string.Format("<color=cyan>[UIViewBase.ID :{0} != shownWindowId :{1}]</color>",uiView.ID,
                    id));
                return null;
            }

            uiView.Controller.Asset = prefab;//version79.4
            return uiView;
        }

        return null;
    }

    //获取Canvas根
    public static Transform GetUIRoot()
    {
        GameObject UINormalWindowRoot = GameObject.Find("RootCanvas");
        if (UINormalWindowRoot == null)
        {
            //UI摄像机
            Camera uiCamera = new GameObject("UICamera").AddComponent<Camera>();
            uiCamera.cullingMask = (1 << LayerMask.NameToLayer("UI"));

            //Canvas
            UINormalWindowRoot = new GameObject("RootCanvas");
            Canvas canvas = UINormalWindowRoot.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = uiCamera;

            //Scaler
            CanvasScaler scaler = UINormalWindowRoot.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280f, 720f);

            //GraphicRayCaster
            UINormalWindowRoot.AddComponent<GraphicRaycaster>();
        }

        return UINormalWindowRoot.transform;
    }

    /// <summary>
    /// 添加子节点
    /// Add child to target
    /// </summary>
    public static void AddChildToTarget(Transform target, Transform child)
    {
        //child.parent = target;
        child.SetParent(target, false);
        // version47  
        child.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 0.5f);
        child.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 0.5f);
        // version47  
        child.localScale = Vector3.one;
        child.localPosition = Vector3.zero;
        child.localEulerAngles = Vector3.zero;


        // ChangeChildLayer(child, target.gameObject.layer);
    }

    // <summary>
    /// 给目标添加Collider背景
    /// </summary>
    public static GameObject AddColliderBgToTarget(GameObject target,  bool isTransparent = true, string spriteName = "")
    {
        Transform windowBg = (new GameObject("WindowBg")).transform;
        windowBg.SetParent(target.transform);
        windowBg.localPosition = Vector3.zero;
        windowBg.localScale = Vector3.one;
        windowBg.eulerAngles = Vector3.zero;
        windowBg.SetAsFirstSibling();

        Image bgImage = windowBg.gameObject.AddComponent<Image>();
        bgImage.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

        if (isTransparent)
        {
            Color color = bgImage.color;
            color.a = 1f/255f;
            bgImage.color = color;
        }
        else
        {
            if (!string.IsNullOrEmpty(spriteName))
            {
                //加载对应图片
                //bgImage.sprite = 
            }
        }


        return null;
    }


    public static List<T> GetComponentCache<T>(GameObject gameObject) where T : Component
    {
        return new List<T>(gameObject.GetComponentsInChildren<T>(true));
    }

    public static T GetComponentFromCache<T>(ref List<T> cache, string name) where T : Component
    {
        T comp = null;

        for (int i = 0; i < cache.Count;)
        {
            if (cache[i].name == name)
            {
                comp = cache[i];
                cache.RemoveAt(i);
                break;
            }
            else if (cache[i].name == "UI")
            {
                cache.RemoveAt(i);
                continue;
            }
            ++i;
        }
        return comp;
    }


    #region UIAnimation

    #region Alpha
    ////设置透明变化 : IMage RawImage, Text
    public static Tweener DoAlpha<T>(T UI, float from, float to, float duration) where T: MaskableGraphic
    {
        //设置初始值
        Color color = UI.color; color.a = from; UI.color = color;
        return DOTween.To(() => UI.color.a, x => { color = UI.color; color.a = x; UI.color = color; }, to, duration);
    }

    //设置透明变化
    public static Tweener DoAlpha(CanvasGroup UI, float from, float to, float duration)
    {
        UI.alpha = from;
        return DOTween.To(() => UI.alpha, x => UI.alpha = x, to, duration);
    }

    //设置透明变化
    public static Tweener DoAlpha(Button UI, float from, float to, float duration)
    {
        //获取组件
        Image img = UI.transform.GetComponent<Image>();
        if (img == null)
        {
            Debug.LogError(UI.name + ": can not find Image On Button");
            return null;
        }
        Text txt = UI.transform.GetChild(0).GetComponent<Text>();
        
        //设置Image和Text的初始值
        Color ImageColor = img.color; ImageColor.a = from; img.color = ImageColor;
        if (txt != null) { Color TextColor = txt.color; TextColor.a = from; txt.color = TextColor; }

        return DOTween.To(() => img.color.a,
            x =>
            {
                if (img != null)
                {
                    ImageColor = img.color;
                    ImageColor.a = x;
                    img.color = ImageColor;
                }

                if (txt != null)
                {
                    Color TextColor = txt.color;
                    TextColor.a = x;
                    txt.color = TextColor;
                }
            },
            to, duration);
    }
    #endregion

    #region Position

        public static Tweener DoMove(Transform UI, Vector3 from, Vector3 to, float duration)
        {
            //设置初始值
            UI.localPosition = from;
            return DOTween.To(() => UI.localPosition, x => { UI.localPosition = x; }, to, duration);
        }

        //垂直方向的移动
        public static Tweener MoveY(Transform UI, float from, float to, float duration)
        {
            Vector3 pos = UI.localPosition;
            pos.y = from;
            UI.localPosition = pos;

            return UI.DOLocalMoveY(to, duration);
        }

        //水平方向的移动
        public static Tweener MoveX(Transform UI, float from, float to, float duration, int loops = 1)
        {
            //初始化位置
            Vector3 pos = UI.localPosition;
            pos.x = from;
            UI.localPosition = pos;

            return UI.DOLocalMoveX(to, duration).SetEase(Ease.Linear).SetLoops(loops);
        }
    #endregion

    #region Scale

    public static Tweener DoScale(Transform UI, Vector3 from, Vector3 to, float duration)
        {
            //设置初始值
            UI.localScale = from;
            return DOTween.To(() => UI.localScale, x => {  UI.localScale = x; }, to, duration);
        }

        public static Tweener DoScale(Transform UI, float from, float to, float duration)
        {
            //设置初始值
            Vector3 tmp = UI.localScale;
            tmp.x = from;
            tmp.y = from;
            tmp.z = from;
            UI.localScale = tmp;

            return DOTween.To(() => UI.localScale.x, x => { tmp = UI.localScale; tmp.x = x; tmp.y = x; tmp.z = x; UI.localScale = tmp; }, to, duration);
        }
    #endregion

    #region Rotation
    // 0 - 360:逆时针旋转; 0 - -360 顺序时针旋转
    public static Tweener DoRotation(Transform UI, Vector3 from, Vector3 to, float duration)
    {
        //设置初始值
        UI.localEulerAngles = from;
        return DOTween.To(() => UI.localEulerAngles, x => { UI.localEulerAngles = x; }, to, duration);
    }
    #endregion

    private static ulong sID = 0;
    public static ulong GetUniquePlayID()
    {
        return ++sID;
    }
    #endregion
}
