using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    // private void [] cards
    [SerializeField] private Player player;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] TurnManager turnManager;

    public Player Player => player;
    public IReadOnlyList<Enemy> Enemies => enemies;

    private void Awake()
    {
        if (turnManager == null)
            turnManager = GetComponent<TurnManager>();

        // Assign the turn manager to each enemy script
        foreach (Enemy enemy in enemies)
            enemy.Turn = turnManager;
    }

    public void PlayerAttack(int value, Enemy enemy)
    {
        if (!turnManager.IsPlayerTurn || !player.IsAlive ||
           enemy == null || !enemy.isActiveAndEnabled || !enemy.IsAlive)
            return;

        // Damage calculation based on roll value and card effects
        player.Attack(enemy, value);

        // todo: resolve any card effects

        // End the turn via animation events
    }

    public void EnemyAttack(Enemy enemy)
    {
        if (player != null && player.IsAlive)
            enemy.StartAttackAnimation(player);
    }

    public bool CheckCombatEnd()
    {
        // Return false if any enemies are alive
        foreach (Enemy enemy in enemies)
        {
            if (enemy.IsAlive)
                return false;
        }

        return true;
    }

}