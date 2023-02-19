using UnityEngine;

public class Health : MonoBehaviour
{
    public uint HealthValue { get { return health; } }
    public uint MaxHealthValue { get { return maxHealth; } }

    [SerializeField] GameObject parent;
    [Space]
    [Tooltip("Player max health must be an Even number bigger than 2")]
    [SerializeField] uint maxHealth;
    [SerializeField] uint health;
    [Space]
    [SerializeField] bool evenMaxHealth;


    private void Update()
    {
        if (evenMaxHealth)
        {
            if (maxHealth % 2 != 0) // Checks the number of hearts is even
            {
                if (maxHealth != 1)
                    maxHealth++;
            }
        }
        if (health > maxHealth) // For Debug in Inspector
            health = maxHealth;

        if (health <= 0)
        {
            if (parent != null)
                Destroy(parent);
            else
                Destroy(gameObject);
        }
    }

    public void Damage(uint value)
    {
        health -= value;
    }

    public void Heal(uint value)
    {
        if (health + value <= maxHealth)
        {
            health += value;
        }
    }
}
