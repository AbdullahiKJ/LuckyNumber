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

    private void Awake()
    {
        uiManager = GetComponent<UIManager>();
        combatManager = GetComponent<CombatManager>();
        diceManager = GetComponent<DiceManager>();
        gameManager = GetComponent<GameManager>();

        rollsRemaining = maxRolls;

        // todo: remove later
        BuildRoundQueue();
    }

    public void BuildRoundQueue()
    {
        turnQueue.Clear();
        turnQueue.AddLast((TurnState.PlayerTurn, null));

        foreach (Enemy enemy in combatManager.Enemies)
        {
            // todo: put this back later
            // if (enemy.isActiveAndEnabled)
            if (enemy != null)
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
        rollsRemaining = maxRolls;
        uiManager.UpdateRollsUI(rollsRemaining);
        uiManager.EnableRollButton();
        diceManager.ResetForPlayerTurn();
    }

    public void UseRoll()
    {
        if (IsPlayerTurn && rollsRemaining > 0 && diceManager.TryRollDice())
        {
            rollsRemaining--;
            uiManager.UpdateRollsUI(rollsRemaining);
        }
    }

    public void EndPlayerTurn()
    {
        if (IsPlayerTurn)
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

        // Turn ends via animation events
    }

    public void EndTurn()
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
        var rows = new List<(bool isPlayer, string label)>(turnQueue.Count);

        foreach (var (state, enemy) in turnQueue)
        {
            bool isPlayer = state == TurnState.PlayerTurn;
            string label = isPlayer ? "Player" : enemy != null ? enemy.name : "Enemy";
            rows.Add((isPlayer, label));
        }

        uiManager.UpdateTurnUI(rows);
    }
}
