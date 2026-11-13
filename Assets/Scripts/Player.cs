using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Health
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0f;
    public event Action<float, float> HealthChanged;
    public event Action Died;


    // Attack and Defense
    private float attackMultiplier = 1f;
    private float defenseMultiplier = 1f;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void Attack(Enemy enemy, int value)
    {
        enemy.TakeDamage(value * attackMultiplier);
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
        HealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - (damage * defenseMultiplier));
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (!IsAlive)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Died?.Invoke();
    }
}
