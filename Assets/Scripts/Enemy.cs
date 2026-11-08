using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Health
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0f;
    public event Action<float, float> HealthChanged;


    // Attack
    [SerializeField] private float attackDamage = 1f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Attack(Player player)
    {
        player.TakeDamage(attackDamage);
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (!IsAlive)
            Die();
    }

    private void Die()
    {
        // todo: Handle death logic here
        Debug.Log($"{gameObject.name} has died.");
    }
}
