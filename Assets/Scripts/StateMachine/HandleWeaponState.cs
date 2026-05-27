using UnityEngine;
using StarterAssets;
public class HandleWeaponState : BasePlayerState
{
    public bool IsAttackFinished { get; private set; }
    public HandleWeaponState(ThirdPersonController controller, StateMachine stateMachine)
        : base(controller, stateMachine) { }

    public override void EnterState()
    {
        controller._animator.SetTrigger("DrawWeapon");
    }
    
    public override void Exit()
    {
        // controller._animator.SetTrigger("UnEquidWeapon");

    }
}
