using System;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    // References
    public GameManager Game;

    // Health
    [SerializeField] private float maxHealth = 100f;

    [SerializeField] private bool debug = false;
    [SerializeField] private float debugHealth = 0f;

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
        if (debug)
        {
            currentHealth = debugHealth;
            return;
        }
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

        // todo: Resolve attack block cards and consume charges
        // if(Game.Cards.OwnedCards.Contains())
        // return;

        currentHealth = Mathf.Max(0f, currentHealth - (damage * defenseMultiplier));
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (!IsAlive)
        {
            // todo: Resolve ressurection cards
            // if(Game.Cards.OwnedCards.Contains())
            // HealthChanged?.Invoke(currentHealth, maxHealth);
            // return;
            Die();
        }
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        Died?.Invoke();
    }
}
