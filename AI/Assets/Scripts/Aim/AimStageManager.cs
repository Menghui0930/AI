using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class AimStageManager : MonoBehaviour {
    public InputActionAsset InputAction;
    public InputAction m_Aim;
    public InputAction lookAction;

    [SerializeField] private Transform camFollowPos;
    [SerializeField] private float mouseSensitivity = .8f;

    private float xRotation;
    private float yRotation;

    #region CameraFOV
    [HideInInspector] public CinemachineCamera vCam;
    public float adsFov = 40;
    [HideInInspector] public float hipFov = 60;
    [HideInInspector] public float currentFov;
    public float fovSmoothSpeed = 10f;

    #endregion

    public Transform aimPos;
    [HideInInspector] public Vector3 actualAimPos;
    [SerializeField] float aimSmoothSpeed = 20;
    [SerializeField] LayerMask aimMask;

    [HideInInspector] public Animator anim;

    AimBaseState currentState;
    [HideInInspector] public HipFireState Hip = new HipFireState();
    [HideInInspector] public AimState Aim = new AimState();


    private void Awake() {
        lookAction = InputSystem.actions.FindAction("Look");
        m_Aim = InputSystem.actions.FindAction("Aim");
        //SetCursorState(false);

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start() {

        vCam = GetComponentInChildren<CinemachineCamera>();
        if (vCam == null) {
            Debug.Log("noVcam");
        }
        hipFov = vCam.Lens.FieldOfView;
        anim = GetComponent<Animator>();

        SwitchState(Hip);
    }

    // Update is called once per frame
    void Update() {

        if (lookAction != null) {
            // Get input from new Input System
            Vector2 lookDelta = lookAction.ReadValue<Vector2>();

            // Apply sensitivity
            xRotation += lookDelta.x * mouseSensitivity;
            yRotation -= lookDelta.y * mouseSensitivity;

            // Clamp vertical rotation
            yRotation = Mathf.Clamp(yRotation, -60, 60f);
        }

        vCam.Lens.FieldOfView = Mathf.Lerp(vCam.Lens.FieldOfView, currentFov, fovSmoothSpeed * Time.deltaTime);
        //Debug.Log(vCam.Lens.FieldOfView);

        Vector2 screenCentre = new Vector2(Screen.width / 2, Screen.height / 2);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, aimMask)) {
            aimPos.position = Vector3.Lerp(aimPos.position, hit.point, aimSmoothSpeed * Time.deltaTime);
        }
        
        currentState.UpdateState(this);
    }

    private void LateUpdate() {
        if (camFollowPos != null) {
            // Apply rotations
            camFollowPos.localEulerAngles = new Vector3(yRotation, camFollowPos.localEulerAngles.y, camFollowPos.localEulerAngles.z);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, xRotation, transform.eulerAngles.z);
        }
    }

    public void SwitchState(AimBaseState state) {
        currentState = state;
        state.EnterState(this);
    }
}
