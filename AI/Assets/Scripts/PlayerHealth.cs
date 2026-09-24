using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour,IDamageable {
    [Header("Health")]
    public int maxHealth = 100;
    private int currentHealth;

    public int CurrentHealth => currentHealth;
    public bool IsDead => currentHealth <= 0;

    // 供 UI 或其他脚本订阅（比如更新血条）
    public event Action<int, int> OnHealthChanged; // (当前血量, 最大血量)
    public event Action OnDeath;

    void Awake() {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount) {
        if (IsDead || amount <= 0) return;

        currentHealth -= amount;
        currentHealth = Mathf.Max(currentHealth, 0);

        Debug.Log($"Player took {amount} damage. Health: {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Heal(int amount) {
        if (IsDead || amount <= 0) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);

        Debug.Log($"Player healed {amount}. Health: {currentHealth}/{maxHealth}");

        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    // 可选：完全恢复满血（比如捡到补给包）
    public void FullHeal() {
        Heal(maxHealth - currentHealth);
    }

    void Die() {
        Debug.Log("Player died.");
        OnDeath?.Invoke();

        // 在这里加死亡后的处理，比如：
        // - 播放死亡动画
        // - 禁用玩家移动脚本
        // - 显示 Game Over UI
        // - 延迟重新加载场景等
    }
}