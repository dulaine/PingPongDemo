

using System.Collections.Generic;
using System.Data;
using Maticsoft;
using UnityEngine;

public class DBAccessor
{
    private static  DBAccessor _instance;

    public static DBAccessor Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new DBAccessor();
            }

            return _instance;
        }
    }


    public DBPlayer GetPlayer(string playerId)
    {
        DBPlayer player = new DBPlayer(playerId);
        return player;
    }

    public DBDiagram GetPhoto(string id)
    {
        DBDiagram photo = new DBDiagram(id);
        return photo;
    }

    public DBround GetRoundInfo(string id)
    {
        DBround round = new DBround(id);
        return round;
    }

    public DBround GetLatestRound()
    {
        DBround latestR = new DBround();
        DataSet dataSet = latestR.GetLatest();
        if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
        {
            Debug.LogWarning("没有最新的信息");
            return null;
        }

        List<DBround> list = latestR.Fill(dataSet);
        return list[0];
    }

    public List<DBTrack> GetTrackInfoByRoundID(string roundID)
    {
        DBTrack track = new DBTrack();
        DataSet dataSet = track.GetList(" roundID = \"" + roundID + "\"");
        List<DBTrack> list = track.Fill(dataSet);
        return list;
    }

    public DBTrack GetTrackInfoByID(string trackID)
    {
        DBTrack track = new DBTrack(trackID);
        return track;
    }

    public DBTrack GetTrackInfoByIndexID(int indexID)
    {
        DBTrack track = new DBTrack(indexID);
        return track;
    }

}
