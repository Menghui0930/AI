using UnityEngine;

[System.Serializable]
public class CrouchState : MovementBaseState {
    public override void EnterState(MovementStageManager movement) {
        movement.anim.SetBool("Crouching", true);
        movement.currentMoveSpeed = 0f; 
    }

    public override void UpdateState(MovementStageManager movement) {
        movement.currentMoveSpeed = 0f;

        if (movement.m_RunAction.WasPressedThisFrame()) ExitState(movement, movement.Run);

        if (movement.m_CrouchAction.WasPressedThisFrame()) {
            if (movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);
            else ExitState(movement, movement.Walk);
        }
    }

    void ExitState(MovementStageManager movement, MovementBaseState State) {
        movement.anim.SetBool("Crouching", false);
        movement.SwitchState(State);
    }
}