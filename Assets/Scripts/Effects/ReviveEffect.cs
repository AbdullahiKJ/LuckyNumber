using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Revive Player")]
public class ReviveEffect : EffectDefinition
{
    public override void Resolve(EffectContext context)
    {
        if (context.Player != null && context.Player.CurrentHealth == 0)
            context.Player.Heal(context.Player.MaxHealth * 0.5f);
    }
}