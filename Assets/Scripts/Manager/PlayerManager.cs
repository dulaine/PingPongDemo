using System.Collections.Generic;
using UnityEngine;

enum PlayerMode
{
    None,
    PlayTrack,
    PlayHawkEye,
}

public class PlayerManager :MonoBehaviour
{
    public GameObject BallGameObject;

    private static PlayerManager _instance;

    public static PlayerManager Instance
    {
        get { return _instance; }
    }

    void Awake()
    {
        _instance = this;
    }

    private List<FrameInfo> m_FramesToPlay = new List<FrameInfo>();
    private int m_FramePlayingIndex = 0; //当前播放到第几个Frame
    private PlayerMode PlayMode = PlayerMode.None;

    private float m_PassedFixedUpdate = 0;
    private float m_TotalPlayTime = 0f;     //总播放时间
    private const float DBFRAMETimeDelta = 0.01f;//0.03f; //每个球之间的帧间隔
    private float m_RealTimeFrameInterval = 0f; //实际播放时候的间隔
    //private float m_RealTimeFrameInterval
    //{
    //    get { return DBFRAMETimeDelta / m_SpeedScale; }
    //}
    public float m_SpeedScale = 1f;

    private const float CameraDis = 500.0f;
    private List<TrackInfo> m_PlayingTrackList = new List<TrackInfo>();

    public void PlayeTrack(string roundID, int trackID, int length)
    {
        //获取最近的回合信息
        RoundInfo roundInfo = DataManager.Instance.GetRoundInfo(roundID);

        //获取轨迹信息
        List<TrackInfo> trackInfoList = roundInfo.GetTargetTracksByIndex(trackID, length);
        if (trackInfoList == null)
        {
            Debug.LogError("获取轨迹为空");
            return;
        }

        if (EventManager.ShowTrackInfo != null) EventManager.ShowTrackInfo(trackInfoList[0]);

        //按照轨迹顺序 播放轨迹 
        ResetPlayingData();
        for (int i = 0; i < trackInfoList.Count; i++)
        {
            TrackInfo info = trackInfoList[i];
            m_FramesToPlay.AddRange(info.FrameInfos);
        }
        PlayMode = PlayerMode.PlayTrack;

        //设置播放速度
        m_RealTimeFrameInterval = DBFRAMETimeDelta / m_SpeedScale;
        m_TotalPlayTime = (m_FramesToPlay.Count - 1) * m_RealTimeFrameInterval; //总播放时长

        //CameraManager.Instance.SetHawkEyeCameraActive(false);
        m_PlayingTrackList = trackInfoList;
    }
    public void PlayTrack(string trackID)
    {
        TrackInfo info = DataManager.Instance.GetTrackInfoFromDB(trackID);

        if (info == null || string.IsNullOrEmpty(info.RoundID))
        {
            Debug.LogError("获取轨迹为空");
            return;
        }

        PlayTrack(info);
    }
    public void PlayTrack(int trackIndexID)
    {
        TrackInfo info = DataManager.Instance.GetTrackInfoFromDB(trackIndexID);

        if (info == null || string.IsNullOrEmpty(info.RoundID))
        {
            Debug.LogError("获取轨迹为空");
            return;
        }

        PlayTrack(info);
    }

    private void PlayTrack(TrackInfo info)
    {
        if (EventManager.ShowTrackInfo != null) EventManager.ShowTrackInfo(info);

        //按照轨迹顺序 播放轨迹 

        ResetPlayingData();

        m_FramesToPlay.AddRange(info.FrameInfos);

        PlayMode = PlayerMode.PlayTrack;

        //设置播放速度
       m_RealTimeFrameInterval = DBFRAMETimeDelta / m_SpeedScale;
        m_TotalPlayTime = (m_FramesToPlay.Count - 1) * m_RealTimeFrameInterval; //总播放时长

        m_PlayingTrackList.Clear();
        m_PlayingTrackList.Add(info);
    }

    public void StopPlay()
    {
        PlayMode = PlayerMode.None;
    }

    private void ResetPlayingData()
    {
        PlayMode = PlayerMode.None;
        m_FramePlayingIndex = 0;
        m_PassedFixedUpdate = 0f;
        m_FramesToPlay.Clear();
    }

    //根据当前时间, 设定每个球播放的Frame间隔. 和每个Frame中的位置, 返回正确的位置
    private Vector3 GetPosition(float playTimePass, float targetFrameTimeDelta, List<FrameInfo> frameList)
    {
        int startFrame = Mathf.FloorToInt(playTimePass / targetFrameTimeDelta); //当要播放的帧
        int endFrame = startFrame + 1;

        if (endFrame > frameList.Count - 1) return frameList[frameList.Count - 1].FramePos;//

        float lerpPercentage = (playTimePass % targetFrameTimeDelta) / targetFrameTimeDelta;

        Vector3 startPos = frameList[startFrame].FramePos;
        Vector3 endPos = frameList[endFrame].FramePos;
        Vector3 pos = Vector3.Lerp(startPos, endPos, lerpPercentage);

        return pos;
    }

    private void SetCurTrackInfo(float playTimePass, float targetFrameTimeDelta, List<FrameInfo> frameList)
    {
        int startFrame = Mathf.FloorToInt(playTimePass / targetFrameTimeDelta); //当要播放的帧
        int endFrame = startFrame + 1;
        if (endFrame > frameList.Count - 1) endFrame = frameList.Count - 1;
        DataManager.Instance.SetCurTrackInfo(frameList[endFrame].RoundID, frameList[endFrame].TrackIndex);
    }

    private void ShowTouchPoint(float playTimePass, float targetFrameTimeDelta, List<FrameInfo> frameList, int trackIndex)
    {
        //获取之前播放了几个track, 知道过了多少frame
        int playingIndex = 0;
        for (int i = 0; i < m_PlayingTrackList.Count; i++)
        {
            if (m_PlayingTrackList[i].Index == trackIndex)
            {
                playingIndex = i;
                break;
            }
        }
        int framePassed = 0;
        for (int i = 0; i < playingIndex; i++)
        {
            framePassed += m_PlayingTrackList[i].FrameInfos.Count;
        }

        TrackInfo trackInfo = DataManager.Instance.GetTrackInfoFromDB(trackIndex);

        int curFrame = Mathf.FloorToInt(playTimePass / targetFrameTimeDelta) - framePassed; //当要播放的帧
        int nxFrame = Mathf.FloorToInt((playTimePass + Time.fixedDeltaTime) / targetFrameTimeDelta) - framePassed; //当要播放的帧

        for (int i = 0; i < trackInfo.TouchTableFrameNumber.Count; i++)
        {
            int touchTableFrame = trackInfo.TouchTableFrameNumber[i];
            if (curFrame < touchTableFrame && nxFrame >= touchTableFrame)
            {
                //下个frame碰到桌面
                if(EventManager.ShowTouchPoint != null) EventManager.ShowTouchPoint(trackInfo.FrameInfos[touchTableFrame].FramePos);
                break;
            }
        }
    }

    private void PlayTrackUpdate()
    {
        if (PlayMode != PlayerMode.PlayTrack) return;

        //没有播放内容
        if (m_FramesToPlay.Count == 0)
        {
            PlayMode = PlayerMode.None;
            return;
        }

        //当前播放的Frames
        BallGameObject.transform.localPosition = GetPosition(m_PassedFixedUpdate, m_RealTimeFrameInterval, m_FramesToPlay);

        //设置当前的track ID
        SetCurTrackInfo(m_PassedFixedUpdate, m_RealTimeFrameInterval, m_FramesToPlay);

        //如果到了Touch 点, 显示Touchpoint
        ShowTouchPoint(m_PassedFixedUpdate, m_RealTimeFrameInterval, m_FramesToPlay, DataManager.Instance.CurTrackInfo.TrackIndex);

        if (m_PassedFixedUpdate >= (m_TotalPlayTime - Mathf.Epsilon))
        {
            m_PassedFixedUpdate = 0f;
            PlayMode = PlayerMode.None;
            return;
        }

        m_PassedFixedUpdate += Time.fixedUnscaledDeltaTime;
        Mathf.Clamp(m_PassedFixedUpdate, 0f, m_TotalPlayTime);
    }

    void FixedUpdate()
    {
        PlayTrackUpdate();
        PlayHawkEyeUpdate();
    }


    #region 鹰眼挑战

    public void PlayHawkEyeChallenge(string trackID)
    {
        //获取轨迹信息
        TrackInfo track = DataManager.Instance.GetTrackInfoFromDB(trackID);
        if (track == null) return;
        PlayHawkEyeByTrackInfo(track);
    }

    public void PlayHawkEyeChallenge(int trackIndexID)
    {
        //获取轨迹信息
        TrackInfo track = DataManager.Instance.GetTrackInfoFromDB(trackIndexID);
        if (track == null) return;
        PlayHawkEyeByTrackInfo(track);
    }

    private void PlayHawkEyeByTrackInfo(TrackInfo track)
    {
        ResetPlayingData();

        //添加frame 直到最后一个落点.
        if (!track.HasTouchPoint()) return;
        Vector3 touchTablePoint = track.GetLatestTouchTablePoint();

        //最后一个落点的index
        //float leastTouch = float.MaxValue;
        //float leastTouchIndex = 0;
        //for (int i = 0; i < track.FrameInfos.Count; i++)
        //{
        //    float dis = Vector3.Distance(track.FrameInfos[i].FramePos, touchTablePoint);
        //    if (leastTouch > dis)
        //    {
        //        leastTouch = dis;
        //        leastTouchIndex = i;
        //    }
        //}
        //for (int i = 0; i < leastTouchIndex; i++)
        //{
        //    m_FramesToPlay.Add(track.FrameInfos[i]);
        //}
        int touchFrame = track.TouchTableFrameNumber[track.TouchTableFrameNumber.Count - 1];
        for (int i = 0; i <= touchFrame; i++)
        {
            m_FramesToPlay.Add(track.FrameInfos[i]);
        }

        PlayMode = PlayerMode.PlayHawkEye;

        //设置播放速度
        m_RealTimeFrameInterval = DBFRAMETimeDelta;//TimeDelta / m_SpeedScale;
        m_TotalPlayTime = (m_FramesToPlay.Count - 1) * m_RealTimeFrameInterval; //总播放时长
    }

    private void PlayHawkEyeUpdate()
    {
        if (PlayMode != PlayerMode.PlayHawkEye) return;

        //没有播放内容
        if (m_FramesToPlay.Count == 0)
        {
            PlayMode = PlayerMode.None;
            return;
        }

        //当前播放的Frames
        BallGameObject.transform.localPosition = GetPosition(m_PassedFixedUpdate, m_RealTimeFrameInterval, m_FramesToPlay);

        //如果到了Touch 点, 显示Touchpoint
        ShowTouchPoint(m_PassedFixedUpdate, m_RealTimeFrameInterval, m_FramesToPlay, DataManager.Instance.CurTrackInfo.TrackIndex);

        if (m_PassedFixedUpdate >= (m_TotalPlayTime - Mathf.Epsilon))
        {
            m_PassedFixedUpdate = 0f;
            PlayMode = PlayerMode.None;
            //播放结束, 摄像机动画
            FinaliseHawkEyeCamera();
            return;
        }

        SetHawkEyeCamera(m_PassedFixedUpdate, m_RealTimeFrameInterval, m_FramesToPlay);

        m_PassedFixedUpdate += Time.fixedUnscaledDeltaTime;
        Mathf.Clamp(m_PassedFixedUpdate, 0f, m_TotalPlayTime);
    }

    #region 摄像机

    //根据当前时间, 设定每个球播放的Frame间隔. 和每个Frame中的位置, 返回正确的位置
    private Vector3 GetDirection(float playTimePass, float targetFrameTimeDelta, List<FrameInfo> frameList)
    {
        int startFrame = Mathf.FloorToInt(playTimePass / targetFrameTimeDelta); //当要播放的帧
        int endFrame = startFrame + 1;

        if (endFrame > frameList.Count - 1) return Vector3.down;//

        Vector3 startPos = frameList[startFrame].FramePos;
        Vector3 endPos = frameList[endFrame].FramePos;

        if (startPos == endPos)
        {
            Debug.LogWarning("位置信息相同:" + startPos + " frame no:" + startFrame);
        }

        return (endPos - startPos).normalized;
    }

    private Vector3 GetCameraPos(Vector3 targetPos, float dis, Vector3 dir)
    {
        return targetPos - dir * dis;
    }

    //初始化到球的后尾, 和球平行运动
    void SetHawkEyeCamera(float playTimePass, float targetFrameTimeDelta, List<FrameInfo> frameList)
    {
        Vector3 dir = GetDirection(playTimePass, targetFrameTimeDelta, frameList);
        
        CameraManager.Instance.SetHawkEyeCameraPos(GetCameraPos(BallGameObject.transform.localPosition, CameraDis, dir));
        CameraManager.Instance.SetHawkEyeCameraDir(dir);
        CameraManager.Instance.SetHawkEyeCameraActive(true);
    }

    //最终把摄像机从上向下看
    private void FinaliseHawkEyeCamera()
    {
        CameraManager.Instance.SetHawkEyeCameraPos(GetCameraPos(BallGameObject.transform.localPosition, CameraDis, Vector3.down));
        CameraManager.Instance.SetHawkEyeCameraDir(Vector3.down);
        CameraManager.Instance.SetHawkEyeCameraActive(true);
    }

    #endregion
    #endregion


  

}
