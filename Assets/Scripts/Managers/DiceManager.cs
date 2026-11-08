using System;
using System.Collections.Generic;
using UnityEngine;

public class DiceManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> allDice;
    private readonly HashSet<Dice> pendingDice = new();
    private readonly HashSet<Dice> rolledDice = new();
    private readonly HashSet<Dice> selectedDice = new();

    public bool IsRolling => pendingDice.Count > 0;
    public event Action RollFinished;

    private void OnEnable()
    {
        foreach (GameObject dieObject in allDice)
        {
            if (dieObject != null && dieObject.TryGetComponent(out Dice die))
                die.RollCompleted += OnDieRollCompleted;
        }
    }

    private void OnDisable()
    {
        foreach (GameObject dieObject in allDice)
        {
            if (dieObject != null && dieObject.TryGetComponent(out Dice die))
                die.RollCompleted -= OnDieRollCompleted;
        }

        pendingDice.Clear();
        rolledDice.Clear();
        selectedDice.Clear();
    }

    public bool TryRollDice()
    {
        if (IsRolling)
            return false;

        // Add each die game object to the pending dice list
        foreach (GameObject dieObject in allDice)
        {
            if (dieObject != null &&
                 dieObject.TryGetComponent(out Dice die) &&
                 die.isActiveAndEnabled &&
                 !selectedDice.Contains(die))
            {
                pendingDice.Add(die);
            }
        }

        // Exit early if there are no pending die
        if (!IsRolling)
            return false;

        // Roll the die
        foreach (Dice die in pendingDice)
            die.Roll();

        return true;
    }

    private void OnDieRollCompleted(Dice die, int value)
    {
        // Exit early if the removal fails
        if (!pendingDice.Remove(die))
            return;

        rolledDice.Add(die);

        // Invoke the roll finished event once all die have been rolled
        if (!IsRolling)
            RollFinished?.Invoke();
    }

    public bool CanHold(Dice die) => !IsRolling && die != null && rolledDice.Contains(die);

    public void ToggleDieSelection(Dice die)
    {
        if (!CanHold(die))
            return;

        bool held = !selectedDice.Remove(die);

        if (held)
            selectedDice.Add(die);

        die.SetHeld(held);
    }
    public int GetSelectedTotal() => GetTotal(selectedDice);

    public int GetFullRollTotal() => GetTotal(rolledDice);

    private int GetTotal(HashSet<Dice> dice)
    {
        if (IsRolling)
            return 0;

        int total = 0;
        foreach (Dice die in dice)
            total += die.Result;

        return total;
    }

    public void ResetForPlayerTurn()
    {
        foreach (Dice die in selectedDice)
        {
            if (die != null)
                die.SetHeld(false);
        }

        selectedDice.Clear();
        rolledDice.Clear();
    }
}