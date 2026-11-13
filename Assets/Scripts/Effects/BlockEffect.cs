using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Block")]
public class BlockEffect : EffectDefinition
{
    public override bool Resolve(EffectContext context)
    {
        // Damage effects use TryResolveDamage.
        return false;
    }

    public override bool TryResolveDamage(DamageContext context)
    {
        if (context.Amount <= 0f)
            return false;

        context.Amount = 0f;
        return true;
    }
}