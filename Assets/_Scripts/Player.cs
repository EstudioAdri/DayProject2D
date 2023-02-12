using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class Player : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float maxHeight;
    [SerializeField] float jumpAcceleration;
    [SerializeField] float tracingHeight = 1.1f;
    float originPositionJump;
    bool moving;
    bool goingUp;
    bool grounded;
    bool jumpBuffer;
    LayerMask ground;
    RaycastHit2D hit;
    direction playerDirection;
    playerState state;
    Vector3 nextPositionAir;
    enum direction
    {
        none,
        left,
        right
    }

    enum playerState
    {
        Idle,
        Moving,
        Jumping
    }

    void Awake()
    {
        goingUp = false;
        nextPositionAir = Vector3.zero;
        ground = LayerMask.GetMask("Ground");
    }


    void Update()
    {
        grounded = Physics2D.Raycast(transform.position, Vector2.down, tracingHeight, ground);
        CheckInputs();
    }

    void FixedUpdate()
    {
        PlayerState();
        Move(playerDirection);

    }

    void PlayerState()
    {
        //Checks if player is jumping from the ground or just falling of a ledge
        if (jumpBuffer && grounded)
        {
            originPositionJump = transform.position.y;
            goingUp = true;
            grounded = false;
            jumpBuffer = false;
        }

        switch (state)
        {
            case (playerState.Idle):
                if (!grounded)
                {
                    state = playerState.Jumping;
                }
                else if (moving)
                {
                    state = playerState.Moving;
                }
                break;
            case (playerState.Jumping):
                PlayerPhysics();
                if (grounded & moving)
                {
                    state = playerState.Moving;
                }
                else if (grounded)
                {
                    state = playerState.Idle;
                }
                break;
            case (playerState.Moving):
                if (!grounded)
                {
                    state = playerState.Jumping;

                }
                if (!moving)
                {
                    state = playerState.Idle;
                }
                break;
        }
    }

    void PlayerPhysics()
    {
        Vector2 hitPoint = new Vector2();
        if (transform.position.y >= originPositionJump + maxHeight)
        {
            goingUp = false;
        }

        if (transform.position.y >= originPositionJump)
        {
            nextPositionAir.y = 1 / (1 + (transform.position.y - originPositionJump));
        }
        else
        {
            nextPositionAir.y = 1;
        }

        if (!goingUp)
        {
            nextPositionAir *= -1;
        }

        hit = Physics2D.Raycast(transform.position, Vector2.down, 100f, ground);
        hitPoint = hit.point;


        if (transform.position.y + nextPositionAir.y * jumpAcceleration < hitPoint.y + 0.95f && !goingUp)
        {            
            transform.position = new Vector3(transform.position.x, hitPoint.y + 0.95f);
        }
        
        else
        {
            transform.position += nextPositionAir * jumpAcceleration;
        }
    }

    void Move(direction direction)
    {
        if (moving)
        {
            Vector2 force = Vector2.zero;
            Vector3 movement = Vector3.zero;
            if (direction == direction.right)
            {
                force = Vector2.right;
            }
            else if (direction == direction.left)
            {
                force = Vector2.left;
            }
            movement.x = force.x;
            transform.position += movement * playerSpeed;
        }
    }

    void CheckInputs()
    {
        //Checks how player is using the jump button
        //Features Incoming
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBuffer = true;
        }
        //Checks if player moves left or right
        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            playerDirection = direction.none;
            moving = false;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            playerDirection = direction.left;
            moving = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerDirection = direction.right;
            moving = true;
        }
        else
        {
            moving = false;
        }
        //Checks player state
        if (Input.GetKey(KeyCode.E))
        {
            //print("State: " + state);
            //print("Moving: " + moving);
            //print("Grounded: " + grounded);
            //print("buffer: " + jumpBuffer);
            //print("goingUP: " + goingUp);
            print("hitPoint: " + hit.point);
            //print("hitDistance:" + hit.distance);
        }
    }
}
