using UnityEngine;

[System.Serializable]
public class HipFireState : AimBaseState
{
    public override void EnterState(AimStageManager aim) {
        aim.anim.SetBool("Aiming",false);
        aim.currentFov = aim.hipFov;
    }
    public override void UpdateState(AimStageManager aim) {
        if (aim.m_Aim.WasPressedThisFrame()) aim.SwitchState(aim.Aim);
    }
}
