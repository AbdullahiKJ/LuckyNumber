public class ActiveCardEffect
{
    public CardDefinition SourceCard { get; }
    public EffectDefinition Definition { get; }
    public int RemainingCharges { get; private set; }
    public bool IsExpired => !Definition.UnlimitedUses && RemainingCharges <= 0;

    public ActiveCardEffect(
     CardDefinition sourceCard,
     EffectDefinition definition)
    {
        SourceCard = sourceCard;
        Definition = definition;
        RemainingCharges = definition.Charges;
    }

    public void ConsumeCharge()
    {
        if (!Definition.UnlimitedUses && RemainingCharges > 0)
            RemainingCharges--;
    }
}