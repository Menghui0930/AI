using UnityEngine;

public class VirusInjectorBehaviour : MonoBehaviour
{
    // Health
    public int maxHealth = 5;
    private int currentHealth;

    // Runner spawning
    public GameObject runnerPrefab;
    public Transform spawnPoint;

    public float spawnInterval = 5f;

    private float spawnTimer;

    void Start()
    {
        currentHealth = maxHealth;
        spawnTimer = 0f;
    }

    void Update()
    {
        // Keep spawning forever
        spawnTimer -= Time.deltaTime;

        if (spawnTimer <= 0f)
        {
            SpawnRunner();

            spawnTimer = spawnInterval;
        }
    }

    void SpawnRunner()
    {
        if (runnerPrefab == null)
        {
            Debug.LogWarning("Runner Prefab is not assigned!");
            return;
        }

        Vector3 spawnPosition = transform.position;

        if (spawnPoint != null)
        {
            spawnPosition = spawnPoint.position;
        }

        GameObject newRunner = Instantiate(
            runnerPrefab,
            spawnPosition,
            Quaternion.identity
        );

        // Find Player for the new Runner
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            RunnerBehaviour runner =
                newRunner.GetComponent<RunnerBehaviour>();

            if (runner != null)
            {
                runner.player = playerObject.transform;
            }
        }

        Debug.Log("Virus Injector spawned a Runner.");
    }

    // Injector takes damage
    public void TakeDamage()
    {
        currentHealth--;

        Debug.Log(
            "Virus Injector Health: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Virus Injector destroyed.");

        Destroy(gameObject);
    }
}