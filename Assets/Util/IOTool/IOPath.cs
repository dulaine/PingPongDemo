using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 读写路径
/// </summary>
public class IOPath
{
    //当前是否是热更新状态
    public static bool IsHotUpdate = false;
    string _NowWWWPath      //当前平台文件的加载路径（普通io用）
    {
        get
        {
            if (!IsHotUpdate)
            {
                return

#if UNITY_EDITOR
                    "file://" + Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
                    Application.streamingAssetsPath + "/";
#else
                    "file://" + Application.streamingAssetsPath + "/";
#endif
            }
            else
            {
                return
                    "file://" + Application.persistentDataPath + "/";
            }
        }
    }
    string _NowPlatformPath      //当前平台加载路径（LoadFromFile用）
    {
        get
        {
            if (!IsHotUpdate)
            {
                return
#if UNITY_EDITOR
                    Application.streamingAssetsPath + "/";
#elif UNITY_ANDROID
                    Application.dataPath + "!assets/";
#elif UNITY_IOS
                    Application.dataPath + "/Raw/";
#else
                    Application.streamingAssetsPath + "/";
#endif
            }
            else
            {
                return
                    Application.persistentDataPath + "/";
            }
        }
    }
    //当前平台文件夹
    public string NowPlatformName =
#if UNITY_STANDALONE_WIN ||UNITY_EDITOR
    "Win";
#elif UNITY_ANDROID
    "Android";
#elif UNITY_IOS
    "iOS";
#endif
    [HideInInspector]
    public string NowPlatformPath;  //当前平台加载路径
    [HideInInspector]
    public string NowWWWPath;       //当前平台www加载路径
    [HideInInspector]
    public string NowPlatformRootPath;      //当前平台根路径
    [HideInInspector]
    public string NowWWWRootPath;           //当前平台WWW根路径

    static IOPath _Instance;
    public static IOPath Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new IOPath();
                _Instance.NowPlatformPath = _Instance._NowPlatformPath;
                _Instance.NowPlatformRootPath = _Instance.NowPlatformPath + _Instance.NowPlatformName + "/";
                _Instance.NowWWWPath = _Instance._NowWWWPath;
                _Instance.NowWWWRootPath = _Instance.NowWWWPath + _Instance.NowPlatformName + "/";
                Debug.Log("平台路径：" + _Instance.NowPlatformPath);
            }
            return _Instance;
        }
    }

}
