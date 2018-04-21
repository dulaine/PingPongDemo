
using System.Collections.Generic;
using UnityEngine;

public class RoundInfo
{
    public string RoundID;
    public List<TrackInfo> TrackInfos;

    //start 从1开始
    public List<TrackInfo> GetTargetTracksByIndex(int start, int length)
    {
        if (TrackInfos == null)
        {
            Debug.LogError("轨迹信息为空");
            return null;
        }

        if (start < 1 || start > TrackInfos.Count || start + length - 1 > TrackInfos.Count)
        {
            Debug.LogError("范围超界 " + start + " " + length);
            return null;
        }

        List<TrackInfo> list = new List<TrackInfo>();
        int targetIndex = start - 1;
        int count = 0;
        for (int i = 0; i < TrackInfos.Count; i++)
        {
            if (i == targetIndex)
            {
                list.Add(TrackInfos[targetIndex]);
                targetIndex++;
                count++;
                if (count >= length) break;
            }
        }

        if (list.Count == 0)
        {
            Debug.LogError("没有找到目标轨迹");
        }

        return list;
    }

    public TrackInfo GetLastTrack()
    {
        List<TrackInfo> list = GetTargetTracksByIndex(TrackInfos.Count, 1);
        return list[0];
    }
}

public class TrackInfo
{
    public int Index;
    public string ID;
    public string RoundID;
    public List<FrameInfo> FrameInfos;
    public List<Vector3> TouchTablePoints;
    public List<int> TouchTableFrameNumber;
    public float StartTimeStamp;
    public float EndTimeStamp;
    public Vector3 TouchArea;//落地区域
    public float BallCenterFromBorder;//球心距离最近边界距离
    public float Rotate;
    public float XRotateSpeed;
    public float YRotateSpeed;
    public float Speed;
    public int OverNetHeight;

    public bool HasTouchPoint()
    {
        Vector3 point = GetLatestTouchTablePoint();
        if (point.x > 100000000f || point.y > 100000000f || point.z > 100000000f)
        {
            Debug.Log("没有touch point");
            return false;
        }
        else
        {
            return true;
        }
    }

    public Vector3 GetLatestTouchTablePoint()
    {
        if (TouchTablePoints.Count > 0)
        {
            return TouchTablePoints[TouchTablePoints.Count - 1];
        }
        else
        {
            Debug.LogError("没有落地点");
            return Vector3.zero;
        }
    }
}

public class FrameInfo
{
    public float FrameNumber;
    public Vector3 FramePos;
    public int TrackIndex;
    public string RoundID;
}
