using UnityEngine;

public class PacketBehaviour : MonoBehaviour
{
    //生成时追踪玩家当前位置的AI

    [Header("Movement")]
    public float moveSpeed = 8f;

    [Header("Damage")]
    public int damage = 1;

    private Vector3 targetPosition;

    void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            // Remember player's position only once
            targetPosition = playerObject.transform.position;
        }
        else
        {
            targetPosition = transform.position + transform.forward * 10f;
        }
    }

    void Update()
    {
        // Move toward the position recorded at the beginning
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Destroy when it reaches the target position
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable target =
                other.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log("Damage!");
            }

            Destroy(gameObject);
        }
    }
}
