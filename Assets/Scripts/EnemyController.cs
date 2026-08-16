using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public int maxHealth = 20;
    public int attackDamage = 5;
    public float attackCooldown = 1f;
    public float collisionRadius = 0.5f; // how close a building has to be to block this enemy

    private int currentHealth;
    private Building currentTargetBuilding;
    private float attackTimer;

    void Update()
    {
        if (currentTargetBuilding != null)
        {
            AttackBuilding();
            return;
        }

        Building blocker = FindBlockingBuilding();
        if (blocker != null)
        {
            currentTargetBuilding = blocker;
            return;
        }

        MoveTowardBase();
    }

    void MoveTowardBase()
    {
        if (BaseCore.Instance == null) return;

        Vector3 basePosition = BaseCore.Instance.transform.position;

        transform.position = Vector3.MoveTowards(transform.position, basePosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, basePosition) < 0.2f)
        {
            BaseCore.Instance.TakeDamage(attackDamage);
            Die();
        }
    }

    Building FindBlockingBuilding()
    {
        Building closest = null;
        float closestDist = collisionRadius;

        foreach (var b in BuildManager.Instance.ActiveBuildings)
        {
            if (b == null) continue;
            float dist = Vector3.Distance(transform.position, b.transform.position);
            if (dist < closestDist)
            {
                closest = b;
                closestDist = dist;
            }
        }

        return closest;
    }

    void AttackBuilding()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            currentTargetBuilding.TakeDamage(attackDamage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        EnemyManager.Instance.Unregister(this);
        Destroy(gameObject);
    }
}