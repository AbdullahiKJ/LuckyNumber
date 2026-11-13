using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Heal Player")]
public class HealEffect : EffectDefinition
{
    [SerializeField, Min(0f)] private float amount;

    public override bool Resolve(EffectContext context)
    {
        Player player = context.Player;

        if (player == null || !player.IsAlive ||
            player.CurrentHealth >= player.MaxHealth || amount <= 0f)
            return false;

        player.Heal(amount);
        return true;
    }
}