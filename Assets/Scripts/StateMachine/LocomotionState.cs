using StarterAssets;
using UnityEngine;
public class LocomotionState: BasePlayerState
{
    public LocomotionState(ThirdPersonController controller, StateMachine stateMachine) : base(controller, stateMachine) { }

    public override void EnterState()
    {
        base.EnterState();
        // controller._animator.SetTrigger("UnEquidWeapon");

    }
    public override void Update()
    {
       base.Update();
    }
}
