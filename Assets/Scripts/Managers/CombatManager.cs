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
    }

    public void PlayerAttack(int value, Enemy enemy)
    {
        if (!turnManager.IsPlayerTurn || !player.IsAlive ||
           enemy == null || !enemy.isActiveAndEnabled || !enemy.IsAlive)
            return;

        // Damage calculation based on roll value and card effects
        player.Attack(enemy, value);

        // todo: resolve any card effects

        // End the turn
        turnManager.EndPlayerTurn();
    }

    public void EnemyAttack(Enemy enemy)
    {
        enemy.Attack(player);
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