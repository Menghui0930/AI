using System;
using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable {
    public int maxHealth = 3;
    private int currentHealth;

    public bool IsDead => currentHealth <= 0;

    public event Action OnDeath;

    void Awake() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount) {
        if (IsDead || amount <= 0) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took {amount} damage. Health: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0) {
            OnDeath?.Invoke();
        }
    }
}