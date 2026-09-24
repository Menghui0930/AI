using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    private InputActionAsset inputAction;
    private InputAction m_mouse0;

    [Header("Fire Rate")]
    [SerializeField] float fireRate;
    float fireRateTimer;
    [SerializeField] bool semiAuto;

    [Header("Bullet Properties")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform barrelPos;
    [SerializeField] float bulletVelocity;
    [SerializeField] int bulletPerShoot;
    AimStageManager aim;

    WeaponAmmo ammo;

    private Animator anim;

    private void Awake() {
        aim = GetComponent<AimStageManager>();
        ammo = GetComponent<WeaponAmmo>();
        anim = GetComponent<Animator>();
        m_mouse0 = InputSystem.actions.FindAction("Shoot");
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fireRateTimer = fireRate;
    }

    private float lastBarrelY;

    void Update() {
        if (ShouldFire()) Fire();
    }

    bool ShouldFire() {
        fireRateTimer += Time.deltaTime;

        if(fireRateTimer < fireRate) return false;
        if(ammo.currentAmmo == 0 ) return false;
        if(semiAuto && m_mouse0.WasPressedThisFrame()) return true;
        if(!semiAuto && m_mouse0.IsPressed()) return true;
        return false;
    }

    void Fire() {
        anim.SetTrigger("Shooting");
        fireRateTimer = 0;
        //barrelPos.LookAt(aim.aimPos);
        ammo.currentAmmo--;

        for (int i = 0; i < bulletPerShoot; i++) {
            Vector3 direction = (aim.aimPos.position - barrelPos.position).normalized;
            Quaternion rotation = direction != Vector3.zero ? Quaternion.LookRotation(direction) : Quaternion.identity;
            GameObject currentBullet = Instantiate(bullet, barrelPos.position, rotation);
            Rigidbody theRB = currentBullet.GetComponent<Rigidbody>();
            theRB.AddForce(direction * bulletVelocity, ForceMode.Impulse);
        }
    }
}
