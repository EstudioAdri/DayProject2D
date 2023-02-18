using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public class CameraControl : MonoBehaviour
{
    [SerializeField] float cameraBoundaryX;
    [SerializeField] float cameraBoundaryY;
    float cameraSpeed = .1f;
    Player player;


    private void Awake()
    {
        player = FindObjectOfType<Player>();
    }

    void FixedUpdate()
    {
        cameraSpeed = player.PlayerSpeed;
        Vector3 relativePosition = transform.InverseTransformPoint(player.transform.position);
        Vector3 cameraPosition = transform.position;
        if (relativePosition.x > cameraBoundaryX)
        {
            cameraPosition += Vector3.right;
        }
        else if (relativePosition.x < -cameraBoundaryX)
        {
            cameraPosition -= Vector3.right;
        }

        if (player.IsGrounded)
        {
            if (relativePosition.y > cameraBoundaryY)
            {
                cameraPosition += Vector3.up;
            }
            else if (relativePosition.y < -cameraBoundaryY)
            {
                cameraPosition -= Vector3.up;
            }
        }

        cameraPosition = Vector3.Lerp(transform.position, cameraPosition, cameraSpeed);
        transform.position = cameraPosition;
    }
}
