using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float timeToLive;
    [SerializeField] private float speed;
    [SerializeField] private int damage;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        timeToLive -= Time.deltaTime;

        if (timeToLive <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + (Vector2)transform.right * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision.gameObject.TryGetComponent(out EnemyController enemy))
            {
                HitEnemy(enemy);
            }
        }
    }

    private void HitEnemy(EnemyController enemy)
    {
        enemy.TakeDamage(damage);
        Destroy(gameObject);
    }
}
