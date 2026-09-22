using UnityEngine;

[System.Serializable]
public class AimState : AimBaseState
{
    public override void EnterState(AimStageManager aim) {
        aim.anim.SetBool("Aiming",true);
        aim.currentFov = aim.adsFov;
    }
    public override void UpdateState(AimStageManager aim) {
        if (aim.m_Aim.WasReleasedThisFrame()) aim.SwitchState(aim.Hip);
    }
}
