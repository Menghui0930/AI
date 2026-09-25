using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour {
    [Header("Health")]
    public PlayerHealth playerHealth;
    public TextMeshProUGUI hpText;
    public Image hpBar;

    [Header("Ammo")]
    public WeaponAmmo weaponAmmo;
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI extraAmmoText;

    void OnEnable() {
        if (playerHealth != null) {
            playerHealth.OnHealthChanged += UpdateHealthUI;
        }
    }

    void OnDisable() {
        if (playerHealth != null) {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }

    void Start() {
        if (playerHealth != null) {
            UpdateHealthUI(playerHealth.CurrentHealth, playerHealth.maxHealth);
        }

        UpdateAmmoUI();
    }

    void Update() {
        // WeaponAmmo 目前没有事件通知，改用轮询检查
        UpdateAmmoUI();
    }

    void UpdateHealthUI(int current, int max) {
        if (hpText != null) {
            hpText.text = current + "/" + max;
        }

        if (hpBar != null) {
            hpBar.fillAmount = (float)current / max;
        }
    }

    void UpdateAmmoUI() {
        if (weaponAmmo == null) return;

        if (ammoText != null) {
            ammoText.text = weaponAmmo.currentAmmo.ToString();
        }

        if (extraAmmoText != null) {
            extraAmmoText.text = "/ " + weaponAmmo.extraAmmo;
        }
    }
}