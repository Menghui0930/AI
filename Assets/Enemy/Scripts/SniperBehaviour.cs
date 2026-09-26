using UnityEngine;
using UnityEngine.AI;

public class SniperBehaviour : MonoBehaviour
{
    public Transform player;
    public WeaponAmmo Weapon;

    // Health
    public int maxHealth = 2;
    private int currentHealth;

    // Detection and attack
    public float detectRange = 10f;
    public float attackRange = 20f;

    // Flee
    public float fleeDistance = 10f;
    public float fleeSpeed = 6f;

    // Attack
    public float attackCooldown = 3f;
    private float attackTimer;

    private NavMeshAgent agent;

    void Start()
    {
        currentHealth = maxHealth;

        agent = GetComponent<NavMeshAgent>();
        if (player == null) {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) {
                player = playerObj.transform;
                Weapon = player.GetComponent<WeaponAmmo>();
                player = player.transform.parent.GetChild(2);
            }
        }

        agent.speed = fleeSpeed;
    }

    void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(transform.position, player.position);

        attackTimer -= Time.deltaTime;

        // Player is close
        if (distance <= detectRange)
        {
            FleeFromPlayer();
        }
        // Player is within attack range
        else if (distance <= attackRange)
        {
            StopAndAttack();
        }
        else
        {
            // Player is too far away
            agent.ResetPath();
        }

        // Face player when attacking
        if (distance > detectRange && distance <= attackRange)
            {
                Vector3 direction =
                    player.position - transform.position;

                direction.y = 0;

                if (direction != Vector3.zero)
                {
                    transform.rotation =
                        Quaternion.LookRotation(direction);
                }
            }
    }

    void FleeFromPlayer()
    {
        Vector3 direction =
            transform.position - player.position;

        direction.y = 0;

        direction.Normalize();

        Vector3 fleePosition =
            transform.position +
            direction * fleeDistance;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            fleePosition,
            out hit,
            fleeDistance,
            NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
      if (agent.velocity.sqrMagnitude > 0.1f)
        {
            Vector3 moveDirection = agent.velocity;
            moveDirection.y = 0;

            transform.rotation =
                Quaternion.LookRotation(moveDirection);
        }
    }

    void StopAndAttack()
    {
        agent.ResetPath();

        if (attackTimer <= 0)
        {
            AttackPlayer();

            attackTimer = attackCooldown;
        }
    }

    void AttackPlayer()
    {
        Debug.Log("Sniper attacks player!");

        PlayerController playerController =
            player.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.TakeDamage();
        }
    }

    public void TakeDamage()
    {
        currentHealth--;

        Debug.Log("Sniper Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Weapon.extraAmmo += 10;
        Destroy(gameObject);
    }
}