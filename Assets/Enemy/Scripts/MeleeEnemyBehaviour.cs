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
    public float attackRange = 1.5f;     
    public float giveUpAttackRange = 2f; 
    public int damage = 1;
    public float meleeHitRadius = 1f;
    public Transform meleeOrigin; 

    private NavMeshAgent agent;
    private Animator anim;
    private Transform player;
    private Vector3 homePosition;
    private Quaternion homeRotation;

    [Header("Carryover VFX")]
    public GameObject carryoverVfxPrefab;
    private GameObject carryoverVfxInstance;

    [Header("Loot Drop")]
    public GameObject healthPickupPrefab;
    [Range(0f, 1f)]
    public float dropChance = 0.1f; // 10%

    enum State { Idle, Chasing, Attacking, Returning, Dead }
    State state = State.Idle;

    void Start() {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        agent.speed = moveSpeed;
        currentHealth = maxHealth;

        homePosition = transform.position;
        homeRotation = transform.rotation;
    }

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
            anim.SetBool("Attack", false);
            agent.isStopped = false;
            state = State.Chasing;
        }
    }

    void HandleReturning() {
        agent.SetDestination(homePosition);

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            transform.rotation = Quaternion.Slerp(transform.rotation, homeRotation, rotateSpeed * Time.deltaTime);

            if (Quaternion.Angle(transform.rotation, homeRotation) < 2f) {
                anim.SetBool("Chase",false);
                state = State.Idle; 
            }
        }
    }

    public void MarkAsCarriedOver() {
        if (carryoverVfxInstance != null) return; 

        if (carryoverVfxPrefab != null) {
            carryoverVfxInstance = Instantiate(carryoverVfxPrefab, transform.position, Quaternion.identity, transform);
            damage *= 2;    
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
        TryDropLoot();

        Destroy(gameObject);
    }

    void TryDropLoot() {
        if (healthPickupPrefab == null) return;

        if (Random.value <= dropChance) {
            Instantiate(healthPickupPrefab, new Vector3(transform.position.x,transform.position.y + 0.8f,transform.position.z), Quaternion.identity);
        }
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