

using System.Collections.Generic;
using Maticsoft;
using UnityEngine;

public class RoundInfoTransformer
{
    private static RoundInfoTransformer _instance;

    public static RoundInfoTransformer Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new RoundInfoTransformer();
            }
            return _instance;
        }
    }

    //frameInfo : "1, 1, 2, 1"
    public FrameInfo GetFrameInfo(string frameInfo, string roundID, int trackIndex)
    {
        string frameNO = frameInfo.Split(',')[0];
        string x = frameInfo.Split(',')[1];
        string y = frameInfo.Split(',')[2];
        string z = frameInfo.Split(',')[3];

        FrameInfo frame = GetFrameInfo(int.Parse(frameNO), float.Parse(x), float.Parse(y), float.Parse(z));
        frame.TrackIndex = trackIndex;
        frame.RoundID = roundID;
        return frame;
    }

    public FrameInfo GetFrameInfo(int frameNO, float x, float y, float z)
    {
        FrameInfo info = new FrameInfo();
        info.FramePos = TransformFromDB2Unity(new Vector3(x, y,z));
        info.FrameNumber = frameNO;
        
        return info;
    }


    //(12.0, 12.0, 12.0)
    public Vector3 GetPoint(string touchDownPoint)
    {
        string numStr = touchDownPoint.Substring(1, touchDownPoint.Length - 2);
        string[] nums = numStr.Split(',');
        float x = float.Parse(nums[0]);
        float y = float.Parse(nums[1]);
        float z = float.Parse(nums[2]);

        return  TransformFromDB2Unity(new Vector3(x, y, z));
    }


    public List<FrameInfo> ParseFrameString(string frameString, string roundID, int trackIndex)
    {
        string[] frameStrings = frameString.Split(';');
        List<FrameInfo> list = new List<FrameInfo>();
        for (int i = 0; i < frameStrings.Length; i++)
        {
            FrameInfo frameInfo = GetFrameInfo(frameStrings[i], roundID, trackIndex);
            list.Add(frameInfo);
        }

        return list;
    }


    public bool ValidTouchPoint(Vector3 point)
    {
        if (point.x > 100000000f || point.y > 100000000f || point.z > 100000000f)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public TrackInfo GetTrackInfo(DBTrack trackinfo)
    {
        TrackInfo info = new TrackInfo();

        info.RoundID = trackinfo.roundID;
        info.ID = trackinfo.ID;
        info.Index = trackinfo.Index;

        string[] horizonSplit = trackinfo.traceString.Split('|');
        string sign = horizonSplit[0];
        string frameString = horizonSplit[1];
        string touchPointsString = horizonSplit[2];
        string timesString = horizonSplit[3];
        string touchAreaString = horizonSplit[4];
        string rotationSpeedString = horizonSplit[5];

        if (sign != "1")
        {
            Debug.LogError("字符串不是以1开头, 不是轨迹格式:" + trackinfo.traceString);
            return null;
        }

        //落地点
        info.TouchTablePoints = new List<Vector3>();
        info.TouchTablePoints.Add(GetPoint(trackinfo.touchdown_p1));
        info.TouchTablePoints.Add(GetPoint(trackinfo.touchdown_p2));

        //开始结束时间
        string[] times = timesString.Split(';');
        info.StartTimeStamp = float.Parse(times[0]);
        info.EndTimeStamp = float.Parse(times[1]);

        info.OverNetHeight = (int)trackinfo.overNetHeight.Value;
        info.Speed = (int)trackinfo.maxSpeed.Value;
        info.Rotate = (int)trackinfo.maxRotateSpeed.Value;

        //帧信息
        info.FrameInfos = ParseFrameString(frameString, info.RoundID, info.Index);

        //touch point所在的frame位置, 先检测有几个合法的touch点
        List<Vector3> validTouchPointList = new List<Vector3>();
        for (int i = 0; i < info.TouchTablePoints.Count; i++)
        {
            Vector3 point = info.TouchTablePoints[i];
            if (ValidTouchPoint(point))
            {
                if (validTouchPointList.Count == 0 || Vector3.Distance(validTouchPointList[validTouchPointList.Count - 1], point) > 1f)
                {
                    validTouchPointList.Add(point);
                }
            }
        }

        info.TouchTableFrameNumber = new List<int>();
        if (validTouchPointList.Count > 0)
        {
            //记录距离touch point最近的Frame, index就是touchPoint在validTouchPointList的索引
            List<int> touchPointToFrameNumber = new List<int>();
            List<float> touchPointToDistance = new List<float>();
            for (int i = 0; i < validTouchPointList.Count; i++)
            {
                touchPointToDistance.Add(float.MaxValue);
                touchPointToFrameNumber.Add(0);
            }

            for (int i = 0; i < info.FrameInfos.Count; i++)
            {
                FrameInfo frame = info.FrameInfos[i];
                for (int j = 0; j < validTouchPointList.Count; j++)
                {
                    float dis = Vector3.Distance(frame.FramePos, validTouchPointList[j]);
                    if (dis < touchPointToDistance[j])
                    {
                        touchPointToDistance[j] = dis;
                        touchPointToFrameNumber[j] = i;
                    }
                    if (dis < 1f)
                    {
                        info.TouchTableFrameNumber.Add(i);
                    }
                }
            }

            if (info.TouchTableFrameNumber.Count != validTouchPointList.Count)
            {

#if UNITY_EDITOR
                Debug.LogWarning("落地点 不在球轨迹中, 落地点数量: " + validTouchPointList.Count + " 轨迹中有的落地点: " + info.TouchTableFrameNumber.Count);
                if (validTouchPointList.Count > 0)
                {
                    for (int i = 0; i < validTouchPointList.Count; i++)
                    {
                        Debug.LogWarning("落地点: " + validTouchPointList[i]);
                    }
                }
                if (info.TouchTableFrameNumber.Count > 0)
                {
                    for (int i = 0; i < info.TouchTableFrameNumber.Count; i++)
                    {
                        Debug.LogWarning("轨迹中落地点: " + info.FrameInfos[info.TouchTableFrameNumber[i]].FramePos);
                    }
                }

                for (int i = 0; i < touchPointToFrameNumber.Count; i++)
                {
                    Debug.LogWarning("记录的距离落地点最近的: " + info.FrameInfos[touchPointToFrameNumber[i]].FramePos);
                }
#endif

                info.TouchTableFrameNumber.Clear();
                for (int i = 0; i < touchPointToFrameNumber.Count; i++)
                {
                    info.TouchTableFrameNumber.Add(touchPointToFrameNumber[i]);
                }
            }
        }

        return info;
    }

    public RoundInfo GetRoundInfo(List<DBTrack> datalist)
    {
        RoundInfo info = new RoundInfo();

        info.RoundID = datalist[0].roundID;
        info.TrackInfos = new List<TrackInfo>();
        for (int i = 0; i < datalist.Count; i++)
        {
            info.TrackInfos.Add(GetTrackInfo(datalist[i]));
        }

        info.TrackInfos.Sort(Compare);
        return info;
    }

    int Compare(TrackInfo a, TrackInfo b)
    {
        return a.StartTimeStamp.CompareTo(b.StartTimeStamp);
    }


    private Vector3 TransformFromDB2Unity(Vector3 DB)
    {
        Vector3 unity = new Vector3(DB.x, DB.z, DB.y);
        return unity;
    }
}
