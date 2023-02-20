using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class Player : MonoBehaviour
{

    #region Parameters

    public float PlayerSpeed { get { return playerSpeed; } }
    public bool WallRight { get { return wallRight; } }
    public bool WallLeft { get { return wallLeft; } }
    public bool IsGrounded { get { return grounded; } }
    public Direction PlayerDirection { get { return playerDirection; } }
    public PlayerState State { get { return state; } }

    #endregion

    #region PrivateVariables

    [SerializeField] uint wallJumpsNumber;
    [SerializeField] uint wallKicksNumber;
    [SerializeField] float playerSpeed;
    [SerializeField] float maxHeight;
    [SerializeField] float jumpAcceleration;
    [SerializeField] float jumpVariation;
    [SerializeField] float jumpArc;
    [SerializeField] float tracingHeight = 1.1f;
    [SerializeField] float tracingWidth;
    [SerializeField] float maxAirSpeed;
    [SerializeField] float bufferTime;
    [SerializeField] List<GameObject> nearestInteractable;


    uint ladderMove;
    uint wallJumps;
    uint wallKicks;
    float originPositionJump;
    bool ladder;
    bool moving, goingUp, grounded, moveLock, wallRight, wallLeft, jumpBuffer, wallJumpBuffer;

    SpriteRenderer spriteRenderer;
    Animator animator;
    LayerMask ground;
    RaycastHit2D hit;
    [SerializeField] Direction playerDirection;
    [SerializeField] PlayerState state; // TODO: remove serializefield, debugging
    Vector3 nextPositionAir;
    Vector3 previousDirection;

    #endregion

    #region UnityEvents

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.tag = Tags.player.ToString();
        nextPositionAir = Vector3.zero;
        ground = LayerMask.GetMask("Ground");
        originPositionJump = transform.position.y;
        nearestInteractable = new();
        ladderMove = 0;
        wallJumps = wallJumpsNumber;
        wallKicks = wallKicksNumber;
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
        SetPlayerAnimation();
        if (state == Enums.PlayerState.Jumping)
        {
            WallJump();
        }
    }

    #endregion

    #region PrivateMethods

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
            case (Enums.PlayerState.Idle):
                if (!grounded)
                {
                    state = Enums.PlayerState.Jumping;
                }
                else if (moving)
                {
                    state = Enums.PlayerState.Moving;
                }
                else if (ladder && ladderMove != 0)
                {
                    state = Enums.PlayerState.OnLadder;
                }
                break;
            case (Enums.PlayerState.Jumping):
                PlayerPhysics();
                if (grounded & moving)
                {
                    state = Enums.PlayerState.Moving;
                }
                else if (grounded)
                {
                    state = Enums.PlayerState.Idle;
                }
                else if (ladder && ladderMove != 0)
                {
                    state = Enums.PlayerState.OnLadder;
                }
                break;
            case (Enums.PlayerState.Moving):
                if (!grounded)
                {
                    state = Enums.PlayerState.Jumping;
                }
                if (!moving)
                {
                    state = Enums.PlayerState.Idle;
                }
                else if (ladder && ladderMove != 0)
                {
                    state = Enums.PlayerState.OnLadder;
                }
                break;
            case (Enums.PlayerState.OnLadder):
                LadderMovement();
                if (jumpBuffer == true)
                {
                    state = Enums.PlayerState.Jumping;
                    goingUp = true;
                    originPositionJump = transform.position.y;
                }
                if (ladderMove == 0)
                {
                    state = Enums.PlayerState.Idle;
                    if (moving)
                    {
                        state = Enums.PlayerState.Moving;
                    }
                    if (!grounded)
                    {
                        state = Enums.PlayerState.Jumping;
                    }
                }
                break;
        }
    }

    void PlayerPhysics()
    {
        Vector2 hitPoint = new Vector2();
        Vector2 originDown = (Vector2)transform.position + Vector2.down * transform.lossyScale.y / 2;
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


        if (transform.position.y - originPositionJump >= maxHeight * jumpArc)
        {
            nextPositionAir.y = 1 / ((transform.position.y - originPositionJump) * jumpVariation);
        }


        if (!goingUp)
        {
            nextPositionAir *= -1;
        }

        hit = Physics2D.BoxCast(originDown, transform.lossyScale, 0, Vector2.down, tracingHeight, ground);

        hitPoint = hit.point;

        nextPositionAir *= jumpAcceleration;
        if (nextPositionAir.y > maxAirSpeed)
        {
            nextPositionAir.y = maxAirSpeed;
        }
        else if (nextPositionAir.y < -maxAirSpeed)
        {
            nextPositionAir.y = -maxAirSpeed;
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

    void Move(Direction direction)
    {
        if (state == Enums.PlayerState.Jumping)
        {
            if (!WallRight && previousDirection.x > 0 || !WallLeft && previousDirection.x < 0)
            {
                transform.position += previousDirection * playerSpeed;   
            }
        }
        else if (moving && !moveLock)
        {
            Vector2 force = Vector2.zero;
            Vector3 movement = Vector3.zero;
            if (direction == Direction.right && !wallRight)
            {
                force = Vector2.right;
            }
            else if (direction == Direction.left && !wallLeft)
            {
                force = Vector2.left;
            }            
                movement.x = force.x;
            transform.position += movement * playerSpeed;
            previousDirection = movement;
        }
        else
        {
            previousDirection = Vector3.zero;
        }

    }
    void WallJump()
    {
        if (wallJumpBuffer == true && wallJumpsNumber > 0)
        {
            if (wallRight && playerDirection == Direction.right)
            {
                originPositionJump = transform.position.y;
                goingUp = true;
                wallJumpsNumber--;
                jumpBuffer = false;
                wallJumpBuffer = false;
                previousDirection = Vector3.right;
            }
            else if (wallLeft && playerDirection == Direction.left)
            {
                originPositionJump = transform.position.y;
                goingUp = true;
                wallJumpsNumber--;
                jumpBuffer = false;
                wallJumpBuffer = false;
                previousDirection = Vector3.left;
            }
        }
        if (wallJumpBuffer == true && wallKicksNumber > 0)
        {
            if (wallRight && playerDirection == Direction.left)
            {
                originPositionJump = transform.position.y;
                previousDirection.x *= -1;
                goingUp = true;
                wallKicksNumber--;
                jumpBuffer = false;
                wallJumpBuffer = false;
                previousDirection = Vector3.left;
            }
            else if (wallLeft && playerDirection == Direction.right)
            {
                originPositionJump = transform.position.y;
                goingUp = true;
                wallJumpsNumber--;
                jumpBuffer = false;
                wallJumpBuffer = false;
                previousDirection = Vector3.right;
            }
        }
    }

    void SetPlayerAnimation()
    {
        animator.SetBool("idle", false);
        animator.SetBool("run", false);
        animator.SetBool("jump", false);

        switch (state)
        {
            case Enums.PlayerState.Idle:
                animator.SetBool("idle", true);
                break;
            case Enums.PlayerState.Moving:
                animator.SetBool("run", true);
                break;
            case Enums.PlayerState.Jumping:
                animator.SetBool("jump", true);
                break;
            case Enums.PlayerState.OnLadder:
                // TODO: implement lol
                break;
            default:
                break;
        }
    }

    void CheckInputs()
    {
        //Checks how player is using the jump button        
        if (Input.GetKeyDown(KeyCode.Space))
        {            
            jumpBuffer = true;
            if (!grounded)
            {
                wallJumpBuffer = true;
            }
            Invoke("JumpBuffer", bufferTime);
        }
        //Checks if player moves left or right

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
        {
            playerDirection = Direction.none;
            moving = false;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            playerDirection = Direction.left;
            moving = true;
            spriteRenderer.flipX = true;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerDirection = Direction.right;
            moving = true;
            spriteRenderer.flipX = false;
        }
        else
        {
            moving = false;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        //Reset Scene
        if (Input.GetKey(KeyCode.R)) //TODO: move to GameManager
        {
            Application.LoadLevel(Application.loadedLevel);
        }
        if (ladder)
        {
            if (Input.GetKey(KeyCode.W))
            {
                ladderMove = 2;
            }
            else if (Input.GetKey(KeyCode.S))
            {
                ladderMove = 3;
            }
            else if (ladderMove != 0)
            {
                ladderMove = 1;
            }

        }
    }

    void Raycasts()
    {
        Vector2 originUp = (Vector2)transform.position + Vector2.up * transform.lossyScale.y / 2;
        Vector2 originDown = (Vector2)transform.position + Vector2.down * transform.lossyScale.y / 2;
        Vector2 originRight = (Vector2)transform.position + Vector2.right * transform.lossyScale.x / 2;
        Vector2 originLeft = (Vector2)transform.position + Vector2.left * transform.lossyScale.x / 2;
        //This floor bool checks if the player is jumping, so it doesn't stop jumping when he goes under a platform

        bool floor = Physics2D.BoxCast(originDown, transform.lossyScale, 0, Vector2.down, tracingHeight, ground);

        if (floor && !goingUp)
        {
            grounded = true;
            wallKicksNumber = wallKicks;
            wallJumpsNumber = wallJumps;
            moveLock = false;
        }
        else
        {
            grounded = false;
        }

        if (Physics2D.BoxCast(originUp, transform.lossyScale, 0, Vector2.up, tracingHeight, ground))
        {
            goingUp = false;
        }

        //BoxCast for wall collision
        if (Physics2D.BoxCast(transform.position, transform.lossyScale, 0, Vector2.right, tracingWidth, ground))
        {
            wallRight = true;
        }
        else
        {
            wallRight = false;
        }
        if (Physics2D.BoxCast(transform.position, transform.lossyScale, 0, Vector2.left, tracingWidth, ground))
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
        GameObject _interactableGameObject = FindNearestInteractable();

        if (_interactableGameObject == null)
            return;

        Interactable _interactable = _interactableGameObject.GetComponent<Interactable>();

        _interactable.Interact();

        if (!_interactable.IsInteractable)
            nearestInteractable.Remove(_interactableGameObject);
    }


    GameObject FindNearestInteractable()
    {
        if (nearestInteractable.Count <= 0)
            return null;

        Transform _nearestInteractable = null;

        foreach (var item in nearestInteractable)
        {
            Vector3 _itemPosition = item.transform.position;
            Vector3 _nearestPosition;
            if (_nearestInteractable == null)
                _nearestPosition = Vector3.positiveInfinity;
            else
                _nearestPosition = _nearestInteractable.position;

            float _distanceToItem = Vector3.Distance(transform.position, _itemPosition);
            float _distanceToNearest = Vector3.Distance(transform.position, _nearestPosition);

            if (_distanceToItem < _distanceToNearest && item.GetComponent<Interactable>().IsInteractable)
                _nearestInteractable = item.transform;
        }

        return _nearestInteractable.gameObject;
    }
    void LadderMovement()
    {
        Vector3 movement = new Vector3();
        if (ladderMove < 2)
        {
            return;
        }
        else if (ladderMove == 2)
        {
            movement = Vector3.up;
        }
        else if (ladderMove == 3 && grounded == false)
        {
            movement = Vector3.down;
        }
        transform.position += movement * playerSpeed;
    }

    #endregion

    #region PublicMethods

    public void JumpBuffer()
    {
        jumpBuffer = false;
        wallJumpBuffer = false;
    }

    #endregion

    #region Colliders & Triggers

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == Tags.interactable.ToString())
        {
            if (collider.GetComponent<Interactable>().IsInteractable && !nearestInteractable.Contains(collider.gameObject))
                nearestInteractable.Add(collider.gameObject);
        }

        if (collider.tag == Tags.ladder.ToString())
        {
            ladder = true;
            ladderMove = 0;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == Tags.interactable.ToString())
        {
            nearestInteractable.Remove(collider.gameObject);
        }

        if (collider.tag == Tags.ladder.ToString())
        {
            ladder = false;
            ladderMove = 0;
        }
    }


    #endregion
}
