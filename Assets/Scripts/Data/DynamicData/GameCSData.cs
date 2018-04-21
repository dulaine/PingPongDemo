using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class GameCSData
{
    static GameCSData _Instance;
    public static GameCSData Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = new GameCSData();
            }
            return _Instance;
        }
    }
    const string cPath = "/conf/cameraConf.json";

    //配置表数据
    public ConfigTableBase<UIConfigTable> UIConfig;             //UI配置



    [Serializable]

    public class CameraData
    {
        public Vector3 Position;
        public Vector3 Rotations;
    }
    [Serializable]

    public class AllData
    {
        public List<CameraData> datas = new List<CameraData>();
    }

    public static void Init()
    {

    }

    #region 配置读取

    public void ReadCamera9Conf()
    {
        var temps = IOTool.LoadFileString(Application.dataPath + cPath);
        var data = JsonUtility.FromJson<AllData>(temps);
        Debug.Log(data.datas.Count);
    }
    public void WriteCamera9Conf(object _data)
    {
        var json = JsonUtility.ToJson(_data);
        Debug.Log(json);
        IOTool.CreateFileString(Application.dataPath + cPath, json);
    }
    #endregion
}
