using UnityEngine;

public class RunnerBehaviour : MonoBehaviour
{
    public Transform player;

    // Movement
    public float moveSpeed = 5f;
    public float stoppingDistance = 1f;

    // Runner Health
    public int maxHealth = 3;
    private int currentHealth;

    // Explosion
    public float explosionRadius = 3f;
    public GameObject explosionEffect;
    private UnityEngine.AI.NavMeshAgent agent;

    void Start()
    {
        currentHealth = maxHealth;
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
    }

    void Update()
    {
        {
        if (player == null)
            return;

        agent.SetDestination(player.position);
        }
    }

    // Runner takes damage
    public void TakeDamage()
    {
        currentHealth--;

        Debug.Log("Runner Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            DestroyRunner();
        }
    }

    // Runner touches player
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerController playerController =
                other.GetComponent<PlayerController>();

            if (playerController != null)
            {
                // Player loses 1 health
                playerController.TakeDamage();
            }

            // Runner explodes
            Explode();
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }

    void DestroyRunner()
    {
        if (explosionEffect != null)
        {
            Instantiate(
                explosionEffect,
                transform.position,
                Quaternion.identity
            );
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}