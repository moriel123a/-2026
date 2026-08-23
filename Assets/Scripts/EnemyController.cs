using UnityEngine;

/// <summary>
/// The class that controls the basic enemy.
/// Just walks in a straight line torwards the base if blocked by a building attack it.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 2f;
    public int maxHealth = 20;
    public int attackDamage = 5;
    public float attackCooldown = 1f;
    public float collisionRadius = 0.5f; // how close a building has to be to block this enemy and how close it has to be to attack the base

    private int currentHealth;
    private Building currentTargetBuilding; // A building that blocks movement.
    private float attackTimer;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        EnemyManager.Instance.Register(this);
        currentHealth = maxHealth;
    }

    void Update()
    {
        currentTargetBuilding = FindBlockingBuilding();
        if (currentTargetBuilding != null)
        {
            AttackBuilding();
        } else
        {
            TryAttackBase();
        }
    }

    private void FixedUpdate()
    {
        if (currentTargetBuilding == null)
        {
            MoveTowardBase();
        }
    }

    void MoveTowardBase()
    {
        if (GridManager.Instance.playerBase == null) return;

        rb.MovePosition(Vector3.MoveTowards(transform.position, GridManager.Instance.playerBase.transform.position, moveSpeed * Time.deltaTime));
    }

    void TryAttackBase() // Attack base if in range
    {
        if (GridManager.Instance.playerBase == null) return;

        if (Vector3.Distance(transform.position, GridManager.Instance.playerBase.transform.position) < collisionRadius)
        {
            AttackBase();
        }
    }

    /// <summary>
    /// Checks if blocked by a building.
    /// </summary>
    /// <returns>Blocking building. Returns null if not blocked.</returns>
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

    /// <summary>
    /// Attack the blocking building if can or lowers attack timer.
    /// </summary>
    void AttackBuilding()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            currentTargetBuilding?.TakeDamage(attackDamage);
        }
    }

    /// <summary>
    /// Like attack building will attack base if can or lower attack timer.
    /// </summary>
    void AttackBase()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            attackTimer = attackCooldown;
            GridManager.Instance.playerBase.TakeDamage(attackDamage);
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