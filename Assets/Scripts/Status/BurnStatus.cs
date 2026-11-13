using UnityEngine;

[CreateAssetMenu(menuName = "Cards/Statuses/Burn")]
public class BurnStatus : StatusDefinition
{
    [SerializeField, Min(0f)] private float damage = 3f;

    public override void OnTurnStarted(Enemy target)
    {
        target.TakeDamage(damage);
    }
}