using System.Collections.Generic;
using UnityEngine;

public class MeleeSpawnPoint : MonoBehaviour, IRoundSpawnPoint {
    [Header("Setup")]
    public GameObject meleeEnemyPrefab;
    public int enemyCount = 3; // 固定数量，在 Inspector 里每个点各自设置 2 或 3，不用随机
    public float spawnRadius = 1.5f; // 生成时略微散开，避免叠在同一点上互相推挤

    private RobotGroupController groupController;
    private List<GameObject> spawnedEnemies = new List<GameObject>();

    void Awake() {
        // 这个生成点本身就是"群体中心"，所以直接在它身上挂 EnemyGroupController
        groupController = GetComponent<RobotGroupController>();
        if (groupController == null) {
            groupController = gameObject.AddComponent<RobotGroupController>();
        }
    }

    public void SpawnForRound(int roundNumber) {
        if (!IsCleared()) {
            // 这个点的敌人还活着，没有被重新生成 —— 给它们加上"保留"VFX
            foreach (var enemy in spawnedEnemies) {
                if (enemy != null) {
                    MeleeEnemyBehaviour meleeBehaviour = enemy.GetComponent<MeleeEnemyBehaviour>();
                    if (meleeBehaviour != null) {
                        meleeBehaviour.MarkAsCarriedOver();
                    }
                }
            }
            return;
        }

        spawnedEnemies.Clear();

        for (int i = 0; i < enemyCount; i++) {
            Vector2 offset2D = Random.insideUnitCircle * spawnRadius;
            Vector3 spawnPos = transform.position + new Vector3(offset2D.x, 0f, offset2D.y);

            GameObject enemy = Instantiate(meleeEnemyPrefab, spawnPos, transform.rotation,transform);
            spawnedEnemies.Add(enemy);
        }

        // 生成完子物体后，让 GroupController 重新收集成员
        groupController.RefreshMembers();
    }

    public bool IsCleared() {
        spawnedEnemies.RemoveAll(e => e == null); // 清掉已经被 Destroy 的
        return spawnedEnemies.Count == 0;
    }
}