using System.Collections;
using UnityEngine;
using System;

/// <summary>
/// 初始化脚本
/// 
/// 注意：已将 Init_Main 和 UITool 中的 LuaTool全部注释，不用Lua
/// </summary>
public class GameRoot : MonoBehaviour
{

    public static GameRoot Instacne;
    public bool IsDebug = true;

    void Awake()
    {
        Instacne = this;
        DontDestroyOnLoad(this);
        Application.targetFrameRate = 30;       //设置平台帧率
        //MyDebug.IsDebug = IsDebug;      //是否打印日志

    }
    void Start()
    {
        DoInit();
    }

    void Destory()
    {
        EndProgram();
    }
    /// <summary>
    /// 初始化方法
    /// </summary>
    void DoInit()
    {
        ConfigManager.Instance.LoadConfig();

        //初始化球桌
        FunctionManager.Instance.InitTable();

        //初始化camera
        CameraManager.Instance.SelectedCamera(CameraID.Default);

        //初始化广告
        AdImageConfigData adConfig = ConfigManager.Instance.GetAdConfig();
        if(adConfig != null && !string.IsNullOrEmpty(adConfig.BillboardAdImagePath))FunctionManager.Instance.LoadAd(EnumAdImage.Billboard, adConfig.BillboardAdImagePath);
        if (adConfig != null && !string.IsNullOrEmpty(adConfig.StadiumAdPicPath)) FunctionManager.Instance.LoadAd(EnumAdImage.Stadium, adConfig.StadiumAdPicPath);

        //Test获取数据
       // DataManager.Instance.CurTrackInfo = new CurTrackInfo();
        RoundInfo info = DataManager.Instance.GetLatestRoundInfo();
        //DataManager.Instance.CurTrackInfo.RoundID = info.RoundID; ;//"2f94b24b-c80a-4233-a253-680d6c10ef8b";//info.RoundID;
        //DataManager.Instance.CurTrackInfo.TrackID = info.TrackInfos[0].ID; //"29a821c6-e3b5-409b-a5ad-2b9ebfb80334";//info.TrackInfos[info.TrackInfos.Count - 1].ID;
        //DataManager.Instance.CurTrackInfo.TrackIndex = info.TrackInfos[0].Index;
        DataManager.Instance.SetCurTrackInfo(info.RoundID, info.TrackInfos[0].Index);

        UIManager.Instance.ShowWindow(WindowID.MainUI);

        //接受数据
        RpcPortManager.Instance.StartDataRecieving();



    }

    void EndProgram()
    {
        RpcPortManager.Instance.EndDataRecieving();
    }
}
