using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Damage Single Target")]
public class SingleDamageEffect : EffectDefinition
{
    [SerializeField, Min(0f)] private float amount;
    public override bool Resolve(EffectContext context)
    {
        if (context.Targets != null)
        {
            // Choose a random target
            int index = Random.Range(0, context.Targets.Count);
            if (context.Targets[index] != null && context.Targets[index].IsAlive)
            {
                context.Targets[index].TakeDamage(amount);
                return true;
            }
        }
        return false;
    }
}