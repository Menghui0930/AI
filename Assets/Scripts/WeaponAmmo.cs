using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAmmo : MonoBehaviour
{
    private InputActionAsset InputAction;
    private InputAction m_Reload;

    public int clipSize;
    public int extraAmmo;
    [HideInInspector] public int currentAmmo;

    private void Awake() {
        m_Reload = InputSystem.actions.FindAction("Reload");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo = clipSize;
    }

    private void Update() {
        if (m_Reload.WasPressedThisFrame()) {
            Reload();
        }
    }

    public void Reload() {
        if (extraAmmo >= clipSize) {
            int ammonToReload = clipSize - currentAmmo;
            extraAmmo -= ammonToReload;
            currentAmmo += ammonToReload;
        } else if (extraAmmo > 0) {
            if (extraAmmo + currentAmmo > clipSize) {
                int leftOverAmmo = extraAmmo + currentAmmo - clipSize;
                extraAmmo = leftOverAmmo;
                currentAmmo = clipSize;
            } else {
                currentAmmo += extraAmmo;
                extraAmmo = 0;
            }
        }
    }

}
