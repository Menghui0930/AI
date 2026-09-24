using System.Collections;
using UnityEngine;

public class BulletSpawner : MonoBehaviour {
    [Header("配置项")]
    public GameObject bulletPrefab;   // 你的子弹 Prefab
    public Transform startPoint;      // 起点位置
    public Transform endPoint;        // 终点位置
    public float moveSpeed = 10f;     // 子弹移动速度
    public float spawnInterval = 2f;  // 生成间隔（秒）

    private void Start() {
        // 启动定时生成子弹的协程
        StartCoroutine(SpawnBulletRoutine());
    }

    private IEnumerator SpawnBulletRoutine() {
        while (true) {
            SpawnAndMoveBullet();
            // 等待 2 秒后继续循环
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnAndMoveBullet() {
        if (bulletPrefab == null || startPoint == null || endPoint == null) {
            Debug.LogWarning("请检查 Inspector，确保 Prefab 和起终点已赋值！");
            return;
        }

        // 1. 在起点生成子弹，并让它朝向终点
        Vector3 direction = (endPoint.position - startPoint.position).normalized;
        Quaternion rotation = direction != Vector3.zero ? Quaternion.LookRotation(direction) : Quaternion.identity;

        GameObject bullet = Instantiate(bulletPrefab, startPoint.position, rotation);

        // 2. 启动让该子弹移动的协程
        StartCoroutine(MoveBulletRoutine(bullet));
    }

    private IEnumerator MoveBulletRoutine(GameObject bullet) {
        // 当子弹还存在，且没有到达终点时循环
        while (bullet != null && Vector3.Distance(bullet.transform.position, endPoint.position) > 0.1f) {
            // 让子弹匀速移向终点
            bullet.transform.position = Vector3.MoveTowards(
                bullet.transform.position,
                endPoint.position,
                moveSpeed * Time.deltaTime
            );

            yield return null; // 等待下一帧
        }

        // 到达终点后销毁子弹，释放内存
        if (bullet != null) {
            Destroy(bullet);
        }
    }
}
