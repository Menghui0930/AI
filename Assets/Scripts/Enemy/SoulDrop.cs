using UnityEngine;

public class SoulDrop : MonoBehaviour
{
    [SerializeField] private GameObject soulPrefab;
    [SerializeField] private int soulAmount = 1;

    private EnemyHealth enemyHealth;

    private void Awake()
    {
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealth.OnDeath += DropSoul;
    }

    private void OnDestroy()
    {
        if (enemyHealth != null)
            enemyHealth.OnDeath -= DropSoul;
    }

    private void DropSoul()
    {
        for (int i = 0; i < soulAmount; i++)
        {
            Instantiate(soulPrefab, transform.position, Quaternion.identity);
        }
    }
}
