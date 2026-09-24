using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyChaser : MonoBehaviour {
    public Transform player;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float updateInterval = 0.2f;
    public float attackRange = 2f;

    [Header("Attack VFX")]
    public GameObject windupVfxPrefab;
    public GameObject explosionVfxPrefab;
    public float windupDuration = 2f;

    [Header("Damage (可选)")]
    public float explosionRadius = 3f;
    public int damage = 1;
    public LayerMask playerLayer;

    EnemyHealth health;

    NavMeshAgent agent;
    float timer;

    GameObject windupVfxInstance; // 记录目前正在播放的前摇特效
    Coroutine attackCoroutine;

    enum State { Chasing, Attacking, Dead }
    State state = State.Chasing;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = attackRange;

        health = GetComponent<EnemyHealth>();
        health.OnDeath += InterruptAndDie; // 血量归零时自动中断攻击并死亡

        if (player == null) {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update() {
        if (player == null || state == State.Dead) return;

        switch (state) {
            case State.Chasing:
                HandleChasing();
                break;

            case State.Attacking:
                FacePlayer();
                break;
        }
    }

    void HandleChasing() {
        timer -= Time.deltaTime;
        if (timer <= 0f) {
            agent.SetDestination(player.position);
            timer = updateInterval;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            EnterAttackState();
        }
    }

    void EnterAttackState() {
        state = State.Attacking;
        agent.isStopped = true; // 攻击(引爆)期间不移动

        attackCoroutine = StartCoroutine(AttackSequence());
    }

    IEnumerator AttackSequence() {
        if (windupVfxPrefab != null) {
            // 建议直接挂在 Enemy 底下当子物体，这样Enemy被销毁时会自动跟着消失
            windupVfxInstance = Instantiate(windupVfxPrefab, transform.position, Quaternion.identity, transform);
        }

        yield return new WaitForSeconds(windupDuration);

        Explode();
    }

    void Explode() {
        // 引爆前先清掉前摇特效
        ClearWindupVfx();

        if (explosionVfxPrefab != null) {
            // 爆炸特效不挂在 Enemy 底下，
            // 这样 Enemy 消失后爆炸特效还能自己播完（不会被 Destroy(gameObject) 连带清掉）
            Instantiate(explosionVfxPrefab, transform.position, Quaternion.identity);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius, playerLayer);
        foreach (var hit in hits) {
            PlayerHealth ph = hit.GetComponent<PlayerHealth>();
            if (ph != null) {
                ph.TakeDamage(damage);
            }
        }

        Die();
    }


    // 被外部打死时：中断攻击流程，不引爆，直接清掉所有特效
    void InterruptAndDie() {
        if (attackCoroutine != null) {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        ClearWindupVfx();
        Die();
    }

    void ClearWindupVfx() {
        if (windupVfxInstance != null) {
            Destroy(windupVfxInstance);
            windupVfxInstance = null;
        }
    }

    void Die() {
        state = State.Dead;
        ClearWindupVfx(); // 保险起见，死亡时再确认一次特效已清除
        Destroy(gameObject);
    }

    void FacePlayer() {
        Vector3 direction = player.position - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f) {
            Quaternion targetRot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 8f * Time.deltaTime);
        }
    }

    void OnDestroy() {
        if (health != null)
            health.OnDeath -= InterruptAndDie;
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}