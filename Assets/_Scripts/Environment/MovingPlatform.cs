using UnityEngine;
using static Enums;

public class MovingPlatform : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] Transform pointA, pointB, destination;

    [SerializeField] AnimationCurve curve;
    [Range(0.1f, 10f)] [SerializeField] float speedMultiplier = 1f; // Default is 1, no multiplier

    float movementDuration, currentMovementTime;
    Transform previousParent;

    #endregion

    #region UnityEvents

    private void Start()
    {
        SetVariables();
    }

    void FixedUpdate()
    {
        float speed = curve.Evaluate((Time.time - currentMovementTime) / movementDuration);
        transform.position = Vector3.Lerp(transform.position, destination.position, speed * Time.fixedDeltaTime * speedMultiplier);
    }

    #endregion

    #region PrivateMethods

    /// <summary>
    /// Sets necessary variables for current time (used to read the value of curve within the animation) 
    /// and the duration of the animation (depends on the distance of pointA to pointB (divided by 2)
    /// </summary>
    void SetVariables()
    {
        currentMovementTime = Time.time;
        movementDuration = Vector3.Distance(pointA.position, pointB.position) / 2;
    }

    #endregion

    #region Collisions & Triggers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == Tags.player.ToString())
        {
            Transform collisionTransform = collision.gameObject.transform;
            if (collisionTransform.position.y > transform.position.y + 0.5f)
            {
                previousParent = collisionTransform.parent;
                collisionTransform.SetParent(gameObject.transform);
            }
        }

        if (collision.gameObject.tag != Tags.platformPoint.ToString())
            return;

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

    private void OnTriggerExit2D(Collider2D collision)
    {
        Transform collisionTransform = collision.gameObject.transform;
        if (collision.gameObject.tag == Tags.player.ToString() && collisionTransform.parent == gameObject.transform)
        {
            collisionTransform.SetParent(previousParent);
        }
    }

    #endregion
}
