using UnityEngine.InputSystem;

[System.Serializable]
public abstract class MovementBaseState {

    public abstract void EnterState(MovementStageManager movement);

    public abstract void UpdateState(MovementStageManager movement);
}
