using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using static Enums;

[RequireComponent(typeof(Health))]

public class Player : MonoBehaviour
{
    [SerializeField] float playerSpeed;
    [SerializeField] float maxHeight;
    [SerializeField] float jumpAcceleration;
    [SerializeField] float jumpVariation;
    [SerializeField] float jumpArc;
    [SerializeField] float tracingHeight = 1.1f;
    [SerializeField] float tracingWidth;
    [SerializeField] float maxAirSpeed;
    [SerializeField] float bufferTime;

    public float PlayerSpeed { get { return playerSpeed; } }
    float originPositionJump;
    bool moving;
    bool goingUp;
    bool grounded;
    bool moveLock;
    bool wallRight;
    bool wallLeft;
    public bool WallRight { get { return wallRight; } }
    public bool WallLeft { get { return wallLeft; } }
    public bool IsGrounded { get { return grounded; } }
    bool jumpBuffer;
    LayerMask ground;
    RaycastHit2D hit;
    public direction PlayerDirection { get { return playerDirection; } }
    direction playerDirection;
    playerState state;
    public playerState State { get { return state; } }
    Vector3 nextPositionAir;
    Vector2 jumpDirection;
    public enum direction
    {
        none,
        left,
        right
    }

    public enum playerState
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
        originPositionJump = transform.position.y;
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
        Vector2 originDown = (Vector2)transform.position + Vector2.down * transform.lossyScale.y / 2;
        if (transform.position.y >= originPositionJump + maxHeight)
        {
            goingUp = false;
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
            Invoke("JumpBuffer", bufferTime);
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
        //Reset Scene
        if (Input.GetKey(KeyCode.R))
        {
            Application.LoadLevel(Application.loadedLevel);
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
        //bool floor = Physics2D.Raycast(transform.position, Vector2.down, tracingHeight, ground);
        if (floor && !goingUp)
        {
            grounded = true;
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

    void JumpBuffer()
    {
        jumpBuffer = false;
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

    #region Colliders & Triggers

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == Tags.interactable.ToString())
        {
            if (collider.GetComponent<Interactable>().IsInteractable)
                nearestInteractable.Add(collider.gameObject);
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == Tags.interactable.ToString())
        {
            nearestInteractable.Remove(collider.gameObject);
        }
    }

    #endregion

    #region Enums

    public enum direction
    {
        none,
        left,
        right
    }

    public enum playerState
    {
        Idle,
        Moving,
        Jumping
    }

    #endregion
}
