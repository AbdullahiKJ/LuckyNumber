using UnityEngine;

public abstract class EffectDefinition : ScriptableObject
{
    [SerializeField] private EffectTrigger trigger;

    public EffectTrigger Trigger => trigger;

    public abstract void Resolve(EffectContext context);
}

public enum EffectTrigger
{
    OnAcquire,
    TurnStarted,
    TurnEnded,
    AfterAttack,
    BeforeDamageApplied,
    OnLethalDamage
}