using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class MeleeEnemyBehaviour : MonoBehaviour, IDamageable {
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 3.5f;
    public float rotateSpeed = 8f;

    [Header("Attack")]
    public float attackRange = 1.5f;     // 进入这个距离才开始近战
    public float giveUpAttackRange = 2f; // 玩家跑出这个距离，停止攻击重新追（要比 attackRange 大一点，避免抖动）
    public int damage = 1;
    public float meleeHitRadius = 1f;
    public Transform meleeOrigin; // 打击判定的中心点，通常放在 enemy 前方

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private Vector3 homePosition;
    private Quaternion homeRotation;

    enum State { Idle, Chasing, Attacking, Returning, Dead }
    State state = State.Idle;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = moveSpeed;
        currentHealth = maxHealth;

        // 记住自己一开始站的位置和朝向，之后"回家"用
        homePosition = transform.position;
        homeRotation = transform.rotation;
    }

    // 由 GroupController 呼叫，通知这只 enemy 要不要进入警戒状态
    public void SetAlert(bool alert, Transform playerTransform) {
        if (state == State.Dead) return;

        if (alert) {
            player = playerTransform;
            agent.isStopped = false;
            state = State.Chasing;
        } else {
            anim.SetBool("Attack", false);
            agent.isStopped = false;
            state = State.Returning;
        }
    }

    void Update() {
        if (state == State.Dead) return;

        switch (state) {
            case State.Chasing:
                HandleChasing();
                break;
            case State.Attacking:
                HandleAttacking();
                break;
            case State.Returning:
                HandleReturning();
                break;
                // Idle 什么都不用做，站着发呆
        }
    }

    void HandleChasing() {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);

        if (dist <= attackRange) {
            agent.isStopped = true;
            anim.SetBool("Attack", true);
            anim.SetBool("Chase", false);
            state = State.Attacking;
            return;
        }

        anim.SetBool("Chase",true);
        agent.SetDestination(player.position);
    }

    void HandleAttacking() {
        if (player == null) {
            anim.SetBool("Attack", false);
            state = State.Returning;
            return;
        }

        FaceTarget(player.position);

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist > giveUpAttackRange) {
            // 玩家跑开了，重新追上去
            anim.SetBool("Attack", false);
            agent.isStopped = false;
            state = State.Chasing;
        }

        // 实际伤害判定不在这里手动倒数计时，
        // 而是靠攻击动画的 Animation Event 呼叫 DealDamage()（见下方说明）
    }

    void HandleReturning() {
        agent.SetDestination(homePosition);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            transform.rotation = Quaternion.Slerp(transform.rotation, homeRotation, rotateSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, homeRotation) < 2f) {
                state = State.Idle; // 回到原位、转回原朝向后，正式变回发呆
            }
        }
    }

    void FaceTarget(Vector3 targetPos) {
        Vector3 dir = targetPos - transform.position;
        dir.y = 0;
        if (dir.sqrMagnitude > 0.01f) {
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotateSpeed * Time.deltaTime);
        }
    }

    // 这个方法由攻击动画里的 Animation Event 呼叫，不是由代码倒数计时呼叫
    public void DealDamage() {
        if (player == null) return;

        Vector3 origin = meleeOrigin != null ? meleeOrigin.position : transform.position;
        Collider[] hits = Physics.OverlapSphere(origin, meleeHitRadius);

        foreach (var hit in hits) {
            if (hit.CompareTag("Player")) {
                IDamageable dmg = hit.GetComponent<IDamageable>();
                if (dmg != null) dmg.TakeDamage(damage);
            }
        }
    }

    public void TakeDamage(int amount) {
        if (state == State.Dead) return;

        currentHealth -= amount;
        if (currentHealth <= 0) Die();
    }

    void Die() {
        state = State.Dead;
        agent.isStopped = true;
        anim.SetBool("Attack", false);
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, giveUpAttackRange);

        if (meleeOrigin != null) {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(meleeOrigin.position, meleeHitRadius);
        }
    }
}