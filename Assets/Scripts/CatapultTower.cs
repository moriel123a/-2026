using UnityEngine;

/// <summary>
/// A normal tower that shoots at the closest enemy in range.
/// </summary>
public class CatapultTower : Building
{
    [Header("Attack")]
    public float attackRange = 3f;
    public int attackDamage = 8;
    public float attackCooldown = 1.2f;

    [SerializeField] private Projectile projectilePrefab;

    [Header("Aiming")]
    [Tooltip("Degrees per second the turret rotates to face its target.")]
    public float rotationSpeed = 360f;
    [Tooltip("Adjust if the turret sprite's default facing isn't to the right (0 = right, -90 = up, 90 = down).")]
    public float rotationOffset = 0f;
    [Tooltip("How close to dead-on (in degrees) the turret needs to be aimed before it's allowed to fire.")]
    public float aimTolerance = 5f;

    private float attackTimer;
    private EnemyController currentTarget;

    void Update()
    {
        currentTarget = FindNearestEnemyInRange();
        if (currentTarget == null) return;

        RotateTowardTarget();

        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f && IsAimedAtTarget())
        {
            Fire();
        }
    }

    void RotateTowardTarget()
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, GetTowerAngle());
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }

    bool IsAimedAtTarget()
    {
        float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, GetTowerAngle()));
        return angleDifference <= aimTolerance;
    }

    float GetTargetAngle()
    {
        Vector3 direction = currentTarget.transform.position - transform.position;
        return Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    }

    // The angle the tower needs to get to to aim at the target takes tower offset into consideration.
    float GetTowerAngle()
    {
        return GetTargetAngle() + rotationOffset;
    }

    void Fire()
    {
        float angle = GetTargetAngle();

        Quaternion projectileRotation = Quaternion.Euler(0f, 0f, angle);

        Instantiate(projectilePrefab, transform.position, projectileRotation);
        attackTimer = attackCooldown;
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