using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Referencees
    [SerializeField] AnimationController animation;
    public GameManager Game;

    // Health
    [SerializeField] private float maxHealth = 100f;
    private float currentHealth;
    public float CurrentHealth => currentHealth;
    public float MaxHealth => maxHealth;
    public bool IsAlive => currentHealth > 0f;
    public event Action<float, float> HealthChanged;

    // Attack
    [SerializeField] private float attackDamage = 1f;
    Player target;

    public bool IsReactingToHit { get; private set; }

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void StartAttackAnimation(Player player = null)
    {
        target = player;
        animation.TriggerAttack();
    }

    public void Attack()
    {
        if (target == null)
            return;

        target.TakeDamage(attackDamage);
        target = null;
    }

    public void TakeDamage(float damage)
    {
        if (!IsAlive || damage <= 0f)
            return;

        currentHealth = Mathf.Max(0f, currentHealth - damage);
        HealthChanged?.Invoke(currentHealth, maxHealth);

        if (!IsAlive)
        {
            IsReactingToHit = false;
            Die();
            return;
        }

        // Play the hit animation
        IsReactingToHit = true;
        animation.TriggerHit();
    }

    private void Die()
    {
        // Play the death animation
        animation.TriggerDie();

        // todo: Handle death logic here
        Debug.Log($"{gameObject.name} has died.");
    }

    // Animation events for ending turns after playing attack, hit and die animations
    public void EndTurn()
    {
        Game.Turns.Invoke("EndTurn", Game.Turns.TurnDelay);
    }

    public void OnHitAnimationFinished()
    {
        IsReactingToHit = false;
    }
}
