using UnityEngine;
using UnityEngine.AI;

public class SniperBehaviour : MonoBehaviour
{
    // Health
    public int maxHealth = 2;
    private int currentHealth;

    // Bullet
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;

    private NavMeshAgent agent;

    void Start()
    {
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
    }

    public void MoveToPosition(Vector3 position)
    {
        if (agent == null)
            return;

        agent.isStopped = false;
        agent.SetDestination(position);
    }

    public void StopMoving()
    {
        if (agent == null)
            return;

        agent.isStopped = true;
        agent.ResetPath();
    }

    public void FacePlayer(Transform player)
    {
        if (player == null)
            return;

        Vector3 direction =
            player.position - transform.position;

        direction.y = 0;

        if (direction != Vector3.zero)
        {
            transform.rotation =
                Quaternion.LookRotation(direction);
        }
    }

    public void Shoot(Transform player)
    {
        if (player == null)
            return;

        if (bulletPrefab == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " has no Bullet Prefab!"
            );

            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " has no Fire Point!"
            );

            return;
        }

        Vector3 direction =
            (player.position - firePoint.position).normalized;

        Quaternion rotation =
            Quaternion.LookRotation(direction);

        GameObject bullet =
            Instantiate(
                bulletPrefab,
                firePoint.position,
                rotation
            );

        Rigidbody rb =
            bullet.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity =
                direction * bulletSpeed;
        }

        Debug.Log(
            gameObject.name +
            " shoots!"
        );
    }

    public void TakeDamage()
    {
        currentHealth--;

        Debug.Log(
            gameObject.name +
            " Health: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}