using UnityEngine;

public abstract class StatusDefinition : ScriptableObject
{
    [SerializeField, Min(1)] private int duration = 3;
    public Sprite icon;

    public int Duration => duration;

    public abstract void OnTurnStarted(Enemy target);
}