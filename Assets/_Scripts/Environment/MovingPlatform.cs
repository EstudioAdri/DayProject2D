using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [SerializeField] Transform pointA, pointB;
    [SerializeField] Transform destination;

    [SerializeField] AnimationCurve curve;
    [Range(0.1f, 10f)][SerializeField] float speedMultiplier = 1f; // Default is 1, no multiplier
    
    float movementDuration;
    float currentMovementTime;

    private void Start()
    {
        SetVariables();
    }

    void FixedUpdate()
    {
        float speed = curve.Evaluate((Time.time - currentMovementTime) / movementDuration);
        transform.position = Vector3.Lerp(transform.position, destination.position, speed * Time.fixedDeltaTime * speedMultiplier);
    }

    /// <summary>
    /// Sets necessary variables for current time (used to read the value of curve within the animation) 
    /// and the duration of the animation (depends on the distance of pointA to pointB (divided by 2)
    /// </summary>
    void SetVariables()
    {
        currentMovementTime = Time.time;
        movementDuration = Vector3.Distance(pointA.position, pointB.position) / 2;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (destination == pointA)
        {
            SetVariables();
            destination = pointB;
        }
        else if (destination == pointB)
        {
            SetVariables();
            destination = pointA;
        }
    }
}
