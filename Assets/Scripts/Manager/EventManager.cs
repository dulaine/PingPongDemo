using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


class EventManager
{
    public static Action<string> DBStateChange;
    public static Action<TrackInfo> ShowTrackInfo;
    public static Action<Vector3> ShowTouchPoint;
    public static Action RefreshTrackUI;
}

