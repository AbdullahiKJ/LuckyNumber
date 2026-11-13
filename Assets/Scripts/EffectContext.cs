
using System.Collections.Generic;

public class EffectContext
{
    public Player Player { get; }
    public IReadOnlyList<Enemy> Targets { get; }
    public int DamageDealt { get; }

    public EffectContext(
        Player player,
        IReadOnlyList<Enemy> targets = null,
        int damageDealt = 0)
    {
        Player = player;
        Targets = targets;
        DamageDealt = damageDealt;
    }
}