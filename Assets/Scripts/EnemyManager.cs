using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }

    public List<EnemyController> ActiveEnemies { get; } = new List<EnemyController>();

    public EnemyController enemyPrefab;

    // Assigned by GridManager once the base is spawned.
    [HideInInspector] public Transform baseTransform;
    [HideInInspector] public BaseCore baseCore;

    void Awake()
    {
        Instance = this;
    }

    public void SpawnEnemy(Vector3 worldPosition)
    {
        EnemyController enemy = Instantiate(enemyPrefab, worldPosition, Quaternion.identity);
        enemy.Init(baseTransform, baseCore);
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