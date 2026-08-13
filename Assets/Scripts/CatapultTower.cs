using UnityEngine;

public class CatapultTower : Building
{
    [Header("Attack")]
    public float attackRange = 3f;
    public int attackDamage = 8;
    public float attackCooldown = 1.2f;

    private float attackTimer;

    void Update()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;

        EnemyController target = FindNearestEnemyInRange();
        if (target != null)
        {
            target.TakeDamage(attackDamage);
            attackTimer = attackCooldown;
        }
    }

    EnemyController FindNearestEnemyInRange()
    {
        EnemyController closest = null;
        float closestDist = attackRange;

        foreach (var enemy in EnemyManager.Instance.ActiveEnemies)
        {
            if (enemy == null) continue;
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist <= closestDist)
            {
                closest = enemy;
                closestDist = dist;
            }
        }

        return closest;
    }
}