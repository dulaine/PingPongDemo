using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGlobalDefine
{
    public const string UINameSPace = "PingpongUI"; //version44.2
    public const string UIPathPrefix = "Assets/Resources/";
    public const string PrefabSuffix = ".prefab";
    public const string UIPrefabPath = "UI/UIPrefab/"; //version44 
    public const string UIWindowsIDDefineScriptPath = "Scripts/UIFrameWork/UI/Define/WindowsIDDefine";//version44 
    public const string ViewScriptSuffix = "View";
    public const string ControlScriptSuffix = "Control";
    public const string ProxyViewScriptSuffix = "UIViewProxy";
}

public enum UIWindowType
{
    Normal, 
    Fixed, // 固定窗口(会一直存在的类型 UITopBar, 比较特殊) 通常是会和其他UI一起存在的UI类型.
    PopUp, // 模态窗口(UIMessageBox, PopWindow , TipsWindow ......)
    DontDestory,//切换场景不清理的, 比如UIdownloading
}


// ShowMode control the window show mode
// NavigationMode control the navigation system
public enum UIWindowShowMode
{
    DoNothing = 0,
    HideOtherWindow,    // 打开界面关闭其他界面
    DestoryOtherWindow,
}

public enum UIWindowNavigationMode
{
    IgnoreNavigation = 0,// 打开界面 不加入导航队列
    NormalNavigation,   // 打开界面 加入导航队列
}

// Background Texture
public enum UIWindowColliderMode
{
    None, // No BgTexture and No Collider
    Normal, // Collider with alpha 0.001 BgTexture
    WithBg, // Collider with alpha 1 BgTexture
}

