using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Effects/Apply Status")]
public class ApplyStatusEffect : EffectDefinition
{
    [SerializeField] private StatusDefinition status;

    public override void Resolve(EffectContext context)
    {
        if (context.Targets == null)
            return;

        foreach (Enemy target in context.Targets)
        {
            if (target != null &&
                target.TryGetComponent(out StatusController controller))
            {
                controller.Apply(status);
            }
        }
    }
}