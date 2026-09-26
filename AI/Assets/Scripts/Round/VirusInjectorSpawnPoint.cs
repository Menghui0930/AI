using UnityEngine;

public class VirusInjectorSpawnPoint : MonoBehaviour, IRoundSpawnPoint {
    public GameObject virusInjectorPrefab;

    public int activeFromRound = 1; // 默认第1round就生效，这个新加的点设成2

    private GameObject currentInstance;

    public void SpawnForRound(int roundNumber) {
        if (roundNumber < activeFromRound) return; // 还没轮到这个点上场

        if (!IsCleared()) return;

        currentInstance = Instantiate(virusInjectorPrefab, transform.position, transform.rotation);
    }

    public bool IsCleared() {
        return currentInstance == null;
    }
}