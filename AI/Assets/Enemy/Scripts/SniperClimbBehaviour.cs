using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class SniperClimbBehaviour : MonoBehaviour, IDamageable {
    public Transform TargetPoint;
    public Transform player;

    [Header("Health")]
    public int maxHealth = 2;
    private int currentHealth;

    [Header("Points")]
    public Transform[] climbPoints;
    public Transform[] topPoints;
    public Transform ShootingPoints;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float climbSpeed = 2f;
    public float turnDuration = 0.3f;

    [Header("Flee")]
    public float detectRange = 6f;

    [Header("Attack")]
    public float attackRange = 20f;
    public float aimLockDuration = 2f;
    public float attackCooldown = 3f;
    public float hitCheckRadius = 1.5f;
    public LayerMask obstructionMask;
    public int damage = 1;
    public Vector3 laserEndPoint;
    public GameObject ExplodeVFX;

    [Header("Aiming / Lose Sight")]
    public float loseSightGraceTime = 1.5f; // 看不到玩家后，等这么久才放弃去下一个点
    private float loseSightTimer = 0f;

    [Header("Laser Visual")]
    public LineRenderer laserLine;

    private NavMeshAgent agent;
    private int currentPointIndex = -1;
    private float attackTimer;
    private Vector3 lockedAimPoint;
    private bool isBusy = false;

    enum State { MovingToPoint, Aiming, Dead }
    State state = State.MovingToPoint;

    void Start() {
        currentHealth = maxHealth;
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;

        if (laserLine != null) laserLine.enabled = false;

        PickNewPoint();
    }

    void Update() {
        if (TargetPoint == null || state == State.Dead) return;
        if (isBusy) return;

        switch (state) {
            case State.MovingToPoint:
                HandleMovingToPoint();
                break;
            case State.Aiming:
                HandleAiming();
                break;
        }
    }

    void HandleMovingToPoint() {
        float playerDist = Vector3.Distance(transform.position, TargetPoint.position);

        if (playerDist <= detectRange) {
            PickNewPoint();
            return;
        }

        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance) {
            StartCoroutine(ClimbUpRoutine(currentPointIndex));
        }
    }

    void HandleAiming() {
        attackTimer -= Time.deltaTime;

        if (!HasLineOfSight()) {
            loseSightTimer += Time.deltaTime;

            if (loseSightTimer >= loseSightGraceTime) {
                loseSightTimer = 0f;
                StartCoroutine(ClimbDownRoutine(currentPointIndex));
            }
            return; // 缓冲期内什么都不做，继续留在原地观察
        } else {
            loseSightTimer = 0f; // 重新看到玩家了，清空计时
        }

        FacePlayerHorizontal();

        if (attackTimer <= 0f) {
            StartCoroutine(AttackSequence());
        }
    }

    bool HasLineOfSight() {
        Transform top = topPoints[currentPointIndex];
        Vector3 toPlayer = TargetPoint.position - top.position;
        float distance = toPlayer.magnitude;

        if (distance > attackRange) return false;

        if (Physics.Raycast(top.position, toPlayer.normalized, out RaycastHit hit, distance, obstructionMask)) {
            return false;
        }

        return true;
    }

    void FacePlayerHorizontal() {
        Vector3 direction = TargetPoint.position - transform.position;
        direction.y = 0;
        if (direction.sqrMagnitude > 0.01f) {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }

    IEnumerator AttackSequence() {
        isBusy = true;

        lockedAimPoint = TargetPoint.position;

        if (laserLine != null) {
            Vector3 fireOrigin = ShootingPoints.position;
            Vector3 aimDirection = (lockedAimPoint - fireOrigin).normalized;
            float maxLaserDistance = attackRange * 2f; // 保证够长，能穿过玩家继续打到后面的地面/墙

            laserEndPoint = fireOrigin + aimDirection * maxLaserDistance;

            // 沿着瞄准方向继续往前打，找真正会挡住去路的地面/墙壁
            if (Physics.Raycast(fireOrigin, aimDirection, out RaycastHit hit, maxLaserDistance, obstructionMask)) {
                laserEndPoint = hit.point;
            }

            laserLine.enabled = true;
            laserLine.SetPosition(0, fireOrigin);
            laserLine.SetPosition(1, laserEndPoint);
        }

        yield return new WaitForSeconds(aimLockDuration);

        float distToLockedPoint = Vector3.Distance(TargetPoint.position, lockedAimPoint);
        if (distToLockedPoint <= hitCheckRadius) {
            PlayerHealth ph = player.GetComponent<PlayerHealth>();
            if (ph != null) {
                Instantiate(ExplodeVFX, lockedAimPoint, Quaternion.identity);
                ph.TakeDamage(damage);
            }
        } else {
            Instantiate(ExplodeVFX, laserEndPoint, Quaternion.identity);
        }
        


        if (laserLine != null) laserLine.enabled = false;

        attackTimer = attackCooldown;
        isBusy = false;
    }

    // ---------------- 爬柱子（上） ----------------
    IEnumerator ClimbUpRoutine(int index) {
        isBusy = true;
        agent.enabled = false;

        Transform climbPoint = climbPoints[index];
        Transform topPoint = topPoints[index];
        float wallYaw = climbPoint.eulerAngles.y;

        // 1. 先对齐 climb point 的 Y 轴朝向（面向柱子）
        yield return RotateOverTime(Quaternion.Euler(0f, wallYaw, 0f), turnDuration);

        // 2. 转成攀爬姿势（绕 X 轴），Y 朝向保持不变
        yield return RotateOverTime(Quaternion.Euler(-90f, wallYaw, 0f), turnDuration);

        // 3. 只沿 Y 轴直线上升，X/Z 不变（贴着柱子往上爬），直到跟 topPoint 一样高
        Vector3 climbStart = transform.position;
        Vector3 climbEnd = new Vector3(climbStart.x, topPoint.position.y, climbStart.z);
        yield return MoveOverSpeed(climbStart, climbEnd, climbSpeed);

        // 4. 转回正常站姿
        yield return RotateOverTime(Quaternion.Euler(0f, wallYaw, 0f), turnDuration);

        // 5. 水平移动到 topPoint 的实际位置（走上平台）
        Vector3 stepStart = transform.position;
        yield return MoveOverSpeed(stepStart, topPoint.position, climbSpeed);

        state = State.Aiming;
        attackTimer = 0f;
        loseSightTimer = 0f;
        isBusy = false;
    }

    // ---------------- 爬柱子（下） ----------------
    IEnumerator ClimbDownRoutine(int index) {
        isBusy = true;

        Transform climbPoint = climbPoints[index];
        float wallYaw = climbPoint.eulerAngles.y;

        // 1. 先对齐 climb point 的 Y 轴朝向
        yield return RotateOverTime(Quaternion.Euler(0f, wallYaw+180f, 0f), turnDuration);

        // 2. 水平移动，回到柱子正上方（X/Z 对齐 climbPoint，高度先保持不变）
        Vector3 backStart = transform.position;
        Vector3 backEnd = new Vector3(climbPoint.position.x, backStart.y, climbPoint.position.z);
        yield return MoveOverSpeed(backStart, backEnd, climbSpeed);

        // 3. 转成攀爬姿势
        yield return RotateOverTime(Quaternion.Euler(90f, wallYaw+180, 0f), turnDuration);

        // 4. 只沿 Y 轴直线下降到 climbPoint 的高度
        Vector3 downStart = transform.position;
        yield return MoveOverSpeed(downStart, climbPoint.position, climbSpeed);

        // 5. 转回正常站姿
        yield return RotateOverTime(Quaternion.Euler(0f, wallYaw, 0f), turnDuration);

        agent.enabled = true;
        agent.Warp(climbPoint.position);
        PickNewPoint();
        state = State.MovingToPoint;

        isBusy = false;
    }

    IEnumerator RotateOverTime(Quaternion targetRot, float duration) {
        Quaternion startRot = transform.rotation;
        float elapsed = 0f;
        while (elapsed < duration) {
            transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRot;
    }

    IEnumerator MoveOverSpeed(Vector3 from, Vector3 to, float speed) {
        float distance = Vector3.Distance(from, to);
        if (distance < 0.001f) yield break;

        float duration = distance / speed;
        float elapsed = 0f;
        while (elapsed < duration) {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.position = to;
    }

    void PickNewPoint() {
        int bestIndex = -1;
        float bestDistance = -1f;

        for (int i = 0; i < climbPoints.Length; i++) {
            if (i == currentPointIndex && climbPoints.Length > 1) continue;

            float dist = Vector3.Distance(climbPoints[i].position, TargetPoint.position);
            if (dist > bestDistance) {
                bestDistance = dist;
                bestIndex = i;
            }
        }

        if (bestIndex == -1) bestIndex = 0;

        currentPointIndex = bestIndex;
        agent.SetDestination(climbPoints[currentPointIndex].position);
    }

    public void TakeDamage(int amount) {
        if (state == State.Dead) return;

        currentHealth -= amount;
        if (currentHealth <= 0) {
            Die();
        }
    }

    void Die() {
        state = State.Dead;
        StopAllCoroutines();
        if (laserLine != null) laserLine.enabled = false;
        Destroy(gameObject);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, hitCheckRadius);

        if (climbPoints != null) {
            Gizmos.color = Color.cyan;
            foreach (var p in climbPoints) {
                if (p != null) Gizmos.DrawSphere(p.position, 0.3f);
            }
        }

        if (topPoints != null) {
            Gizmos.color = Color.green;
            foreach (var p in topPoints) {
                if (p != null) Gizmos.DrawSphere(p.position, 0.3f);
            }
        }
    }
}