using System.Collections;
using UnityEngine;

public class SniperSpawnPoint : MonoBehaviour, IRoundSpawnPoint
{
    public GameObject sniperPrefab;
    public float spawnInterval = 30f;

    [Header("这个生成点专属的柱子（拖场景里对应的物体）")]
    public Transform[] climbPoints;
    public Transform[] topPoints;

    private GameObject currentInstance;
    private Coroutine spawnRoutine;

    public void SpawnForRound(int roundNumber)
    {
        if (spawnRoutine == null)
        {
            spawnRoutine = StartCoroutine(SpawnLoop());
        }
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            if (currentInstance == null)
            {
                currentInstance = Instantiate(sniperPrefab, transform.position, transform.rotation);

                SniperClimbBehaviour sniper = currentInstance.GetComponent<SniperClimbBehaviour>();
                if (sniper != null)
                {
                    sniper.climbPoints = climbPoints;
                    sniper.topPoints = topPoints;
                }
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    public bool IsCleared()
    {
        return currentInstance == null;
    }
}