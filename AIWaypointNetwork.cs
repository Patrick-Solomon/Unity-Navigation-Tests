using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PathDisplayMode { None, Connections, Paths }
public class AIWaypointNetwork : MonoBehaviour
{
    [HideInInspector]
    public PathDisplayMode displayMode = PathDisplayMode.Connections;

    [HideInInspector]
    public int UIStart = 0; //First waypoint to draw

    [HideInInspector]
    public int UIEnd = 0; //Last Waypoint to draw

    public List<Transform> waypoints = new List<Transform>();
}


