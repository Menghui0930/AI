using UnityEngine;

public class FirewallBehaviour : MonoBehaviour
{
    //不会动，靠近触发范围伤害的AI

    [Header("Laser")]
    public GameObject laserObject;

    private void Start()
    {
        // Laser is off at the beginning
        if (laserObject != null)
        {
            laserObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ActivateLaser();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            DeactivateLaser();
        }
    }

    void ActivateLaser()
    {
        if (laserObject != null)
        {
            laserObject.SetActive(true);
        }

        Debug.Log("Firewall: Laser Activated!");
    }

    void DeactivateLaser()
    {
        if (laserObject != null)
        {
            laserObject.SetActive(false);
        }

        Debug.Log("Firewall: Laser Deactivated!");
    }}
