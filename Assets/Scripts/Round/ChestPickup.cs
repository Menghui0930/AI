using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class ChestPickup : MonoBehaviour {
    [Header("Rewards")]
    public int extraAmmoAmount = 150;
    public int healAmount = 50;

    [Header("Timing")]
    public float destroyDelayAfterOpen = 5f;

    private InputAction m_PickupAction;
    private Animator anim;

    private bool playerInRange = false;
    private bool isOpened = false;
    private Transform playerRef;

    void Awake() {
        anim = GetComponent<Animator>();
        m_PickupAction = InputSystem.actions.FindAction("Pickup");
    }

    void Update() {
        if (isOpened || !playerInRange) return;

        if (m_PickupAction != null && m_PickupAction.WasPressedThisFrame()) {
            Open(playerRef);
        }
    }

    void OnTriggerEnter(Collider other) {
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        playerRef = other.transform;
    }

    void OnTriggerExit(Collider other) {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        playerRef = null;
    }

    void Open(Transform player) {
        isOpened = true;

        WeaponAmmo ammo = player.GetComponent<WeaponAmmo>();
        if (ammo != null) {
            ammo.extraAmmo += extraAmmoAmount;
        }

        PlayerHealth health = player.GetComponent<PlayerHealth>();
        if (health != null) {
            health.Heal(healAmount);
        }

        if (anim != null) {
            anim.SetTrigger("Open");
        }

        StartCoroutine(DespawnAfterDelay());
    }

    IEnumerator DespawnAfterDelay() {
        yield return new WaitForSeconds(destroyDelayAfterOpen);
        Destroy(gameObject);
    }
}