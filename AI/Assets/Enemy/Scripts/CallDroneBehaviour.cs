using System.Collections;
using UnityEngine;

public class CallDroneBehaviour : MonoBehaviour {
    public Transform player;

    [Header("Follow")]
    public float followDistance = 6f;
    public float moveSpeed = 5f;
    public float hoverHeight = 1.5f;
    public float rotateSpeed = 10f;

    [Header("Wander (待机时在玩家附近闲晃)")]
    public float wanderRadius = 3f;
    public float wanderInterval = 2f;
    public float wanderReachDistance = 0.3f;
    public float wanderPauseDuration = 1f;   // 到达闲晃点后停留的时间

    [Header("Attack")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float attackRange = 15f;
    public float attackCooldown = 5f;
    public float aimDuration = 0.3f;
    public float postAttackPauseDuration = 1f; // 攻击完后停留的时间

    private float attackTimer;
    private bool isReturning = false;
    private bool isAttacking = false;

    private Vector3 wanderTarget;
    private float wanderTimer;
    private bool isPausing = false;
    private float pauseTimer;

    void Start() {
        if (player == null) {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
                player = playerObject.transform;
        }

        PickNewWanderTarget();
    }

    void Update() {
        if (player == null) return;

        attackTimer -= Time.deltaTime;

        float playerDistance = Vector3.Distance(transform.position, player.position);

        // 离玩家太远 —— 这条规则永远最优先，会打断暂停/闲晃状态
        if (playerDistance > followDistance) {
            isReturning = true;
            isPausing = false; // 强制取消暂停，立刻开始返回
        } else if (isReturning && playerDistance <= followDistance * 0.5f) {
            isReturning = false;
            PickNewWanderTarget();
        }

        if (isAttacking) {
            return;
        }

        if (isReturning) {
            MoveTowardsPlayer();
            return;
        }

        // 正在暂停/休息中，不做任何移动，只倒数计时
        if (isPausing) {
            pauseTimer -= Time.deltaTime;
            if (pauseTimer <= 0f) {
                isPausing = false;
                PickNewWanderTarget();
            }
            return;
        }

        GameObject enemy = FindNearestEnemy();

        if (enemy != null && attackTimer <= 0f) {
            StartCoroutine(AttackSequence(enemy));
            return;
        }

        Wander();
    }

    void MoveTowardsPlayer() {
        Vector3 targetPos = player.position + Vector3.up * hoverHeight;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
        FaceDirection(player.position - transform.position);
    }

    void Wander() {
        wanderTimer -= Time.deltaTime;

        float distanceToTarget = Vector3.Distance(transform.position, wanderTarget);

        if (distanceToTarget <= wanderReachDistance || wanderTimer <= 0f) {
            // 到达目标点（或超时），先进入暂停状态，而不是立刻选下一个点
            isPausing = true;
            pauseTimer = wanderPauseDuration;
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, wanderTarget, moveSpeed * Time.deltaTime);

        Vector3 moveDir = wanderTarget - transform.position;
        if (moveDir.sqrMagnitude > 0.01f) {
            FaceDirection(moveDir);
        }
    }

    void PickNewWanderTarget() {
        Vector2 randomCircle = Random.insideUnitCircle * wanderRadius;
        Vector3 randomOffset = new Vector3(randomCircle.x, 0f, randomCircle.y);

        wanderTarget = player.position + randomOffset + Vector3.up * hoverHeight;
        wanderTimer = wanderInterval;
    }

    void FaceDirection(Vector3 direction) {
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f) {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }

    GameObject FindNearestEnemy() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject nearestEnemy = null;
        float nearestDistance = attackRange;

        foreach (GameObject enemy in enemies) {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            if (distance < nearestDistance) {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    IEnumerator AttackSequence(GameObject enemy) {
        isAttacking = true;
        float elapsed = 0f;

        while (elapsed < aimDuration) {
            if (enemy == null) {
                isAttacking = false;
                yield break;
            }

            Vector3 dir = enemy.transform.position - transform.position;
            FaceDirection(dir);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Fire(enemy);

        attackTimer = attackCooldown;

        // 攻击完后休息一下，再回到 Wander 状态
        isAttacking = false;
        isPausing = true;
        pauseTimer = postAttackPauseDuration;
    }

    void Fire(GameObject enemy) {
        if (enemy == null || bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        Vector3 direction = (enemy.transform.position - firePoint.position).normalized;
        bullet.transform.rotation = Quaternion.LookRotation(direction);
        Rigidbody theRB = bullet.GetComponent<Rigidbody>();
        theRB.AddForce(direction * 100, ForceMode.Impulse);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        if (player != null)
            Gizmos.DrawWireSphere(player.position, followDistance);
        else
            Gizmos.DrawWireSphere(transform.position, followDistance);

        Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
        if (player != null)
            Gizmos.DrawWireSphere(player.position, followDistance * 0.5f);

        Gizmos.color = Color.green;
        if (player != null)
            Gizmos.DrawWireSphere(player.position, wanderRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(wanderTarget, 0.2f);

        if (firePoint != null) {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(firePoint.position, firePoint.position + firePoint.forward * 2f);
        }
    }
}