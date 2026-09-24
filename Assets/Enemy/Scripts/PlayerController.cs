using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private float horizontal;
    private float vertical;

    // Camera
    public Transform cameraTransform;

    // Health
    public int maxHealth = 3;
    public int currentHealth;

    // Invincibility
    public float invincibleTime = 2f;
    private bool isInvincible = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        currentHealth = maxHealth;
    }

    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        // Camera forward
        Vector3 forward = cameraTransform.forward;

        // Camera right
        Vector3 right = cameraTransform.right;

        // Remove vertical angle
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // Calculate movement direction
        Vector3 moveDirection =
            forward * vertical +
            right * horizontal;

        // Prevent diagonal movement from being faster
        moveDirection.Normalize();

        // Move player
        rb.MovePosition(
            rb.position +
            moveDirection * moveSpeed * Time.fixedDeltaTime
        );

        // Player faces movement direction
        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation =
                Quaternion.LookRotation(moveDirection);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );
        }
    }

    // Take damage
    public void TakeDamage()
    {
        if (isInvincible)
            return;

        currentHealth--;

        Debug.Log("Player Health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        // Start invincibility
        StartCoroutine(Invincibility());
    }

    // Invincibility timer
    System.Collections.IEnumerator Invincibility()
    {
        isInvincible = true;

        Debug.Log("Player is Invincible");

        yield return new WaitForSeconds(invincibleTime);

        isInvincible = false;

        Debug.Log("Player is no longer Invincible");
    }

    void Die()
    {
        Debug.Log("Player Died");

    }
}