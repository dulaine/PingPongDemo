using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationData
{
    public UIControllerBase showingUI;            //当前要显示的UI

    public List<WindowID> beingHidedUI;     //当前被遮挡,需要隐藏的UI, 下次返回时候恢复
}