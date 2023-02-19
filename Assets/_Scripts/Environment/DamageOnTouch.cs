using System.Collections;
using UnityEngine;

public class DamageOnTouch : MonoBehaviour
{
    #region PrivateVariables

    [SerializeField] public uint Damage;
    [SerializeField] float damageRepeatDelay;

    bool stopRoutine;

    #endregion

    #region Enumerators

    IEnumerator DamageRoutine(GameObject who)
    {
        while (true)
        {
            if (stopRoutine) break;
            who.GetComponent<Health>().Damage(Damage);
            yield return new WaitForSeconds(damageRepeatDelay);
        }
    }

    #endregion

    #region Colliders & Triggers

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.GetComponent<Health>())
            return;

        stopRoutine = false;
        StartCoroutine(DamageRoutine(collision.gameObject));
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.GetComponent<Health>())
            return;

        stopRoutine = true;
        StopAllCoroutines();
    }

    #endregion
}
