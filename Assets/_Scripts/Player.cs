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
    [SerializeField] float jumpVariation;
    [SerializeField] float jumpArc;
    [SerializeField] float tracingHeight = 1.1f;
    [SerializeField] float tracingWidth;
    [SerializeField] float maxFallSpeed;
    [Space]
    [SerializeField] List<GameObject> nearestInteractable;

    public float PlayerSpeed { get { return playerSpeed; } }
    float originPositionJump;
    bool moving;
    bool goingUp;
    bool grounded;
    bool moveLock;
    bool wallRight;
    bool wallLeft;
    public bool IsGrounded { get { return grounded; } }
    bool jumpBuffer;
    LayerMask ground;
    RaycastHit2D hit;
    direction playerDirection;
    playerState state;
    Vector3 nextPositionAir;
    Vector2 jumpDirection;

    void Awake()
    {
        goingUp = false;
        nextPositionAir = Vector3.zero;
        ground = LayerMask.GetMask("Ground");
    }


    void Update()
    {
        
        Raycasts();
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
            moveLock = true;            
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
        nextPositionAir.y = 1;
        if (transform.position.y >= originPositionJump + maxHeight)
        {
            goingUp = false;
        }  

        if (transform.position.y - originPositionJump >= maxHeight * jumpArc)
        {
            nextPositionAir.y = 1 / ((transform.position.y - originPositionJump) * jumpVariation);
        }

        if (transform.position.y - originPositionJump >= maxHeight * jumpArc)
        {
            nextPositionAir.y = 1 / ((transform.position.y - originPositionJump) * jumpVariation);
        }

        if (!goingUp)
        {
            nextPositionAir *= -1;
        }

        hit = Physics2D.Raycast(transform.position, Vector2.down, 100f, ground);
        
        hitPoint = hit.point;

        nextPositionAir *= jumpAcceleration;
        if (nextPositionAir.y > maxFallSpeed)
        {
            nextPositionAir.y = maxFallSpeed;
        }
        else if(nextPositionAir.y < -maxFallSpeed)
        {
            nextPositionAir.y = -maxFallSpeed ;
        }

        if (transform.position.y + nextPositionAir.y < hitPoint.y + 0.95f && !goingUp)
        {            
            transform.position = new Vector3(transform.position.x, hitPoint.y + 0.95f);
        }        
        else
        {
            transform.position += nextPositionAir;
        }
    }

    void Move(direction direction)
    {
        if (moving)
        {
            Vector2 force = Vector2.zero;
            Vector3 movement = Vector3.zero;
            if (direction == direction.right && !wallRight)
            {
                force = Vector2.right;
            }
            else if (direction == direction.left && !wallLeft)
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
        if (!moveLock)
        {
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
        }
        //Checks player state
        if (Input.GetKey(KeyCode.E))
        {
            //print("State: " + state);
            //print("Moving: " + moving);
            //print("Grounded: " + grounded);
            //print("buffer: " + jumpBuffer);
            //print("goingUP: " + goingUp);
            print("sppeed: " + PlayerSpeed);
            //print("hitDistance:" + hit.distance);
        }
    }

    void Raycasts()
    {
        //This floor bool checks if the player is jumping, so it doesn't stop jumping when he goes under a platform
        bool floor = Physics2D.Raycast(transform.position, Vector2.down, tracingHeight, ground);
        if (floor && !goingUp)
        {
            grounded = true;
            moveLock = false;
        }
        else
        {
            grounded = false;
        }
        if (Physics2D.Raycast(transform.position, Vector2.up, tracingHeight, ground))
        {
            goingUp = false;
        }

        //BoxCast for wall collision
        Vector2 originRight = (Vector2)transform.position + Vector2.right * transform.lossyScale.x / 2;
        Vector2 originLeft = (Vector2)transform.position + Vector2.left * transform.lossyScale.x / 2;
        if (Physics2D.BoxCast(originRight, transform.lossyScale, 0, Vector2.right, tracingWidth, ground))
        {
            wallRight = true;
        }
        else
        {
            wallRight = false;
        }
        if (Physics2D.BoxCast(originLeft, transform.lossyScale, 0, Vector2.left, tracingWidth, ground))
        {
            wallLeft = true;
        }
        else
        {
            wallLeft = false;
        }
    }

    void Interact()
    {
        foreach (var item in nearestInteractable)
        {

        }
    }

    #region Colliders & Triggers

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (!collider.gameObject.GetComponent<Chest>().HasBeenInteracted)
        {
            Chest _chest = collider.gameObject.GetComponent<Chest>();
            
            if (!_chest.HasBeenInteracted)
                nearestInteractable.Add(_chest.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.gameObject.GetComponent<Chest>())
        {
            Chest _chest = collider.gameObject.GetComponent<Chest>();

            if (!_chest.HasBeenInteracted)
                nearestInteractable.Remove(_chest.gameObject);
        }
    }

    #endregion

    #region Enums

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

    #endregion
}
