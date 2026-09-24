using UnityEngine;
using UnityEngine.AI;

public class CallDroneBehaviour : MonoBehaviour
{
    public Transform player;

    // Follow
    public float followDistance = 3f;
    public float moveSpeed = 5f;

    // Attack
    public float attackRange = 15f;
    public float attackCooldown = 2f;

    private float attackTimer;

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;

        // Attack range is 5 times follow distance
        attackRange = followDistance * 5f;

        // Find player automatically
        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    void Update()
    {
        if (player == null)
            return;

        attackTimer -= Time.deltaTime;

        // Find nearest enemy
        GameObject enemy = FindNearestEnemy();

        if (enemy != null)
        {
            float distance =
                Vector3.Distance(transform.position, enemy.transform.position);

            if (distance <= attackRange)
            {
                AttackEnemy(enemy);
            }
        }

        // Follow player
        float playerDistance =
            Vector3.Distance(transform.position, player.position);

        if (playerDistance > followDistance)
        {
            agent.SetDestination(player.position);
        }
        else
        {
            agent.ResetPath();
        }
    }

    GameObject FindNearestEnemy()
    {
        GameObject[] enemies =
            GameObject.FindGameObjectsWithTag("Enemy");

        GameObject nearestEnemy = null;

        float nearestDistance = attackRange;

        foreach (GameObject enemy in enemies)
        {
            float distance =
                Vector3.Distance(
                    transform.position,
                    enemy.transform.position
                );

            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    void AttackEnemy(GameObject enemy)
    {
        if (attackTimer > 0)
            return;

        Debug.Log("Drone attacks " + enemy.name);

        // Runner
        RunnerBehaviour runner =
            enemy.GetComponent<RunnerBehaviour>();

        if (runner != null)
        {
            runner.TakeDamage();
        }

        // Sniper
        SniperBehaviour sniper =
            enemy.GetComponent<SniperBehaviour>();

        if (sniper != null)
        {
            sniper.TakeDamage();
        }

        // Virus Injector
        VirusInjectorBehaviour injector =
            enemy.GetComponent<VirusInjectorBehaviour>();

        if (injector != null)
        {
            injector.TakeDamage();
        }

        attackTimer = attackCooldown;
    }
}