using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public List<EnemyController> ActiveEnemies { get; } = new List<EnemyController>();

    public EnemyController enemyPrefab;

    public void SpawnEnemy(Vector3 worldPosition)
    {
        EnemyController enemy = Instantiate(enemyPrefab, worldPosition, Quaternion.identity);
    }

    public void Register(EnemyController enemy)
    {
        if (!ActiveEnemies.Contains(enemy))
            ActiveEnemies.Add(enemy);
    }

    public void Unregister(EnemyController enemy)
    {
        ActiveEnemies.Remove(enemy);
    }
}