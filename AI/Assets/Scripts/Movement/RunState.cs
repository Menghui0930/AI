using UnityEngine;

[System.Serializable]
public class RunState : MovementBaseState {
    public override void EnterState(MovementStageManager movement) {
        movement.anim.SetBool("Running", true);
    }

    public override void UpdateState(MovementStageManager movement) {
        if (movement.m_RunAction.WasReleasedThisFrame()) ExitState(movement, movement.Walk);
        else if (movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);

        if (movement.vInput < 0) movement.currentMoveSpeed = movement.runBackSpeed;
        else movement.currentMoveSpeed = movement.runSpeed;

        FaceMoveDirection(movement);
    }

    void FaceMoveDirection(MovementStageManager movement) {
        if (movement.dir.sqrMagnitude < 0.0001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(movement.dir.normalized, Vector3.up);
        movement.transform.rotation = Quaternion.Slerp(
            movement.transform.rotation,
            targetRotation,
            Time.deltaTime * movement.runTurnSpeed
        );
    }

    void ExitState(MovementStageManager movement, MovementBaseState State) {
        movement.anim.SetBool("Running", false);
        movement.SwitchState(State);
    }
}