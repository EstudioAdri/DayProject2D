using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Enums;

public class BoxWalkable : MonoBehaviour
{
    Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Tags.player.ToString() && collision.gameObject.GetComponent<PlayerController>().IsOnProp)
        {
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == Tags.player.ToString())
        {
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints2D.None;
        }
    }
}
