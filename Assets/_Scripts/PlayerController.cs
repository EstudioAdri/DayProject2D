using System.Collections.Generic;
using UnityEngine;
using static Enums;

[RequireComponent(typeof(Health))]

public class PlayerController : MonoBehaviour
{

    #region Parameters

    public bool IsOnProp { get { return isOnProp; } }

    #endregion

    #region PrivateVariables

    [SerializeField] float movementSpeed, jumpForce, bufferTime;
    [SerializeField] bool isGrounded, isOnProp, isTouchingLeftWall, isTouchingRightWall;
    [SerializeField] List<GameObject> nearestInteractable;
    [SerializeField] LayerMask groundLayerMask, propLayerMask;

    Rigidbody2D rb;

    #endregion

    #region UnityEvents

    void Awake()
    {
        gameObject.tag = Tags.player.ToString();
        groundLayerMask = LayerMask.GetMask("Ground");

        rb = gameObject.GetComponent<Rigidbody2D>();

        nearestInteractable = new();
    }

    private void Update()
    {
        InputAction();
    }

    void FixedUpdate()
    {
        CheckForGround(new Vector2(transform.localScale.x * .45f, 0.1f));
        CheckForWalls(new Vector2(.1f, 1.75f));
    }

    void CheckForGround(Vector2 boxCastSize)
    {
        RaycastHit2D hitGround = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, 1f, groundLayerMask);
        RaycastHit2D hitProp = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.down, 1f, propLayerMask);

        if (hitGround.collider != null)
            isGrounded = true;
        else
            isGrounded = false;
        
        if (hitProp.collider != null)
            isOnProp = true;
        else
            isOnProp = false;
    }

    void CheckForWalls(Vector2 boxCastSize)
    {
        RaycastHit2D hitLeft = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.left, .5f, groundLayerMask);
        RaycastHit2D hitRight = Physics2D.BoxCast(transform.position, boxCastSize, 0f, Vector2.right, .5f, groundLayerMask);

        if (hitLeft.collider != null)
            isTouchingLeftWall = true;
        else
            isTouchingLeftWall = false;

        if (hitRight.collider != null)
            isTouchingRightWall = true;
        else
            isTouchingRightWall = false;
    }

    #endregion

    #region PrivateMethods

    void InputAction()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isOnProp || isGrounded)
                Invoke("Jump", bufferTime);
        }

        if (Input.GetKey(KeyCode.A) && !isTouchingLeftWall)
        {
            rb.velocity = new Vector2(-movementSpeed, rb.velocity.y);
        }

        if (Input.GetKey(KeyCode.D) && !isTouchingRightWall)
        {
            rb.velocity = new Vector2(movementSpeed, rb.velocity.y);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            Interact();
        }
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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

    #endregion

    #region PublicMethods

    #endregion

    #region Colliders & Triggers

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == Tags.interactable.ToString())
        {
            if (col.GetComponent<Interactable>().IsInteractable && !nearestInteractable.Contains(col.gameObject))
                nearestInteractable.Add(col.gameObject);
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
