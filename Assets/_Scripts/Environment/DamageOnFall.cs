using UnityEngine;

[RequireComponent(typeof(DamageOnTouch))]
public class DamageOnFall : MonoBehaviour
{
    #region PrivateVariables

    bool grounded;
    uint originalDamage;
    LayerMask ground;
    DamageOnTouch damageOnTouch;

    #endregion

    #region UnityEvents

    void Start()
    {
        ground = LayerMask.GetMask("Ground");
        damageOnTouch = GetComponent<DamageOnTouch>();
        originalDamage = damageOnTouch.Damage;
    }

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

    #endregion
}
