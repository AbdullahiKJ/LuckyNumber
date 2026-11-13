using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Heal Player")]
public class HealEffect : EffectDefinition
{
    [SerializeField, Min(0f)] private float amount;

    public override void Resolve(EffectContext context)
    {
        if (context.Player != null && context.Player.IsAlive)
            context.Player.Heal(amount);
    }
}