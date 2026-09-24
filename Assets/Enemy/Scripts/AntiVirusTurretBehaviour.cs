using UnityEngine;

public class AntiVirusTurretBehaviour : MonoBehaviour
{
    // Rotation
    public float rotationSpeed = 60f;

    // Shooting
    public GameObject energyPulse;
    public Transform firePoint;
    public float fireInterval = 2f;

    private float fireTimer = 0f;

    void Update()
    {
        // Continuously rotate
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Shooting timer
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    void Fire()
    {
        Instantiate(
            energyPulse,
            firePoint.position,
            firePoint.rotation
        );
    }
}
