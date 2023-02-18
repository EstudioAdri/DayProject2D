using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushableObject : MonoBehaviour
{
    [SerializeField] float tracingWidth;
    [SerializeField] float margin = 2;
    [SerializeField] float checkBuffer = 0.5f;
    float bufferTime = 0.017f;
    Player player;
    Player.direction ownDirection;
    bool moving;
    bool playerCheck;
    bool selfCheck;
    [SerializeField] bool wallRight;
    [SerializeField] bool wallLeft;
    LayerMask ground;
    // Start is called before the first frame update
    void Start()
    {
        ground = LayerMask.GetMask("Ground");
    }

    // Update is called once per frame
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

    void MoveSelf()
    {
        Vector3 movement = Vector3.zero;
        if (ownDirection == Player.direction.right)
        {
            movement = Vector3.right;
        }
        else if (ownDirection == Player.direction.left)
        {
            movement = Vector3.left;
        }
        transform.position += movement * player.PlayerSpeed;
        if (player.PlayerDirection != ownDirection || ownDirection == Player.direction.right && !player.WallRight || ownDirection == Player.direction.left && !player.WallLeft)
        {
            Invoke("CheckBuffer", checkBuffer);
        }
    }

    void CheckDirection()
    {
        bool check;
        Vector2 position = transform.position;
        Vector3 force = Vector3.zero;
        if (player.WallLeft && transform.position.x < player.transform.position.x && player.PlayerDirection == Player.direction.left && !wallLeft)
        {
            ownDirection = Player.direction.left;
            check = true;
        }
        else if (player.WallRight && transform.position.x > player.transform.position.x && player.PlayerDirection == Player.direction.right && !wallRight)
        {
            ownDirection = Player.direction.right;
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
        if (Physics2D.BoxCast(origin, transform.lossyScale, 0, Vector2.right, tracingWidth, ground))
        {
            wallRight = true;
        }
        else
        {
            wallRight = false;
        }
        if (Physics2D.BoxCast(origin, transform.lossyScale, 0, Vector2.left, tracingWidth, ground))
        {
            wallLeft = true;
        }
        else
        {
            wallLeft = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        player = FindObjectOfType<Player>();
        moving = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Invoke("MovingBuffer", bufferTime);

    }
    void MovingBuffer()
    {
        moving = false;
        player = null;
    }

    void CheckBuffer()
    {
        selfCheck = false;
    }
}

