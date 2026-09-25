using UnityEngine;
using UnityEngine.AI;

public class SniperGroupBehaviour : MonoBehaviour
{
    public GameObject sniperPrefab;
    public Transform player;
    public int sniperCount = 7;
    public float detectRange = 25f;
    public float spacing = 3f;
    public float rowSpacing = 3f;
    public float shootInterval = 1f;
    public float patrolRadius = 15f;
    public float patrolWaitTime = 3f;
    private SniperBehaviour[] snipers;
    private bool playerDetected;
    private int currentShooter;
    private float shootTimer;
    private float patrolTimer;
    private Vector3 patrolCenter;

    void Start()
    {
        patrolCenter = transform.position;

        SpawnSnipers();

        SetNewPatrolPosition();

        shootTimer = shootInterval;
    }

    void Update()
    {
        if (player == null)
            return;

        CheckPlayer();

        if (playerDetected)
        {
            CombatMode();
        }
        else
        {
            PatrolMode();
        }
    }

    void CheckPlayer()
    {
        float closestDistance = Mathf.Infinity;

        for (int i = 0; i < snipers.Length; i++)
        {
            if (snipers[i] == null)
                continue;

            float distance =
                Vector3.Distance(
                    snipers[i].transform.position,
                    player.position
                );

            if (distance < closestDistance)
            {
                closestDistance = distance;
            }
        }

        if (closestDistance <= detectRange)
        {
            if (!playerDetected)
            {
                Debug.Log("Sniper team detected player!");

                playerDetected = true;

                shootTimer = shootInterval;

                MoveIntoFormation();
            }
        }
        else
        {
            if (playerDetected)
            {
                Debug.Log(
                    "Player escaped sniper team!"
                );

                playerDetected = false;

                SetNewPatrolPosition();
            }
        }
    }

    // =========================
    // PATROL
    // =========================

    void PatrolMode()
    {
        patrolTimer -= Time.deltaTime;

        bool reachedDestination = true;

        for (int i = 0; i < snipers.Length; i++)
        {
            if (snipers[i] == null)
                continue;

            NavMeshAgent agent =
                snipers[i].GetComponent<NavMeshAgent>();

            if (agent == null)
                continue;

            if (agent.pathPending)
            {
                reachedDestination = false;
            }
            else if (agent.remainingDistance > 1f)
            {
                reachedDestination = false;
            }

            agent.isStopped = false;

            agent.SetDestination(
                GetPatrolPosition(i)
            );
        }

        if (reachedDestination || patrolTimer <= 0)
        {
            SetNewPatrolPosition();
        }
    }

    void SetNewPatrolPosition()
    {
        patrolTimer = patrolWaitTime;

        patrolCenter =
            transform.position +
            new Vector3(
                Random.Range(
                    -patrolRadius,
                    patrolRadius
                ),
                0,
                Random.Range(
                    -patrolRadius,
                    patrolRadius
                )
            );
    }

    Vector3 GetPatrolPosition(int index)
    {
        Vector3 position =
            patrolCenter;

        int row = index / 3;

        int column = index % 3;

        position +=
            transform.forward *
            (row * -rowSpacing);

        position +=
            transform.right *
            ((column - 1) * spacing);

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            position,
            out hit,
            5f,
            NavMesh.AllAreas))
        {
            return hit.position;
        }

        return transform.position;
    }

    // =========================
    // COMBAT
    // =========================

    void CombatMode()
    {
        bool formationReady = true;

        for (int i = 0; i < snipers.Length; i++)
        {
            if (snipers[i] == null)
                continue;

            Vector3 formationPosition =
                GetFormationPosition(i);

            NavMeshAgent agent =
                snipers[i].GetComponent<NavMeshAgent>();

            if (agent == null)
                continue;

            float distance =
                Vector3.Distance(
                    snipers[i].transform.position,
                    formationPosition
                );

            if (distance > 1f)
            {
                formationReady = false;

                agent.isStopped = false;

                agent.SetDestination(
                    formationPosition
                );
            }
            else
            {
                agent.isStopped = true;
            }

            // Always look at CURRENT player position
            snipers[i].FacePlayer(player);
        }

        if (!formationReady)
            return;

        // Shoot one sniper at a time
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0)
        {
            ShootNextSniper();

            shootTimer = shootInterval;
        }
    }

    void MoveIntoFormation()
    {
        for (int i = 0; i < snipers.Length; i++)
        {
            if (snipers[i] == null)
                continue;

            Vector3 position =
                GetFormationPosition(i);

            snipers[i].MoveToPosition(
                position
            );
        }
    }

    Vector3 GetFormationPosition(int index)
    {
        Vector3 position =
            transform.position;

        if (index == 0)
        {
            position +=
                transform.forward *
                rowSpacing;
        }
        else if (index == 1)
        {
            position +=
                transform.right *
                -spacing;
        }
        else if (index == 2)
        {
            position +=
                transform.right *
                spacing;
        }
        else if (index == 3)
        {
            position +=
                transform.forward *
                -rowSpacing;

            position +=
                transform.right *
                -spacing;
        }
        else if (index == 4)
        {
            position +=
                transform.forward *
                -rowSpacing;

            position +=
                transform.right *
                spacing;
        }
        else if (index == 5)
        {
            position +=
                transform.forward *
                -rowSpacing * 2;

            position +=
                transform.right *
                -spacing;
        }
        else if (index == 6)
        {
            position +=
                transform.forward *
                -rowSpacing * 2;

            position +=
                transform.right *
                spacing;
        }

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            position,
            out hit,
            5f,
            NavMesh.AllAreas))
        {
            return hit.position;
        }

        return position;
    }

    void ShootNextSniper()
    {
        for (int i = 0; i < snipers.Length; i++)
        {
            int index =
                (currentShooter + i) %
                snipers.Length;

            if (snipers[index] != null)
            {
                snipers[index].FacePlayer(
                    player
                );

                snipers[index].Shoot(
                    player
                );

                currentShooter =
                    (index + 1) %
                    snipers.Length;

                return;
            }
        }
    }
    void SpawnSnipers()
    {
        snipers = new SniperBehaviour[sniperCount];

        for (int i = 0; i < sniperCount; i++)
        {
            GameObject sniper =
                Instantiate(
                    sniperPrefab,
                    transform.position,
                    transform.rotation
                );

            SniperBehaviour sniperBehaviour =
                sniper.GetComponent<SniperBehaviour>();

            snipers[i] = sniperBehaviour;
        }
    }
}