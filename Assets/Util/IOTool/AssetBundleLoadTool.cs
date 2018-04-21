using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// 此脚本仅负责包加载并返回，不负责下载
/// </summary>
public class AssetBundleLoadTool : MonoBehaviour
{
    public static AssetBundleLoadTool Instance;

    static object _Locker = new object();
    AssetBundleManifest _Manifest;      //根文件（用来保存引用）
    Dictionary<string, AssetData> LoadedAsset = new Dictionary<string, AssetData>();        //当前已加载的所有asset
    Dictionary<string, AssetData> LoadingAsset = new Dictionary<string, AssetData>();       //加载中的asset
    Dictionary<string, List<Action<UnityEngine.Object>>> LoadedCallback = new Dictionary<string, List<Action<UnityEngine.Object>>>();   //当前加载中的资源名称及回调
    Dictionary<string, AssetBundle> allAssets = new Dictionary<string, AssetBundle>();      //只在此处存储所有加载了的asset，统一引用、管理

    const string _LuaSameName = "lua/lua";      //要在所有asset中找出Lua的相同名称部分

    void Awake()
    {
        Instance = this;
        Init();
    }

    public void Init()
    {

    }

    //---------------------------外部调用------------------------------
    #region 外部调用
    /// <summary>
    /// gameobject的加载
    /// </summary>
    public void LoadObj<T>(string assetName, Action<UnityEngine.Object> callback) where T : UnityEngine.Object
    {
        LoadObj<T>(assetName, "", callback);
    }
    /// <summary>
    /// object加载
    /// </summary>
    public void LoadObj<T>(string assetName, string prefabName, Action<UnityEngine.Object> callback) where T : UnityEngine.Object
    {
        if (LoadedCallback.ContainsKey(assetName))
        {
            LoadedCallback[assetName].Add(callback);
        }
        else
        {
            LoadedCallback[assetName] = new List<Action<UnityEngine.Object>>() { callback };
            GetObj<T>(assetName, prefabName);
        }
    }
    /// <summary>
    /// 场景的加载及切换
    /// 热更场景加载及切换用，普通场景不用此方法
    /// </summary>
    public void LoadScene(string path, Action loadOverCallback)
    {
        StartCoroutine(DoLoadScene(IOPath.Instance.NowWWWRootPath + path, loadOverCallback));
    }

    /// <summary>
    /// 加载lua对应的asset
    /// </summary>
    public void LoadLua(string _path, Action<AssetBundle> _cb)
    {
        string abPath = IOPath.Instance.NowPlatformRootPath + _path;
        if (File.Exists(abPath))
        {
            if (_cb != null)
            {
                _cb(AssetBundle.LoadFromFile(abPath));
            }
        }
    }

    /// <summary>
    /// 用www方式读取StreamingAssets下的数据文件
    /// </summary>
    public void LoadFile(string path, Action<byte[]> callback)
    {
        StartCoroutine(DoLoadFile(path, callback));
    }
    /// <summary>
    /// 必要时候释放当前所有依赖项
    /// </summary>
    public void ReleaseDependAssets()
    {
        if (allAssets.Count > 0)
        {
            var _values = allAssets.Values.ToList();
            for (int i = 0; i < _values.Count; i++)
            {
                _values[i].Unload(false);
            }
            allAssets.Clear();
        }
    }
    #endregion

    //---------------------------内部调用------------------------------
    #region 内部调用
    /// <summary>
    /// 通过传入路径，加载此处对象并返回
    /// </summary>
    /// <typeparam name="T">类型，如gameobject，texture，audio等</typeparam>
    void GetObj<T>(string assetName, string prefabName) where T : UnityEngine.Object
    {
        lock (_Locker)
        {
            if (LoadedAsset.ContainsKey(assetName))
            {
                LoadOver<T>(assetName, prefabName);
            }
            else
            {
                LoadingAsset[assetName] = new AssetData();
                LoadingAsset[assetName].AssetName = assetName;
                LoadManifest();
                LoadDepend(assetName);
                LoadMain<T>(assetName, prefabName);
            }
        }
    }
    /// <summary>
    /// 调用加载完成后的回调
    /// </summary>
    void LoadOver<T>(string assetName, string prefabName) where T : UnityEngine.Object
    {
        var _list = LoadedCallback[assetName];
        string _tempName = "";      //prefab名称
        if (string.IsNullOrEmpty(prefabName))
        {
            int _i = assetName.LastIndexOf('.');
            if (_i < 0)
            {
                _tempName = assetName;
            }
            else
            {
                _tempName = assetName.Substring(0, _i);
            }
            _tempName = GetName(_tempName);
        }
        else
        {
            _tempName = prefabName;
        }
        var _obj = allAssets[assetName].LoadAsset<T>(_tempName);
        for (int i = 0; i < _list.Count; i++)
        {
            _list[i](_obj);
        }
        LoadedCallback.Clear();
    }
    /// <summary>
    /// 加载主文件
    /// </summary>
    void LoadManifest()
    {
        //看根文件释放加载完成
        if (_Manifest == null)
        {
            AssetBundle _mainAsset = LoadAsset(IOPath.Instance.NowPlatformName);
            _Manifest = (AssetBundleManifest)_mainAsset.LoadAsset("AssetBundleManifest");
        }
    }
    /// <summary>
    /// 加载次文件的依赖文件
    /// </summary>
    void LoadDepend(string assetName)
    {
        string[] _depends = _Manifest.GetAllDependencies(assetName);
        LoadingAsset[assetName].DependAssetName = _depends;
        for (int i = 0; i < _depends.Length; i++)
        {
            string _name = _depends[i];
            if (!allAssets.ContainsKey(_name))
            {
                LoadAsset(_name);       //加载所有依赖项
            }
        }
    }
    /// <summary>
    /// 加载主要文件
    /// </summary>
    void LoadMain<T>(string assetName, string prefabName) where T : UnityEngine.Object
    {
        LoadAsset(assetName);       //加载主文件
        LoadedAsset[assetName] = LoadingAsset[assetName];
        LoadingAsset.Remove(assetName);
        LoadOver<T>(assetName, prefabName);
    }
    /// <summary>
    /// 加载并切换场景
    /// </summary>
    IEnumerator DoLoadScene(string path, Action loadOverCallback)
    {
        WWW www = new WWW(path);
        yield return www;
        AssetBundle _asset = www.assetBundle;
        string _name = Path.GetFileName(path);
        _name = _name.Substring(0, _name.LastIndexOf('.'));
        var _scene = SceneManager.LoadSceneAsync(_name);
        yield return _scene;
        //场景加载完毕，销毁www内存
        if (_scene.isDone)
        {
            _asset.Unload(false);
            www.Dispose();
            if (loadOverCallback != null)
            {
                loadOverCallback();
            }
        }
    }
    /// <summary>
    /// 加载完毕的回调
    /// </summary>
    IEnumerator DoLoadFile(string path, Action<byte[]> callback)
    {
        WWW www = new WWW(path);
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            if (callback != null)
            {
                callback.Invoke(www.bytes);
            }
        }
        else
        {
            Debug.LogError("file文件加载报错了！ " + www.error + "        " + path);
        }
    }
    //------工具方法-------
    AssetBundle LoadAsset(string assetName)
    {
        if (allAssets.ContainsKey(assetName))
        {
            return allAssets[assetName];
        }
        else
        {
            var _asset = AssetBundle.LoadFromFile(IOPath.Instance.NowPlatformRootPath + assetName);
            if (_asset == null)
            {
                Debug.LogError("加载此包失败！ " + assetName);
            }
            allAssets.Add(assetName, _asset);
            return _asset;
        }
    }
    string GetName(string path)
    {
        if (path.Contains("/"))
        {
            int _tempI = path.LastIndexOf("/");
            return path.Substring(_tempI + 1);
        }
        else
            return path;
    }
    #endregion
}

/// <summary>
/// 加载的asset的数据
/// </summary>
public class AssetData
{
    public string AssetName;        //asset名称
    public string[] DependAssetName;    //引用到的asset
}
