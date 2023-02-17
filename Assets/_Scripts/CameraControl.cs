using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements.Experimental;

public class CameraControl : MonoBehaviour
{    
    [SerializeField] float smooth = 1f;
    [SerializeField] float smoothingBoundaryX;
    [SerializeField] float smoothingBoundaryY;
    Player player;
    bool boundaries;

    private void Start()
    {
        player = FindObjectOfType<Player>();        
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }
        Vector3 relativePosition = transform.InverseTransformDirection(player.transform.position - transform.position);
        Vector3 cameraPosition = transform.position;        

        if (relativePosition.x > smoothingBoundaryX)
        {
            cameraPosition += Vector3.right * player.PlayerSpeed;            
        }
        else if (relativePosition.x < -smoothingBoundaryX)
        {
            cameraPosition += Vector3.left * player.PlayerSpeed;
        }
        if (player.IsGrounded)
        {
            if (player.transform.position.y > cameraPosition.y + smoothingBoundaryY)
            {
                cameraPosition += Vector3.up * player.PlayerSpeed;
            }
            else if (player.transform.position.y < cameraPosition.y - smoothingBoundaryY)
            {
                cameraPosition += Vector3.down * player.PlayerSpeed;
            }
        }
        cameraPosition = Vector3.Lerp(transform.position, cameraPosition, smooth);
        transform.position = cameraPosition;
    }
}
