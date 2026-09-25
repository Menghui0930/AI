using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour {
    [SerializeField] private float timeToDestroy;
    [SerializeField] private int damage = 1;

    float timer;

    void Update() {
        timer += Time.deltaTime;
        if (timer > timeToDestroy) Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Touch Enemy" + other);
        if (other.CompareTag("Enemy")) {
            IDamageable damageable = other.GetComponent<IDamageable>();
            if (damageable != null) {
                damageable.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}