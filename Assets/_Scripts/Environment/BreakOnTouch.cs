using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Health))]
public class BreakOnTouch : MonoBehaviour
{
    [SerializeField] bool damageOnLand;
    [SerializeField] bool damageOnLeave;
    [SerializeField] float damageTimer;
    bool playerTouched;
    bool damageCD;
    platformState state;
    Health health;
    Player player;

    enum platformState
    {
        Idle,
        PlayerOnIt,
        PlayerOffIt
    }

    void Awake()
    {
        health = GetComponent<Health>();
        player = FindObjectOfType<Player>();
        state = platformState.Idle;
    }

    private void FixedUpdate()
    {
        if (playerTouched)
        {
            if (player.transform.position.y > transform.position.y)
            {
                CheckDamage();
            }
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        playerTouched = true;    
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        playerTouched = false;        
    }


    void CheckDamage()
    {
        if (playerTouched)
        {
            switch(state)
            {
                case platformState.Idle:
                    if (player.IsGrounded)
                    {
                        state = platformState.PlayerOnIt;
                    }
                    break;
                case platformState.PlayerOnIt:
                    if(damageOnLand && !damageCD) 
                    {
                        Invoke("DamageSelf", damageTimer);
                        damageCD = true;
                    }
                    if (!player.IsGrounded)
                    {
                        state = platformState.PlayerOffIt;
                        damageCD = false;
                    }
                    break;
                case platformState.PlayerOffIt:
                    if (damageOnLeave && !damageCD)
                    {
                        Invoke("DamageSelf", damageTimer);
                        damageCD = true;
                        state = platformState.Idle;
                    }
                    else
                    {
                        state = platformState.Idle;
                    }
                        break;
            }
        }
    }

    void DamageSelf()
    {
        health.Damage(1);
    }
}
