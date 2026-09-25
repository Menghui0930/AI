using UnityEngine;

public class GarbageBehaviour : MonoBehaviour
{
    //逃跑的同时会留下攻击拖尾的AI
    [Header("Player")]
    public Transform player;

    [Header("Detection")]
    public float fleeRange = 8f;

    [Header("Movement")]
    public float wanderSpeed = 2f;
    public float fleeSpeed = 5f;

    [Header("Wander")]
    public float wanderChangeTime = 2f;

    [Header("Corrupted Data Trail")]
    public GameObject trailPrefab;
    public float trailInterval = 0.5f;

    private Vector3 wanderDirection;
    private float wanderTimer;
    private float trailTimer;

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObj != null)
            {
                player = playerObj.transform;

                Debug.Log(
                    "Garbage found Player: " +
                    player.name +
                    " at " +
                    player.position
                );
            }
        }

        ChooseNewWanderDirection();
    }

    void Update()
    {
        if (player == null)
            return;

        float distance =
            Vector3.Distance(
                transform.position,
                player.position
            );

        Debug.Log("Garbage Distance: " + distance);

        if (distance <= fleeRange)
        {
            Flee();
        }
        else
        {
            Wander();
        }
    }

    void Wander()
    {
        wanderTimer -= Time.deltaTime;

        if (wanderTimer <= 0f)
        {
            ChooseNewWanderDirection();
        }

        transform.position +=
            wanderDirection *
            wanderSpeed *
            Time.deltaTime;

        FaceDirection(wanderDirection);

        trailTimer = 0f;
    }

    void Flee()
    {
        // 从 Player 指向 Garbage
        Vector3 direction =
            transform.position -
            player.position;

        direction.y = 0f;

        if (direction.sqrMagnitude < 0.01f)
            return;

        direction.Normalize();

        transform.position +=
            direction *
            fleeSpeed *
            Time.deltaTime;

        FaceDirection(direction);

        trailTimer -= Time.deltaTime;

        if (trailTimer <= 0f)
        {
            CreateTrail();
            trailTimer = trailInterval;
        }
    }

    void ChooseNewWanderDirection()
    {
        Vector2 randomDirection =
            Random.insideUnitCircle.normalized;

        wanderDirection =
            new Vector3(
                randomDirection.x,
                0f,
                randomDirection.y
            );

        wanderTimer = wanderChangeTime;
    }

    void FaceDirection(Vector3 direction)
    {
        if (direction.sqrMagnitude < 0.01f)
            return;

        transform.rotation =
            Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(direction),
                10f * Time.deltaTime
            );
    }

    void CreateTrail()
    {
        if (trailPrefab == null)
            return;

        Instantiate(
            trailPrefab,
            transform.position,
            Quaternion.identity
        );
    }
}
