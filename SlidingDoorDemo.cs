using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum DoorState { Open, Animating, Closed};
public class SlidingDoorDemo : MonoBehaviour
{
    public float slidingDistance = 3.5f;
    public float duration = 1.5f;
    public AnimationCurve doorCurve = new AnimationCurve();

    private Transform _transform = null;
    private Vector3 _openPosition = Vector3.zero;
    private Vector3 _closedPosition = Vector3.zero;
    private DoorState _doorState = DoorState.Closed; 

    void Start()
    {
        _transform = transform;
        _closedPosition = transform.position;
        _openPosition = _closedPosition + (slidingDistance * _transform.right); // Add right vector scaled by distance on to closed position to get the open position
    }

    void Update()
    {
        // If the space bar is pressed and the door is not already animating
        // then start the coroutine to animate it
        if (Input.GetKeyDown(KeyCode.Space) && _doorState != DoorState.Animating)
        {
            StartCoroutine(AnimateDoor((_doorState == DoorState.Open)?DoorState.Closed:DoorState.Open));
        }
    }

    IEnumerator AnimateDoor(DoorState newState)
    {
        _doorState = DoorState.Animating; // Block coroutine from starting again while it is still executing
        float time = 0.0f;

        // Choose the starting position and ending positions of the Lerp
        // based on the state we are moving into 
        Vector3 startPosition = (newState == DoorState.Open) ? _closedPosition : _openPosition;
        Vector3 endPosition = (newState == DoorState.Open) ? _openPosition : _closedPosition; 

        while(time <= duration)
        {
            // Calculate normalized time and evaluate the result on our animation curve.
            // The result of the curve evaluation is then used as the t value in the
            // Vector Lerp between the start and ending positions
            float t = time / duration;
            _transform.position = Vector3.Lerp(startPosition, endPosition, doorCurve.Evaluate(t));
            time += Time.deltaTime;
            yield return null; 
        }

        _transform.position = endPosition; // Snap object to the end position (just to make sure)
        _doorState = newState; // Assign new state to door
    }
}
