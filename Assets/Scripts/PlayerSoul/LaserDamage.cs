using UnityEngine;

public class LaserDamage : MonoBehaviour
{
    public int damage = 1;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            IDamageable target = other.GetComponent<IDamageable>();

            if (target != null)
            {
                target.TakeDamage(damage);
                Debug.Log("Damage!");
            }
        }
    }
}
