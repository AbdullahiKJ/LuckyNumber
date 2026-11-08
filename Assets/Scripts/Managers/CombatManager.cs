using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    // private void [] cards
    [SerializeField] private Player player;
    [SerializeField] private Enemy[] enemies;

    public Player Player => player;
    public IReadOnlyList<Enemy> Enemies => enemies;

    public void PlayerAttack(int value, Enemy enemy)
    {
        // Damage calculation based on roll value and card effects
        player.Attack(enemy, value);
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