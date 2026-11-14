using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    // References
    [SerializeField] private GameManager gameManager;

    // Roll and turn management
    private int rollsRemaining;
    [SerializeField] private int maxRolls = 2;
    [SerializeField] private float turnDelay = 2f;
    public float TurnDelay => turnDelay;
    public bool IsPlayerTurn => turnQueue.First?.Value.state == TurnState.PlayerTurn;
    private readonly LinkedList<(TurnState state, Enemy enemy)> turnQueue = new();

    enum TurnState
    {
        PlayerTurn,
        EnemyTurn
    }

    public event Action PlayerTurnStarted;
    public event Action PlayerTurnEnded;

    private void Awake()
    {
        if (gameManager == null)
            gameManager = GetComponent<GameManager>();

        rollsRemaining = maxRolls;

        // todo: remove later
        BuildRoundQueue();
    }

    public void BuildRoundQueue()
    {
        turnQueue.Clear();
        turnQueue.AddLast((TurnState.PlayerTurn, null));

        foreach (Enemy enemy in gameManager.Combat.Enemies)
        {
            if (enemy != null && enemy.isActiveAndEnabled && enemy.IsAlive)
            {
                turnQueue.AddLast((TurnState.EnemyTurn, enemy));
            }
        }

        RefreshTurnUI();
    }

    public void CheckNextTurn()
    {
        if (!gameManager.IsGameActive())
            return;

        if (turnQueue.Count > 0)
        {
            var (state, enemy) = turnQueue.First.Value;
            switch (state)
            {
                case TurnState.PlayerTurn:
                    StartPlayerTurn();
                    break;
                case TurnState.EnemyTurn:
                    ExecuteEnemyTurn();
                    break;
            }
        }
    }

    public void StartPlayerTurn()
    {
        PlayerTurnStarted?.Invoke();

        // Update remaining rolls and ui
        rollsRemaining = maxRolls;
        gameManager.UI.UpdateRollsUI(rollsRemaining);
        gameManager.UI.EnableRollButton();

        // Reset held die
        gameManager.Dice.ResetForPlayerTurn();

        // Resolve active card effects
        gameManager.Cards.ResolveEffects(EffectTrigger.TurnStarted, new EffectContext(gameManager.Combat.Player));
    }

    public void UseRoll()
    {
        if (!gameManager.IsGameActive())
            return;

        if (IsPlayerTurn && rollsRemaining > 0 && gameManager.Dice.TryRollDice())
        {
            rollsRemaining--;
            gameManager.UI.UpdateRollsUI(rollsRemaining);
        }
    }

    public void EndPlayerTurn()
    {
        if (IsPlayerTurn)
        {
            PlayerTurnEnded?.Invoke();
            EndTurn();
        }
    }

    public void GrantExtraPlayerTurn()
    {
        var current = turnQueue.First;
        if (current == null || current.Value.state != TurnState.PlayerTurn)
            return;

        turnQueue.AddAfter(current, (TurnState.PlayerTurn, null));
        RefreshTurnUI();
    }

    private void ExecuteEnemyTurn()
    {
        StartCoroutine(ExecuteEnemyTurnRoutine());
    }

    private IEnumerator ExecuteEnemyTurnRoutine()
    {
        Enemy enemy = turnQueue.First.Value.enemy;

        // Exit early if the enemy is dead or inactive
        if (enemy == null || !enemy.isActiveAndEnabled || !enemy.IsAlive)
        {
            EndTurn();
            yield break;
        }

        // Trigger any status effects
        enemy.GetComponent<StatusController>().ResolveTurnStart();

        if (!gameManager.IsGameActive())
            yield break;

        // Wait for the hit animation to end
        while (enemy != null && enemy.isActiveAndEnabled &&
                enemy.IsAlive && enemy.IsReactingToHit)
        {
            yield return null;
        }

        // Exit early if the game has ended
        if (!gameManager.IsGameActive())
            yield break;

        // Exit early if the enemy is dead or inactive
        if (enemy == null || !enemy.isActiveAndEnabled || !enemy.IsAlive)
        {
            EndTurn();
            yield break;
        }

        StatusController statuses = enemy.GetComponent<StatusController>();

        if (statuses.TryConsumeAttackSkip())
        {
            EndTurn();
            yield break;
        }

        gameManager.Combat.EnemyAttack(enemy);
    }

    public void EndTurn()
    {
        if (!gameManager.IsGameActive())
            return;

        if (turnQueue.First == null)
            return;

        turnQueue.RemoveFirst();

        // If combat ended, clear the list, refresh the UI, and return.
        if (gameManager.Combat.CheckCombatEnd())
        {
            turnQueue.Clear();
            RefreshTurnUI();
            return;
        }

        if (turnQueue.Count == 0)
            BuildRoundQueue(); // This already refreshes the UI.
        else
            RefreshTurnUI();

        CheckNextTurn();
    }

    private void RefreshTurnUI()
    {
        var rows = new List<(bool isPlayer, string label)>(turnQueue.Count);

        foreach (var (state, enemy) in turnQueue)
        {
            bool isPlayer = state == TurnState.PlayerTurn;
            string label = isPlayer ? "Player" : enemy != null ? enemy.name : "Enemy";
            rows.Add((isPlayer, label));
        }

        gameManager.UI.UpdateTurnUI(rows);
    }

    public void StopCombat()
    {
        StopAllCoroutines();
        CancelInvoke();

        turnQueue.Clear();
        RefreshTurnUI();
    }
}
