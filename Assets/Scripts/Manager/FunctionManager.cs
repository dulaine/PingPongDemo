

using System.Collections.Generic;
using UnityEngine;

public enum FunctionState
{
    Default,
    ConfigCamera,
    FreeCamera,
    ChooseBillbard,
    ChangeColor,
    ConfigTable,
    FixedCameraPos,
}
public enum CameraID
{
    Default,
    Camera1,
    Camera2,
    Camera3,
    Camera4,
    Camera5,
    Camera6,
    Camera7,
    Camera8,
    Camera9,
    FreeCamera,
}

public class FunctionManager
{
    private static FunctionManager _instance;

    public static FunctionManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = new FunctionManager();
            }
            return _instance;
        }
    }

    public FunctionState PreState = FunctionState.Default;
    public FunctionState CurState = FunctionState.Default;

    private void SetNewState(FunctionState newState)
    {
        if (CurState == newState) return;
        PreState = CurState;
        CurState = newState;
    }

    private void RestoreToPreState()
    {
        CurState = PreState;
    }

    #region 显示落点

    //显示落地点
    ////根据信息, 根据每个轨迹, 获取所有落地点;  在所有落地点,显示一个图片Obj
    public void ShowTouchTablePoints()
    {
        TrackInfo track = DataManager.Instance.GetTrackInfoFromDB(DataManager.Instance.CurTrackInfo.TrackIndex);
        List<Vector3> touchPoints = track.TouchTablePoints;

        for (int i = 0; i < touchPoints.Count; i++)
        {
            ShowTouchPoint(touchPoints[i]);
        }

    }

    public void ShowTouchPoint(Vector3 touchPoint)
    {
        GameObject touchPointGameObject = ResourceManager.Instance.GetTouchPointGameObject();
        touchPointGameObject.transform.localPosition = touchPoint;
    }

    public void HideTouchTablePoints()
    {
        ResourceManager.Instance.HideTouchPoints();
    }

    #endregion

    #region 配置摄像机

    private CameraID CurConfigCameraID;
    public void StartCameraConfig(CameraID configCameraID)
    {
        if(CurState != FunctionState.ConfigCamera) SetNewState(FunctionState.ConfigCamera); //重复进入状态
        CurConfigCameraID = configCameraID;
    }

    public void SaveCameraConfig()
    {
        if (CurState != FunctionState.ConfigCamera) return;
        GameObject camera = CameraManager.Instance.GetCamera();
        ConfigManager.Instance.UpdateCameraConfig(CurConfigCameraID, camera.transform.localPosition, camera.transform.localEulerAngles);
        ConfigManager.Instance.SaveCameraCofig();
        RestoreToPreState();
    }

    public void CancelCameraConfig()
    {
        if (CurState != FunctionState.ConfigCamera) return;
        RestoreToPreState();
    }
    #endregion

    #region EnterFreeModeCamera

    public void EnterFreeCameraMode()
    {
        SetNewState(FunctionState.FreeCamera);
    }

    public void ExitFreeCameraMode()
    {
        if (CurState != FunctionState.FreeCamera) return;
        RestoreToPreState();
    }


    #endregion

    #region 设置9个camera位置

    public void EnterFixedCameraPos()
    {
        SetNewState(FunctionState.FixedCameraPos);
    }

    #endregion

    #region 换色

    public void StartChangeColorFor(Renderer renderer)
    {
        SetNewState(FunctionState.ChangeColor);
        ColorPickerManager.Instance.PickColor(renderer);
        EndChangeColor();
    }
    public void StartChangeColorFor(Material material)
    {
        SetNewState(FunctionState.ChangeColor);
        ColorPickerManager.Instance.PickColor(material);
        EndChangeColor();
    }

    public void EndChangeColor()
    {
        if (CurState != FunctionState.ChangeColor) return;
        RestoreToPreState();
    }

    #endregion

    #region 点选广告版

    public void StartChooseBillbard()
    {
        SetNewState(FunctionState.ChooseBillbard);
    }

    public void EndChooseBillard()
    {
        if (CurState != FunctionState.ChooseBillbard) return;
        RestoreToPreState();
    }

    public void LoadAd(EnumAdImage type, string path)
    {
        OpenFileManager.Instance.LoadAd(type, path);
    }

    #endregion

    #region AdjustTable

    public void InitTable()
    {
        TableConfigData config = ConfigManager.Instance.GetTableConfig();
        if (config == null) return;
        GameObject table = ResourceManager.Instance.TableObj;
        table.transform.localPosition = config.Pos;
        table.transform.localEulerAngles = config.Rotation;
    }

    public void AdjustTable()
    {
        SetNewState(FunctionState.ConfigTable);
    }

    public void CancelAdjustTable()
    {
        if (CurState != FunctionState.ConfigTable) return;
        RestoreToPreState();
    }

    public void EndAdjustTable()
    {
        if (CurState != FunctionState.ConfigTable) return;
        SaveTableConfig();
        RestoreToPreState();
    }

    private void SaveTableConfig()
    {
        GameObject table = ResourceManager.Instance.GetTable();
        ConfigManager.Instance.UpdateTableConfig(table.transform.localPosition, table.transform.localEulerAngles);
        ConfigManager.Instance.SaveTableCofig();
    }

    #endregion

    //多屏幕显示
    public void MultiDisplaye()
    {
       MultiDisplayManager.Instance.Open();
    }

}
