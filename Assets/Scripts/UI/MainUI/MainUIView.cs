
/*
    file desc: Auto Generation by [UGUIScriptGenerator] 
*/


using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace PingpongUI
{
public class MainUIView : UIViewBase
{
    public Button UI_Button_Track;
    public Button UI_Button_Follow;
    public Button UI_Button_Main;
    public Button UI_Button_ShowPoint;
    public Button UI_Button_Statistics;
    public Button UI_Button_MutiDisplay;
    public Text UI_CurTrackIDText;
    public Button UI_Button_Pre;
    public Button UI_Button_Next;
    public InputField UI_TrackID;
    public Button UI_Button_PlayTrack;
    public Button UI_Button_PlayeTrackSeq;
    public Button UI_Button_ReconnectDB;
    public Text UI_DatabaseState;
    public Button UI_Button1;
    public Button UI_Button2;
    public Button UI_Button3;
    public Button UI_Button4;
    public Button UI_Button5;
    public Button UI_Button6;
    public Button UI_Button7;
    public Button UI_Button8;
    public Button UI_Button9;
    public Button UI_Button_About;
    public Button UI_Button_Exit;
    public Button UI_Button_Set;
    public Button UI_TableColor;
    public Button UI_BallColor;
    public Button UI_FloorColor;
    public Button UI_Billboard;
    public Button UI_StadiumAd;
    public Button UI_PlaySpeed;
    public Slider UI_PlaySpeedSlider;
    public Button UI_AdjustTable;
    public Toggle UI_IsShowMoveSpeed;
    public Toggle UI_IsShowRoutateSpeed;
    public Toggle UI_IsShowHeight;
    public Toggle UI_IsShowRole;
    public Toggle UI_IsShowPoint;
    public Toggle UI_IsShowScore;
    public Button UI_SetCameraConfig;
    public Button UI_SaveCameraCofig;
    public Button UI_CancelCameraCofig;
    public Button UI_Button_FreeCamera;


    protected override void SetWindowId()
    {
        ID = WindowID.MainUI;
    }

    protected override void InitWindowConfigData()
    {
    }

    protected override void InitWindowOnAwake()
    {
    if (UI_Button_Track != null) return;
        List<Button> buttonCache = GameUIUtility.GetComponentCache<Button>(this.gameObject);
        List<Text> textCache = GameUIUtility.GetComponentCache<Text>(this.gameObject);
        List<InputField> inputfieldCache = GameUIUtility.GetComponentCache<InputField>(this.gameObject);
        List<Slider> sliderCache = GameUIUtility.GetComponentCache<Slider>(this.gameObject);
        List<Toggle> toggleCache = GameUIUtility.GetComponentCache<Toggle>(this.gameObject);

        UI_Button_Track = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_Track");
        if (UI_Button_Track == null) Debug.LogError("UI_Button_Track获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_Follow = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_Follow");
        if (UI_Button_Follow == null) Debug.LogError("UI_Button_Follow获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_Main = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_Main");
        if (UI_Button_Main == null) Debug.LogError("UI_Button_Main获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_ShowPoint = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_ShowPoint");
        if (UI_Button_ShowPoint == null) Debug.LogError("UI_Button_ShowPoint获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_Statistics = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_Statistics");
        if (UI_Button_Statistics == null) Debug.LogError("UI_Button_Statistics获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_MutiDisplay = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_MutiDisplay");
        if (UI_Button_MutiDisplay == null) Debug.LogError("UI_Button_MutiDisplay获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_CurTrackIDText = GameUIUtility.GetComponentFromCache(ref textCache, "UI_CurTrackIDText");
        if (UI_CurTrackIDText == null) Debug.LogError("UI_CurTrackIDText获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_Pre = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_Pre");
        if (UI_Button_Pre == null) Debug.LogError("UI_Button_Pre获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_Next = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_Next");
        if (UI_Button_Next == null) Debug.LogError("UI_Button_Next获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_TrackID = GameUIUtility.GetComponentFromCache(ref inputfieldCache, "UI_TrackID");
        if (UI_TrackID == null) Debug.LogError("UI_TrackID获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_PlayTrack = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_PlayTrack");
        if (UI_Button_PlayTrack == null) Debug.LogError("UI_Button_PlayTrack获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_PlayeTrackSeq = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_PlayeTrackSeq");
        if (UI_Button_PlayeTrackSeq == null) Debug.LogError("UI_Button_PlayeTrackSeq获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_ReconnectDB = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_ReconnectDB");
        if (UI_Button_ReconnectDB == null) Debug.LogError("UI_Button_ReconnectDB获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_DatabaseState = GameUIUtility.GetComponentFromCache(ref textCache, "UI_DatabaseState");
        if (UI_DatabaseState == null) Debug.LogError("UI_DatabaseState获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button1 = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button1");
        if (UI_Button1 == null) Debug.LogError("UI_Button1获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button2 = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button2");
        if (UI_Button2 == null) Debug.LogError("UI_Button2获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button3 = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button3");
        if (UI_Button3 == null) Debug.LogError("UI_Button3获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button4 = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button4");
        if (UI_Button4 == null) Debug.LogError("UI_Button4获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button5 = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button5");
        if (UI_Button5 == null) Debug.LogError("UI_Button5获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button6 = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button6");
        if (UI_Button6 == null) Debug.LogError("UI_Button6获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button7 = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button7");
        if (UI_Button7 == null) Debug.LogError("UI_Button7获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button8 = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button8");
        if (UI_Button8 == null) Debug.LogError("UI_Button8获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button9 = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button9");
        if (UI_Button9 == null) Debug.LogError("UI_Button9获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_About = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_About");
        if (UI_Button_About == null) Debug.LogError("UI_Button_About获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_Exit = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_Exit");
        if (UI_Button_Exit == null) Debug.LogError("UI_Button_Exit获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_Set = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_Set");
        if (UI_Button_Set == null) Debug.LogError("UI_Button_Set获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_TableColor = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_TableColor");
        if (UI_TableColor == null) Debug.LogError("UI_TableColor获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_BallColor = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_BallColor");
        if (UI_BallColor == null) Debug.LogError("UI_BallColor获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_FloorColor = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_FloorColor");
        if (UI_FloorColor == null) Debug.LogError("UI_FloorColor获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Billboard = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Billboard");
        if (UI_Billboard == null) Debug.LogError("UI_Billboard获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_StadiumAd = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_StadiumAd");
        if (UI_StadiumAd == null) Debug.LogError("UI_StadiumAd获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_PlaySpeed = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_PlaySpeed");
        if (UI_PlaySpeed == null) Debug.LogError("UI_PlaySpeed获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_PlaySpeedSlider = GameUIUtility.GetComponentFromCache(ref sliderCache, "UI_PlaySpeedSlider");
        if (UI_PlaySpeedSlider == null) Debug.LogError("UI_PlaySpeedSlider获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_AdjustTable = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_AdjustTable");
        if (UI_AdjustTable == null) Debug.LogError("UI_AdjustTable获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_IsShowMoveSpeed = GameUIUtility.GetComponentFromCache(ref toggleCache, "UI_IsShowMoveSpeed");
        if (UI_IsShowMoveSpeed == null) Debug.LogError("UI_IsShowMoveSpeed获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_IsShowRoutateSpeed = GameUIUtility.GetComponentFromCache(ref toggleCache, "UI_IsShowRoutateSpeed");
        if (UI_IsShowRoutateSpeed == null) Debug.LogError("UI_IsShowRoutateSpeed获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_IsShowHeight = GameUIUtility.GetComponentFromCache(ref toggleCache, "UI_IsShowHeight");
        if (UI_IsShowHeight == null) Debug.LogError("UI_IsShowHeight获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_IsShowRole = GameUIUtility.GetComponentFromCache(ref toggleCache, "UI_IsShowRole");
        if (UI_IsShowRole == null) Debug.LogError("UI_IsShowRole获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_IsShowPoint = GameUIUtility.GetComponentFromCache(ref toggleCache, "UI_IsShowPoint");
        if (UI_IsShowPoint == null) Debug.LogError("UI_IsShowPoint获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_IsShowScore = GameUIUtility.GetComponentFromCache(ref toggleCache, "UI_IsShowScore");
        if (UI_IsShowScore == null) Debug.LogError("UI_IsShowScore获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_SetCameraConfig = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_SetCameraConfig");
        if (UI_SetCameraConfig == null) Debug.LogError("UI_SetCameraConfig获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_SaveCameraCofig = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_SaveCameraCofig");
        if (UI_SaveCameraCofig == null) Debug.LogError("UI_SaveCameraCofig获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_CancelCameraCofig = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_CancelCameraCofig");
        if (UI_CancelCameraCofig == null) Debug.LogError("UI_CancelCameraCofig获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
        UI_Button_FreeCamera = GameUIUtility.GetComponentFromCache(ref buttonCache, "UI_Button_FreeCamera");
        if (UI_Button_FreeCamera == null) Debug.LogError("UI_Button_FreeCamera获取为空, 请检查是不是没用Ebo.UI的脚本!!!");
    }

    protected override void InitController()
    {
        Controller = new MainUIControl(this);
    }

}
}

