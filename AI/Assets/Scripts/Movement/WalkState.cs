using UnityEngine;
[System.Serializable]
public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStageManager movement) {
        movement.anim.SetBool("Walking",true);
    }

    public override void UpdateState(MovementStageManager movement) {
        if (movement.m_RunAction.WasPressedThisFrame()) ExitState(movement,movement.Run);
        else if (movement.m_CrouchAction.WasPressedThisFrame()) ExitState(movement,movement.Crouch);
        else if (movement.dir.magnitude < 0.1f) ExitState(movement,movement.Idle);

        if (movement.vInput < 0) movement.currentMoveSpeed = movement.walkBackSpeed;
        else movement.currentMoveSpeed = movement.walkSpeed;
    }

    void ExitState(MovementStageManager movement, MovementBaseState State) {
        movement.anim.SetBool("Walking", false);
        movement.SwitchState(State);
    }
}
