using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WindowConfigData
{
    public bool forceClearNavigation = false;                                                  //只有主UI才会设置true,一打开UI以前的导航信息都清空.
    public UIWindowType windowType = UIWindowType.Normal;                                   
    public UIWindowShowMode showMode = UIWindowShowMode.DoNothing;                          //显示是否会关闭其他UI
    public UIWindowColliderMode colliderMode = UIWindowColliderMode.None;                   //默认背景UI
    public UIWindowNavigationMode navigationMode = UIWindowNavigationMode.IgnoreNavigation; //是否保存导航信息
    public int depth = 0;
    public WindowID PreWindowId = WindowID.Count;               //默认的前置UI, 点击返回的UI
}
