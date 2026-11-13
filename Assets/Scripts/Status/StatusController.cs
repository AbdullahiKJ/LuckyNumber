using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class StatusController : MonoBehaviour
{
    private sealed class ActiveStatus
    {
        public StatusDefinition Definition;
        public int RemainingTurns;

        public ActiveStatus(StatusDefinition definition)
        {
            Definition = definition;
            RemainingTurns = definition.Duration;
        }
    }

    private readonly List<ActiveStatus> statuses = new();
    private Enemy owner;

    public event Action StatusesChanged;

    private void Awake()
    {
        owner = GetComponent<Enemy>();
    }

    public void Apply(StatusDefinition definition)
    {
        if (definition == null || !owner.IsAlive)
            return;

        ActiveStatus existing = statuses.Find(
            status => status.Definition == definition);

        if (existing != null)
            existing.RemainingTurns = definition.Duration;
        else
            statuses.Add(new ActiveStatus(definition));

        StatusesChanged?.Invoke();
    }

    public void ResolveTurnStart()
    {
        // New statuses applied during resolution start next turn.
        foreach (ActiveStatus status in statuses.ToArray())
        {
            if (!owner.IsAlive)
                break;

            // Another status may have removed this one.
            if (!statuses.Contains(status))
                continue;

            status.RemainingTurns--;
            status.Definition.OnTurnStarted(owner);
        }

        statuses.RemoveAll(status => status.RemainingTurns <= 0);
        StatusesChanged?.Invoke();
    }

    public void Clear()
    {
        statuses.Clear();
        StatusesChanged?.Invoke();
    }

    public IReadOnlyList<(StatusDefinition definition, int turns)> GetStatuses()
    {
        var result = new List<(StatusDefinition, int)>();

        foreach (ActiveStatus status in statuses)
            result.Add((status.Definition, status.RemainingTurns));

        return result;
    }
}