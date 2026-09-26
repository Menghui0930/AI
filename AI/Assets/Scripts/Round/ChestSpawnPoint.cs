using UnityEngine;

public class ChestSpawnPoint : MonoBehaviour {
    public GameObject chestPrefab;
    private GameObject currentInstance;

    public void SpawnChest() {
        // 上一个宝箱还在场上（没被打开），就不生成新的
        if (currentInstance != null) return;

        currentInstance = Instantiate(chestPrefab, transform.position, transform.rotation);
    }
}