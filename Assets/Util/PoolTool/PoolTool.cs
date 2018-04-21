using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

using PathologicalGames;

/// <summary>
/// 加载物体时用到的数据
/// </summary>
public class PoolLoadData
{
    public object data;
    public Transform transform;
    public PoolLoadData(object _data, Transform _transform)
    {
        data = _data;
        transform = _transform;
    }
}
public enum EPoolAddType
{
    Permanent,  //加入永久池
    Scene,      //加入场景池
    No,         //不加入池
}

/// <summary>
/// 池管理工具
/// </summary>
public class PoolTool
{
    class PoolToolData      //pool加载类
    {
        public string name;
        public string path;
        public object data;
        public EPoolAddType addType;    //是放入永久池还是场景池
        public Action<PoolLoadData> callback;
        public PoolToolData(string _name, string _path, object _data, EPoolAddType _addType, Action<PoolLoadData> _callback)
        {
            name = _name;
            path = _path;
            data = _data;
            callback = _callback;
            addType = _addType;
        }
    }
    static readonly string poolNamePermanent = "PermanentPool";
    static readonly string poolNameScene = "ScenePool";
    static SpawnPool permanentPool;     //永久池
    static SpawnPool scenePool;         //场景池

    static Queue<PoolToolData> _WaitLoad = new Queue<PoolToolData>();   //等待加载的物体

    static bool _IsInit = false;
    public static void Init()
    {
        if (!_IsInit)
        {
            _IsInit = true;
            var _obj = new GameObject();
            _obj.name = "PoolManager";
            UnityEngine.Object.DontDestroyOnLoad(_obj);
            CreatePermanentPool(_obj.transform);
        }
    }
    /// <summary>
    /// 创建不销毁的池
    /// </summary>
    public static void CreatePermanentPool(Transform _parent)
    {
        permanentPool = PoolManager.Pools.Create(poolNamePermanent);
        permanentPool.group.parent = _parent;
        permanentPool.group.localPosition = new Vector3(1.5f, 0, 0);
        permanentPool.group.localRotation = Quaternion.identity;
    }
    /// <summary>
    /// 创建动态池，每个场景都创建
    /// </summary>
    public static void CreateScenePool()
    {
        scenePool = PoolManager.Pools.Create(poolNameScene);
    }

    /// <summary>
    /// 通过assetbundle的路径，获取一个文件
    /// </summary>
    public static void GetGameObject(string _name, string _path, Action<PoolLoadData> _callback, object _data = null, EPoolAddType _addType = EPoolAddType.Scene)
    {
        Transform tTrans = null;
        if (_addType == EPoolAddType.Permanent)
        {
            if (permanentPool.prefabs.ContainsKey(_name))
            {
                tTrans = permanentPool.Spawn(_name);
            }
            else if (scenePool.prefabs.ContainsKey(_name))
            {
                //LoggerHelper.Debug("用 scenePool 池的物体在自己池中创建个新prefabPool 名字 " + _name);
                tTrans = permanentPool.Spawn(scenePool.prefabs[_name]);     //用其他池的tansform来创建自己池
            }
        }
        else if (_addType == EPoolAddType.Scene)
        {
            if (scenePool.prefabs.ContainsKey(_name))
            {
                tTrans = scenePool.Spawn(_name);
            }
            else if (permanentPool.prefabs.ContainsKey(_name))
            {
                //LoggerHelper.Debug("用 permanentPool 池的物体在自己池中创建个新prefabPool 名字 " + _name);
                tTrans = scenePool.Spawn(permanentPool.prefabs[_name]);     //用其他池的tansform来创建自己池
            }
        }
        if (tTrans == null)
        {
            //说明池中没有，开始加载此物体，并放入池中
            _WaitLoad.Enqueue(new PoolToolData(_name, _path, _data, _addType, _callback));
            if (_WaitLoad.Count == 1)
            {
                DoLoad();
            }
        }
        else
        {
            //tTrans.gameObject.SetActive(false);
            _callback(new PoolLoadData(_data, tTrans));
            //是否有待加载的物体，开始加载其
            if (_WaitLoad.Count != 0)
            {
                DoLoad();
            }
        }
    }
    /// <summary>
    /// 从Resourecs中获取物体
    /// </summary>
    public static void GetGameObjectFromResourecs(string _path, Action<PoolLoadData> _callback, object _data = null)
    {
        _path = _path.Substring(0, _path.LastIndexOf("."));
        Transform tTrans = GameObject.Instantiate(Resources.Load(_path) as GameObject).transform;
        PoolLoadData tData = new PoolLoadData(_data, tTrans);
        _callback.Invoke(tData);
    }
    /// <summary>
    /// 将物体放入池中
    /// </summary>
    public static void RemoveGameObject(Transform _trans, EPoolAddType _isScene = EPoolAddType.Scene)
    {
        if (_isScene == EPoolAddType.Permanent)
        {
            permanentPool.Despawn(_trans);
            _trans.parent = permanentPool.group;
        }
        else if (_isScene == EPoolAddType.Scene)
        {
            scenePool.Despawn(_trans);
            _trans.parent = scenePool.group;
        }
    }

    #region 加载obj
    static void DoLoad()
    {
        //开始加载
        AssetBundleLoadTool.Instance.LoadObj<GameObject>(_WaitLoad.Peek().path, OnLoaded);
    }
    static void OnLoaded(UnityEngine.Object _obj)
    {
        PoolToolData tData = _WaitLoad.Dequeue();
        Transform tTrans = null;
        //根据_data的情况，看放入哪个池
        if (tData.addType == EPoolAddType.No)
        {
            GameObject tObj = GameObject.Instantiate(_obj as GameObject);
            tObj.name = tData.name;
            tTrans = tObj.transform;
        }
        else
        {
            GameObject tObj = _obj as GameObject;
            if (tData.addType == EPoolAddType.Permanent)
            {
                tTrans = permanentPool.Spawn(tObj.transform);
            }
            else if (tData.addType == EPoolAddType.Scene)
            {
                tTrans = scenePool.Spawn(tObj.transform);
            }
        }
        tData.callback(new PoolLoadData(tData.data, tTrans));
        //是否有待加载的物体，开始加载其
        if (_WaitLoad.Count != 0)
        {
            DoLoad();
        }
    }
    #endregion

}
