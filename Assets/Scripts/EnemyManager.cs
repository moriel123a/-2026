using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class responsible for creating and destroying all enemy instances.
/// Contains all active enemies, when an enemy is created or destroyed it should register here.
/// </summary>
public class EnemyManager : Singleton<EnemyManager>
{
    public List<EnemyController> ActiveEnemies { get; } = new List<EnemyController>();

    public EnemyController enemyPrefab;
    public EnemyController bossPrefab;

    public event Action<EnemyController> OnEnemyKilled;

    public void SpawnEnemy(Vector3 worldPosition)
    {
        EnemyController enemy = Instantiate(enemyPrefab, worldPosition, Quaternion.identity);
    }

    public void SpawnBoss(Vector3 worldPosition)
    {
        EnemyController boss = Instantiate(bossPrefab, worldPosition, Quaternion.identity);
    }

    public void Register(EnemyController enemy)
    {
        if (!ActiveEnemies.Contains(enemy))
            ActiveEnemies.Add(enemy);
    }

    public void Unregister(EnemyController enemy)
    {
        ActiveEnemies.Remove(enemy);
        OnEnemyKilled?.Invoke(enemy);
    }
}