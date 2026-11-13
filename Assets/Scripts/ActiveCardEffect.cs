public class ActiveCardEffect
{
    public CardDefinition SourceCard { get; }
    public EffectDefinition Definition { get; }

    public ActiveCardEffect(
        CardDefinition sourceCard,
        EffectDefinition definition)
    {
        SourceCard = sourceCard;
        Definition = definition;
    }

    // Add remaining charges, duration, and stacks when needed.
}