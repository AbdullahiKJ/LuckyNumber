using UnityEngine;

public abstract class EffectDefinition : ScriptableObject
{
    [SerializeField] private EffectTrigger trigger;
    public EffectTrigger Trigger => trigger;

    [SerializeField] private bool unlimitedUses;
    [SerializeField, Min(1)] private int charges = 1;

    public bool UnlimitedUses => unlimitedUses;
    public int Charges => charges;

    public abstract bool Resolve(EffectContext context);

    public virtual bool TryResolveDamage(DamageContext context)
    {
        return false;
    }
}

public enum EffectTrigger
{
    OnAcquire,
    TurnStarted,
    TurnEnded,
    AfterAttack,
    BeforeAttackDamage,
    BeforeIncomingDamage,
    BeforeDamageApplied,
    OnLethalDamage
}