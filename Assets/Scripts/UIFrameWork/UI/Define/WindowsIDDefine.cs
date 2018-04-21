using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WindowID
{
    WindowID_Invaild = 0,
    MainUI = 1,
    Count,
}

public class UIResourceDefine
{
    // Define the UIWindow prefab paths
    // all window prefab placed in Resources folder
    // or assetbundle path
    public static Dictionary<int, string> windowPrefabPath = new Dictionary<int, string>()
    {
        {(int) WindowID.MainUI, "MainUI"},
        {(int) WindowID.Count, ""},
    };

    // Main folder
    public static string UIPrefabPath = UIGlobalDefine.UIPrefabPath;
}
