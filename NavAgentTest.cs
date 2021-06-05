using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NavAgentTest : MonoBehaviour
{
    private NavMeshAgent navAgent;

    public AIWaypointNetwork waypointNetwork;
    public int currentWaypoint = 0;


    public bool hasPath = false;
    public bool pathPending = false;
    public bool pathStale = false;
    public NavMeshPathStatus pathStatus = NavMeshPathStatus.PathInvalid;
    public AnimationCurve jumpCurve; 

    void Start()
    {
        navAgent = GetComponent<NavMeshAgent>();

        if (waypointNetwork == null) return; // If no valid Waypoint Network has been assigned then return

        SetNextDestination(false);
    }


    void SetNextDestination(bool increment) //optionally increment current waypoint index and set next destination for agent
    {
        if (!waypointNetwork) return; // if no network, return

        int incStep = increment ? 1 : 0; // Calculate how much the current waypoint index needs to be incremented

        // Calculate index of next waypoint factoring in the increment with wrap-around and fetch waypoint 
        int nextWaypoint = (currentWaypoint + incStep >= waypointNetwork.waypoints.Count) ? 0 : currentWaypoint + incStep;
        Transform nextWaypointTransform = waypointNetwork.waypoints[nextWaypoint];

        // Assuming we have a valid waypoint transform
        if (nextWaypointTransform != null)
        {
            // Update the current waypoint index, assign its position as the NavMeshAgents
            // Destination and then return
            currentWaypoint = nextWaypoint;
            navAgent.destination = nextWaypointTransform.position;
            return;
        }

        // We did not find a valid waypoint in the list for this iteration
        currentWaypoint++;
    }

    void Update()
    {
        // Copy NavMeshAgents state into inspector visible variables
        hasPath = navAgent.hasPath;
        pathPending = navAgent.pathPending;
        pathStale = navAgent.isPathStale;
        pathStatus = navAgent.pathStatus;

        if(navAgent.isOnOffMeshLink)
        {
            StartCoroutine(Jump(1.0f));
            return; 
        }


        // If we don't have a path and one isn't pending then set the next
        // waypoint as the target, otherwise if path is stale regenerate path
        if ((!hasPath && !pathPending) || pathStatus == NavMeshPathStatus.PathInvalid)
        {
            SetNextDestination(true); 
        }
        else 
        if(navAgent.isPathStale)
        {
            SetNextDestination(false); 
        }
    }

    IEnumerator Jump(float duration)
    {
        OffMeshLinkData data = navAgent.currentOffMeshLinkData; // Get the current OffMeshLink data
        Vector3 startPosition = navAgent.transform.position; // Start Position is agent current position
        Vector3 endPosition = data.endPos + (navAgent.baseOffset * Vector3.up); // End position is fetched from OffMeshLink data and adjusted for baseoffset of agent
        float time = 0.0f; 
        while(time <= duration)
        {
            float t = time / duration;
            // Lerp between start position and end position and adjust height based on evaluation of t on Jump Curve
            navAgent.transform.position = Vector3.Lerp(startPosition, endPosition, t) + (jumpCurve.Evaluate(t) * Vector3.up);
            time += Time.deltaTime;
            yield return null; 
        }

        navAgent.CompleteOffMeshLink(); // All done so inform the agent it can resume control
    }
}
