
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CameraConfig
{
    public int ID;
    public Vector3 Pos;
    public Vector3 Rotation;
}

[Serializable]
public class ConfigData
{
    public List<CameraConfig> Configs;

}

[Serializable]
public class TableConfigData
{
    public Vector3 Pos;
    public Vector3 Rotation;
}


[Serializable]
public class AdImageConfigData
{
    public string BillboardAdImagePath;
    public string StadiumAdPicPath;
}