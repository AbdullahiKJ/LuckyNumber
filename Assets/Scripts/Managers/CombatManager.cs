using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    // private void [] cards
    [SerializeField] private Player player;
    [SerializeField] private Enemy[] enemies;
    [SerializeField] GameManager gameManager;
    [SerializeField] float playerTurnDelay = 2f;

    public Player Player => player;
    public IReadOnlyList<Enemy> Enemies => enemies;

    private bool playerAttackInProgress;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = GetComponent<GameManager>();

        // Assign the turn manager to each enemy script
        foreach (Enemy enemy in enemies)
            enemy.Turn = gameManager.Turns;
    }

    public void PlayerAttack(int value, Enemy enemy)
    {
        if (!gameManager.Turns.IsPlayerTurn || !player.IsAlive ||
           enemy == null || !enemy.isActiveAndEnabled || !enemy.IsAlive)
            return;

        // Damage calculation based on roll value and card effects
        player.Attack(enemy, value);

        // todo: resolve any card effects

        // End the turn
        Invoke("OnPlayerAttackFinished", playerTurnDelay);
    }

    public void OnPlayerAttackFinished()
    {
        if (!playerAttackInProgress)
            return;

        playerAttackInProgress = false;
        gameManager.Turns.EndPlayerTurn();
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