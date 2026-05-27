using StarterAssets;
using UnityEngine;

public class UnEquidWeapoState : BasePlayerState
{
    public UnEquidWeapoState(ThirdPersonController controller, StateMachine stateMachine) : base(controller, stateMachine)
    {
    }
    public override void EnterState()
    {
        base.EnterState();
        controller._animator.ResetTrigger("DrawWeapon");
        controller._animator.SetTrigger("UnEquidWeapon");

    }
}