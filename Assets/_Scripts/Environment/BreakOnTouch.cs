using UnityEngine;
using static Enums;

[RequireComponent(typeof(Health))]

public class BreakOnTouch : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] bool damageOnLand, damageOnLeave;
    [SerializeField] float damageTimer;

    bool playerTouched, damageCD;
    PlatformState state;
    Health healthComponent;
    Player playerComponent;

    #endregion

    #region UnityEvents

    void Awake()
    {
        healthComponent = GetComponent<Health>();
        playerComponent = FindObjectOfType<Player>();
        state = PlatformState.Idle;
    }

    private void FixedUpdate()
    {
        if (playerTouched)
        {
            if (playerComponent.transform.position.y > transform.position.y)
            {
                CheckDamage();
            }
        }
    }

    #endregion

    #region Colliders & Triggers

    void OnTriggerEnter2D(Collider2D collision)
    {
        playerTouched = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerTouched = false;
    }

    #endregion

    #region PrivateMethods

    void CheckDamage()
    {
        if (!playerTouched)
            return;

        switch (state)
        {
            case PlatformState.Idle:
                if (playerComponent.IsGrounded)
                {
                    state = PlatformState.PlayerOnIt;
                }
                break;
            case PlatformState.PlayerOnIt:
                if (damageOnLand && !damageCD)
                {
                    Invoke("DamageSelf", damageTimer);
                    damageCD = true;
                }
                if (!playerComponent.IsGrounded)
                {
                    state = PlatformState.PlayerOffIt;
                    damageCD = false;
                }
                break;
            case PlatformState.PlayerOffIt:
                if (damageOnLeave && !damageCD)
                {
                    Invoke("DamageSelf", damageTimer);
                    damageCD = true;
                    state = PlatformState.Idle;
                }
                else
                {
                    state = PlatformState.Idle;
                }
                break;
        }
    }

    void DamageSelf() // Invoked
    {
        healthComponent.Damage(1);
    }

    #endregion
}
