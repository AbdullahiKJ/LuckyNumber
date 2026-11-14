using System;
using UnityEngine;

[Serializable]
public class EnemySpawn
{
    public Enemy prefab;
    public Transform spawnPoint;
}

[Serializable]
public class WaveDefinition
{
    public EnemySpawn[] enemies;
}