

using System.Collections.Generic;
using System.Linq;
using Maticsoft;
using UnityEngine;

public struct CurTrackInfo
{
    public string RoundID;
    public string TrackID;
    public int TrackIndex;

}
public class DataManager
{
    private static DataManager _instance;

    public static DataManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DataManager();
                _instance.Init();
            }

            return _instance;
        }
    }

    private Dictionary<string, RoundInfo> m_RoundInfoDic = new Dictionary<string, RoundInfo>();
    public Dictionary<string, RoundInfo> RoundInfoDic { get { return m_RoundInfoDic; } }

    private Dictionary<int, TrackInfo> m_TrackInfoDic = new Dictionary<int, TrackInfo>();
    public Dictionary<int, TrackInfo> TrackInfoDic { get { return m_TrackInfoDic; } }

    public CurTrackInfo CurTrackInfo = new CurTrackInfo();
    private void Init()
    {
    }

    private string GetLatestRoundID()
    {
        DBround round = DBAccessor.Instance.GetLatestRound();
        if (round == null)
        {
            Debug.LogError("获取最新的回合信息为空");
            return "";
        }

        return round.ID;
    }

    public RoundInfo GetLatestRoundInfo()
    {
        RoundInfo info = null;
        string roundID = GetLatestRoundID();
        if (!string.IsNullOrEmpty(roundID))
        {
            info = GetRoundInfo(roundID);
        }
        return info;
    }

    #region 获取回合信息

    public RoundInfo GetCurRoundInfo()
    {
        return GetRoundInfo(CurTrackInfo.RoundID);
    }

    public RoundInfo GetRoundInfo(string roundID)
    {
        if (m_RoundInfoDic.ContainsKey(roundID))
        {
            return m_RoundInfoDic[roundID];
        }
        else
        {
            List<DBTrack> list = GetRoundInfoFromDB(roundID.ToString());
            if (list.Count > 0)
            {
                RoundInfo info = RoundInfoTransformer.Instance.GetRoundInfo(list);
                m_RoundInfoDic.Add(roundID, info);
                return info;
            }
            else
            {
                Debug.LogError("can not find round info " + roundID);
                return null;
            }
        }
    }

    private List<DBTrack> GetRoundInfoFromDB(string roundID)
    {
        return DBAccessor.Instance.GetTrackInfoByRoundID(roundID.ToString());
    }

    public TrackInfo GetTrackInfoFromDB(string trackID)
    {
        DBTrack track = DBAccessor.Instance.GetTrackInfoByID(trackID);
        if (track == null || string.IsNullOrEmpty( track.ID))
        {
            Debug.LogWarning("没有轨迹资料" + trackID);
            return null;
        }
        return RoundInfoTransformer.Instance.GetTrackInfo(track);
    }

    public TrackInfo GetTrackInfoFromDB(int trackIndex)
    {
        if (m_TrackInfoDic.ContainsKey(trackIndex))
        {
            return m_TrackInfoDic[trackIndex];
        }
        else
        {
            DBTrack track = DBAccessor.Instance.GetTrackInfoByIndexID(trackIndex);
            if (track == null || string.IsNullOrEmpty(track.ID))
            {
                Debug.LogWarning("没有轨迹Index资料" + trackIndex);
                return null;
            }
            
            TrackInfo trackInfo = RoundInfoTransformer.Instance.GetTrackInfo(track);
            m_TrackInfoDic.Add(trackIndex, trackInfo);
            return trackInfo;
        }
    }

    #endregion

    #region 设置当前回合信息

    public void SetCurTrackInfo(string round, int trackIndex)
    {
        CurTrackInfo.TrackIndex = trackIndex;
        CurTrackInfo.RoundID = round;
        if(EventManager.RefreshTrackUI != null) EventManager.RefreshTrackUI();
    }

    #endregion

}
