using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Revive Player")]
public class ReviveEffect : EffectDefinition
{
    [SerializeField, Range(0.01f, 1f)]
    private float healthFraction = 0.5f;

    public override bool Resolve(EffectContext context)
    {
        // Damage effects use TryResolveDamage.
        return false;
    }

    public override bool TryResolveDamage(DamageContext context)
    {
        if (context.Player.IsAlive)
            return false;

        context.ReviveHealth = context.Player.MaxHealth * healthFraction;

        return true;
    }
}