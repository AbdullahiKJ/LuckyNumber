using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Statuses/Sleep")]
public class SleepStatus : StatusDefinition
{
    public override bool SkipsAttack => true;

    public override void OnTurnStarted(Enemy target)
    {
        // Its duration is consumed when an attack is skipped.
    }
}