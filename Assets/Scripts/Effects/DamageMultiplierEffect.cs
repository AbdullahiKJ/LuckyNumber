using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Damage Multiplier")]
public class DamageMultiplierEffect : EffectDefinition
{
    [SerializeField, Min(0f)] private float multiplier = 1f;

    public override bool Resolve(EffectContext context)
    {
        // Damage effects use TryResolveDamage.
        return false;
    }

    public override bool TryResolveDamage(DamageContext context)
    {
        if (context.Amount <= 0f || multiplier == 1f)
            return false;

        context.Amount *= multiplier;
        return true;
    }
}