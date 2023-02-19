using UnityEngine;
using static Enums;

public class PushableObject : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] float tracingWidth;
    [SerializeField] float margin = 2;
    [SerializeField] float checkBuffer = 0.5f;
    [SerializeField] bool wallRight;
    [SerializeField] bool wallLeft;

    float bufferTime = 0.017f;
    bool moving, playerCheck, selfCheck;
    Player player;
    Direction ownDirection;
    LayerMask groundLayer;

    #endregion

    #region UnityEvents

    void Start()
    {
        groundLayer = LayerMask.GetMask("Ground");
    }

    void FixedUpdate()
    {
        Walls();
        if (moving)
        {
            CheckDirection();
            if (playerCheck)
            {
                selfCheck = true;
            }
            if (selfCheck)
            {
                MoveSelf();
            }
        }
    }

    #endregion

    #region PrivateMethods

    void MoveSelf()
    {
        if (transform.position.y + 1 < player.transform.position.y)
        {
            return;
        }
        Vector3 movement = Vector3.zero;
        if (ownDirection == Direction.right)
        {
            movement = Vector3.right;
        }
        else if (ownDirection == Direction.left)
        {
            movement = Vector3.left;
        }
        transform.position += movement * player.PlayerSpeed;
        if (player.PlayerDirection != ownDirection || ownDirection == Direction.right && !player.WallRight || ownDirection == Direction.left && !player.WallLeft)
        {
            Invoke("CheckBuffer", checkBuffer);
        }
    }

    void CheckDirection()
    {
        bool check;
        if (player.WallLeft && transform.position.x < player.transform.position.x && player.PlayerDirection == Direction.left && !wallLeft)
        {
            ownDirection = Direction.left;
            check = true;
        }
        else if (player.WallRight && transform.position.x > player.transform.position.x && player.PlayerDirection == Direction.right && !wallRight)
        {
            ownDirection = Direction.right;
            check = true;
        }
        else
        {
            check = false;
        }

        playerCheck = check;
    }

    void Walls()
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.y + margin);
        if (Physics2D.BoxCast(origin, transform.lossyScale, 0, Vector2.right, tracingWidth, groundLayer))
        {
            wallRight = true;
        }
        else
        {
            wallRight = false;
        }
        if (Physics2D.BoxCast(origin, transform.lossyScale, 0, Vector2.left, tracingWidth, groundLayer))
        {
            wallLeft = true;
        }
        else
        {
            wallLeft = false;
        }
    }

    void MovingBuffer() // Invoked
    {
        moving = false;
        player = null;
    }

    void CheckBuffer() // Invoked
    {
        selfCheck = false;
    }

    #endregion

    #region Colliders & Triggers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != Tags.player.ToString())
            return;

        player = FindObjectOfType<Player>();
        moving = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag != Tags.player.ToString())
            return;

        Invoke("MovingBuffer", bufferTime);
    }

    #endregion
}

