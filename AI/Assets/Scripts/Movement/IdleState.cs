using UnityEngine;

[System.Serializable]
public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStageManager movement) {

    }

    public override void UpdateState(MovementStageManager movement) {
        if (movement.dir.magnitude > 0.01f) {
            if (movement.m_RunAction.WasPressedThisFrame()) movement.SwitchState(movement.Run);
            else movement.SwitchState(movement.Walk);
        }

        if (movement.m_CrouchAction.WasPressedThisFrame()) movement.SwitchState(movement.Crouch);
    }
}
