using System;
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
        if (!IsAlive || enemy == null ||
            !enemy.isActiveAndEnabled || !enemy.IsAlive || value <= 0)
            return;

        var context = new DamageContext(this, value * attackMultiplier);

        Game.Cards.ResolveDamageEffects(
            EffectTrigger.BeforeAttackDamage,
            context);

        enemy.TakeDamage(context.Amount);
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

        var context = new DamageContext(
            this,
            Mathf.Max(0f, damage * defenseMultiplier));

        // Resolve any damage nullification effects
        Game.Cards.ResolveDamageEffects(
            EffectTrigger.BeforeIncomingDamage,
            context);

        // Resolve blocks.
        Game.Cards.ResolveDamageEffects(
            EffectTrigger.BeforeDamageApplied,
            context,
            stopAfterFirst: true);

        if (context.Amount <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - context.Amount);

        // Resolve any resurrection effects
        if (!IsAlive)
        {
            Game.Cards.ResolveDamageEffects(
            EffectTrigger.OnLethalDamage,
            context,
            stopAfterFirst: true);

            currentHealth = Mathf.Clamp(context.ReviveHealth, 0f, maxHealth);
        }

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
