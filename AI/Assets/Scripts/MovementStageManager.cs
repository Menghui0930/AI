using UnityEngine;
using UnityEngine.InputSystem;

public class MovementStageManager : MonoBehaviour {
    public InputActionAsset InputAction;
    public InputAction m_MoveAction;
    public InputAction m_RunAction;
    public InputAction m_CrouchAction;

    #region Movement

    public float currentMoveSpeed;
    public float walkSpeed = 3, walkBackSpeed = 2;
    public float runSpeed = 7, runBackSpeed = 5;
    public float crouchSpeed = 2, crouchBackSpeed = 1;

    public float runTurnSpeed = 10f; 


    [HideInInspector] public Vector3 dir;
    public float hzInput, vInput;
    CharacterController controller;

    private float targetSpeed;
    private float speedChangeRate = 10f;
    // animation
    private float hzBlend, vBlend;

    #endregion

    #region GroundCheck

    [SerializeField] private float groundYOffset;
    [SerializeField] private LayerMask groundMask;
    private Vector3 spherePos;

    #endregion

    #region Gravity

    [SerializeField] float gravity = -9.81f;
    private Vector3 velocity;

    #endregion

    [HideInInspector] public Animator anim;

    
    [HideInInspector] MovementBaseState currentState;
    [HideInInspector] public IdleState Idle = new IdleState();
    [HideInInspector] public WalkState Walk = new WalkState();
    [HideInInspector] public CrouchState Crouch = new CrouchState();
    [HideInInspector] public RunState Run = new RunState();
    

    private void Awake() {
        m_MoveAction = InputSystem.actions.FindAction("Move");
        m_RunAction = InputSystem.actions.FindAction("Run");
        m_CrouchAction = InputSystem.actions.FindAction("Crouch");
    }

    private void Start() {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        SwitchState(Idle);
    }

    private void Update() {
        GetDirAndMove();
        Gravity();

        hzBlend = Mathf.Lerp(hzBlend, hzInput, Time.deltaTime * speedChangeRate);
        vBlend = Mathf.Lerp(vBlend, vInput, Time.deltaTime * speedChangeRate);
        anim.SetFloat("hzInput", hzBlend);
        anim.SetFloat("vInput", vBlend);

        currentState.UpdateState(this);
    }

    
    public void SwitchState(MovementBaseState state) {
        currentState = state;
        currentState.EnterState(this);
    }
    

    void GetDirAndMove() {
        Vector2 m_moveAmt = m_MoveAction.ReadValue<Vector2>();

        hzInput = m_moveAmt.x;
        vInput = m_moveAmt.y;

        float inputMagnitude = new Vector2(hzInput, vInput).magnitude;
        float goalSpeed = inputMagnitude < 0.01f ? 0f : targetSpeed;

        currentMoveSpeed = Mathf.Lerp(currentMoveSpeed, goalSpeed, Time.deltaTime * speedChangeRate);

        dir = transform.forward.normalized * vInput + transform.right.normalized * hzInput;

        controller.Move(dir.normalized * currentMoveSpeed * Time.deltaTime);

    }

    bool IsGrounded() {
        spherePos = new Vector3(transform.position.x, transform.position.y - groundYOffset, transform.position.z);
        if (Physics.CheckSphere(spherePos, controller.radius - 0.05f, groundMask)) return true;
        return false;
    }

    void Gravity() {
        if (!IsGrounded()) velocity.y += gravity * Time.deltaTime;
        else if (velocity.y < 0) velocity.y = -2; // let it not change to flow always stick ground

        // Move once to combine horizontal movement and vertical gravity.
        Vector3 moveVec = dir.normalized * currentMoveSpeed + new Vector3(0, velocity.y, 0);
        controller.Move(moveVec * Time.deltaTime);
    }

    /*
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(spherePos, controller.radius - 0.05f);
    }
    */
}
