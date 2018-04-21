

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public enum EnumAdImage
{
    Billboard,
    Stadium,
}

public class ConfigManager
{
    private static ConfigManager _instance;

    public static ConfigManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new ConfigManager();
            }
            return _instance;
        }
    }

    private string m_FileName = "config.json";
    private string m_TableFileName = "tableConfig.json";
    private string m_AdFileName = "adImageonfig.json";
    private ConfigData m_CameraConfigData;
    private TableConfigData m_TableConfigData;
    private AdImageConfigData m_AdImageConfigData;

    public void LoadConfig()
    {
        LoadCameraConfig();
        LoadTableConfig();
        LoadAdConfig();
        //GameObject.Find("ConfigText").GetComponent<UnityEngine.UI.Text>().text = text;
    }

    #region 摄像机配置

    public void LoadCameraConfig()
    {
        string path = Application.streamingAssetsPath + "/" + m_FileName;
        string text = File.ReadAllText(path);
        m_CameraConfigData = JsonUtility.FromJson<ConfigData>(text);
    }

    public CameraConfig GetCameraConfig(CameraID cameraID)
    {
        List<CameraConfig> list = m_CameraConfigData.Configs;
        bool find = false;
        for (int i = 0; i < list.Count; i++)
        {
            CameraConfig config = list[i];
            if (config.ID == (int)cameraID)
            {
                return config;
            }
        }

        return null;
    }

    public void UpdateCameraConfig(CameraID CameraID, Vector3 pos, Vector3 rotation)
    {
        List<CameraConfig> list = m_CameraConfigData.Configs;
        bool find = false;
        for (int i = 0; i < list.Count; i++)
        {
            CameraConfig config = list[i];
            if (config.ID == (int)CameraID)
            {
                config.Pos = pos;
                config.Rotation = rotation;
                find = true;
                break;
            }
        }

        if (!find)
        {
            CameraConfig newConfig = new CameraConfig();
            newConfig.ID = (int)CameraID;
            newConfig.Pos = pos;
            newConfig.Rotation = rotation;
            m_CameraConfigData.Configs.Add(newConfig);
        }
    }

    public void SaveCameraCofig()
    {
        //m_ConfigData = new ConfigData();
        //m_ConfigData.Configs = new List<CameraConfig>();
        //for (int i = 0; i < 3; i++)
        //{
        //    CameraConfig config = new CameraConfig();
        //    config.ID = i;
        //    config.Pos = new Vector3();
        //    config.Rotation = new Vector3();
        //    m_ConfigData.Configs.Add(config);
        //}

        string path = Application.streamingAssetsPath + "/" + m_FileName;
        string json = JsonUtility.ToJson(m_CameraConfigData, true);
        File.WriteAllText(path, json);
    }
    #endregion

    #region TableConfig
    public void LoadTableConfig()
    {
        string path = Application.streamingAssetsPath + "/" + m_TableFileName;
        string text = File.ReadAllText(path);
        m_TableConfigData = JsonUtility.FromJson<TableConfigData>(text);
    }

    public void SaveTableCofig()
    {
        string path = Application.streamingAssetsPath + "/" + m_TableFileName;
        string json = JsonUtility.ToJson(m_TableConfigData, true);
        File.WriteAllText(path, json);
        LoadTableConfig();
    }

    public TableConfigData GetTableConfig()
    {
        if (m_TableConfigData == null)
        {
            m_TableConfigData = new TableConfigData();
            m_TableConfigData.Pos = Vector3.zero;
            m_TableConfigData.Rotation = Vector3.zero;
        }
        return m_TableConfigData;
    }

    public void UpdateTableConfig(Vector3 pos, Vector3 rotation)
    {
        if (m_TableConfigData == null) m_TableConfigData = new TableConfigData();
        m_TableConfigData.Pos = pos;
        m_TableConfigData.Rotation = rotation;
    }

    #endregion

    #region AdConfig
    public void LoadAdConfig()
    {
        string path = Application.streamingAssetsPath + "/" + m_AdFileName;
        if (!File.Exists(path)) return;
        string text = File.ReadAllText(path);
        m_AdImageConfigData = JsonUtility.FromJson<AdImageConfigData>(text);
    }

    public void SaveAdCofig()
    {
        string path = Application.streamingAssetsPath + "/" + m_AdFileName;
        string json = JsonUtility.ToJson(m_AdImageConfigData, true);
        File.WriteAllText(path, json);
        LoadAdConfig();
    }

    public AdImageConfigData GetAdConfig()
    {
        if (m_AdImageConfigData == null)
        {
            //m_AdImageConfigData = new TableConfigData();
            //m_TableConfigData.Pos = Vector3.zero;
            //m_TableConfigData.Rotation = Vector3.zero;
        }
        return m_AdImageConfigData;
    }

    public void UpdateAdConfig(EnumAdImage image, string path)
    {
        if (m_AdImageConfigData == null) m_AdImageConfigData = new AdImageConfigData();
        switch (image)
        {
            case EnumAdImage.Billboard:
                m_AdImageConfigData.BillboardAdImagePath = path;
                break;
            case EnumAdImage.Stadium:
                m_AdImageConfigData.StadiumAdPicPath = path;
                break;
            default:
                throw new ArgumentOutOfRangeException("image", image, null);
        }
    }

    #endregion
}
