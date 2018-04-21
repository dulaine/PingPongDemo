using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private static CameraManager _instance;

    public static CameraManager Instance
    {
        get { return _instance; }
    }

    public GameObject HawkEyeCamera;
    public GameObject HawkEyeCamera2;

    void Awake()
    {
        _instance = this;
    }

    public GameObject GetCamera()
    {
        return HawkEyeCamera;
    }
    public GameObject GetCamera2()
    {
        return HawkEyeCamera2;
    }


    public void SetHawkEyeCameraPos(Vector3 pos)
    {
        HawkEyeCamera.transform.localPosition = pos;
        //GetCamera2().transform.localPosition = pos;
    }

    public void SetHawkEyeCameraDir(Vector3 dir)
    {
        HawkEyeCamera.transform.rotation = Quaternion.LookRotation(dir);
        //GetCamera2().transform.rotation = Quaternion.LookRotation(dir);
    }

    public void SetHawkEyeCameraActive(bool active)
    {
        HawkEyeCamera.SetActive(active);
        //GetCamera2().SetActive(active);
    }


    #region 选择摄像机

    public CameraID CurSelectedCamera = CameraID.Default;

    public void SelectedCamera(CameraID cameraID)
    {
        CurSelectedCamera = cameraID;
        CameraConfig config = ConfigManager.Instance.GetCameraConfig(cameraID);
        if (config != null) SetCameraByConfig(config);
        FunctionManager.Instance.EnterFixedCameraPos();
    }

    private void SetCameraByConfig(CameraConfig config)
    {
        GameObject obj = GetCamera();
        obj.transform.localPosition = config.Pos;
        obj.transform.localEulerAngles = config.Rotation;

    }

    #endregion


    public void CHooosecolor()
    {
        FunctionManager.Instance.StartChooseBillbard();
    }

}
