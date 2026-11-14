using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    [SerializeField] GameManager gameManager;
    [SerializeField] private Player player;
    [SerializeField] private Transform enemyContainer;
    [SerializeField] float playerTurnDelay = 2f;

    private readonly List<Enemy> enemies = new();

    public Player Player => player;
    public IReadOnlyList<Enemy> Enemies => enemies;
    public event Action EnemiesDefeated;

    private bool playerAttackInProgress;
    private bool waveReported;

    private void Awake()
    {
        player.Game = gameManager;
    }

    public void SpawnWave(WaveDefinition wave)
    {
        ClearEnemies();
        waveReported = false;

        foreach (EnemySpawn spawn in wave.enemies)
        {
            Enemy enemy = Instantiate(
                spawn.prefab,
                spawn.spawnPoint.position,
                spawn.spawnPoint.rotation,
                enemyContainer);

            enemy.Game = gameManager;
            enemy.HealthChanged += OnEnemyHealthChanged;
            enemy.gameObject.name = spawn.prefab.name;
            enemies.Add(enemy);
        }
    }

    public void PlayerAttack(int value, Enemy enemy)
    {
        if (!gameManager.Turns.IsPlayerTurn || !player.IsAlive ||
           enemy == null || !enemy.isActiveAndEnabled || !enemy.IsAlive)
            return;

        playerAttackInProgress = true;

        // Damage calculation based on roll value and card effects
        player.Attack(enemy, value);

        // The attack may have defeated the last enemy.
        if (!gameManager.IsGameActive())
            return;

        // End the turn
        Invoke(nameof(OnPlayerAttackFinished), playerTurnDelay);
    }

    public void OnPlayerAttackFinished()
    {
        if (!playerAttackInProgress)
            return;

        playerAttackInProgress = false;
        gameManager.Turns.EndPlayerTurn();
    }

    private void OnEnemyHealthChanged(float current, float maximum)
    {
        if (current > 0f || waveReported ||
            !gameManager.IsGameActive() || !CheckCombatEnd())
            return;

        waveReported = true;
        EnemiesDefeated?.Invoke();
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

    public void StopActions()
    {
        CancelInvoke();
        playerAttackInProgress = false;

        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
                enemy.CancelInvoke();
        }
    }

    private void ClearEnemies()
    {
        StopActions();

        foreach (Enemy enemy in enemies)
        {
            if (enemy == null)
                continue;

            enemy.HealthChanged -= OnEnemyHealthChanged;
            enemy.gameObject.SetActive(false);
            Destroy(enemy.gameObject);
        }

        enemies.Clear();
    }

    private void OnDestroy()
    {
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
                enemy.HealthChanged -= OnEnemyHealthChanged;
        }
    }
}