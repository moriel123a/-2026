using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : Singleton<EnemyManager>
{
    public List<EnemyController> ActiveEnemies { get; } = new List<EnemyController>();

    public EnemyController enemyPrefab;
    public EnemyController bossPrefab;

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
        if (ActiveEnemies.Count == 0 && GridManager.Instance.bossSpawned)
        {
            GameManager.Instance.GameWon();
        }
    }
}