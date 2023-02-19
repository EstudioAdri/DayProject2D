using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageOnTouch))]
public class DamageOnFall : MonoBehaviour
{
    bool grounded;
    uint originalDamage;
    LayerMask ground;
    DamageOnTouch damageOnTouch;
    // Start is called before the first frame update
    void Start()
    {
        Damage();
        ground = LayerMask.GetMask("Ground");
        damageOnTouch = GetComponent<DamageOnTouch>();
        originalDamage = damageOnTouch.Damage;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 originDown = (Vector2)transform.position + Vector2.down * transform.lossyScale.y / 2;
        grounded = Physics2D.BoxCast(originDown, transform.lossyScale, 0, Vector2.down, 0, ground);
        if (grounded)
        {
            damageOnTouch.Damage = originalDamage;
        }
        else
        {
            if (damageOnTouch.Damage == 0)
            {
                damageOnTouch.Damage = 1;
            }
            else
            {
                damageOnTouch.Damage = originalDamage * 2;
            }
        }
    }

    void Damage()
    {
        
    }
}
