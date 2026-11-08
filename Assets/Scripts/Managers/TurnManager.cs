using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    // References
    private UIManager uiManager;
    private CombatManager combatManager;
    private DiceManager diceManager;
    private GameManager gameManager;

    // Roll and turn management
    private int rollsRemaining;
    private int maxRolls;
    bool isPlayerTurn;
    private readonly LinkedList<(TurnState state, Enemy enemy)> turnQueue = new();

    enum TurnState
    {
        PlayerTurn,
        EnemyTurn
    }

    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
        combatManager = GetComponent<CombatManager>();
        diceManager = GetComponent<DiceManager>();

        rollsRemaining = maxRolls;
    }

    public void BuildRoundQueue()
    {
        turnQueue.Clear();
        turnQueue.AddLast((TurnState.PlayerTurn, null));

        foreach (Enemy enemy in combatManager.Enemies)
        {
            if (enemy.isActiveAndEnabled)
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
                    isPlayerTurn = true;
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
        rollsRemaining = maxRolls;
    }

    private void UseRoll()
    {
        if (rollsRemaining > 0)
        {
            diceManager.RollDice();
            rollsRemaining--;
        }
    }

    private void EndPlayerTurn()
    {
        isPlayerTurn = false;
        EndTurn();
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
        Enemy enemy = turnQueue.First.Value.enemy;
        if (enemy != null && enemy.isActiveAndEnabled && enemy.IsAlive)
            combatManager.EnemyAttack(enemy);

        EndTurn();
    }

    private void EndTurn()
    {
        if (turnQueue.First == null)
            return;

        turnQueue.RemoveFirst();

        // If combat ended, clear the list, refresh the UI, and return.
        if (combatManager.CheckCombatEnd())
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
        var labels = new List<string>(turnQueue.Count);

        foreach (var (state, enemy) in turnQueue)
        {
            labels.Add(state == TurnState.PlayerTurn
                ? "Player"
                : enemy != null ? enemy.gameObject.name : "Enemy");
        }

        uiManager.UpdateTurnUI(labels);
    }
}
