using StarterAssets;
using UnityEngine;

public class AttackState : BasePlayerState
{
    private float timePassed;
    private float clipLength;
    private float clipSpeed;

    public bool IsAttackFinished = true;

    public AttackState(ThirdPersonController controller, StateMachine stateMachine)
        : base(controller, stateMachine) { }

    public override void EnterState()
    {
        IsAttackFinished = false;
        timePassed = 0f;

        controller._animator.applyRootMotion = true;
        controller._animator.SetTrigger("attack");
        controller._animator.SetFloat("Speed", 0f);
    }

    public override void Update()
    {
        controller.HandleGroundedCheck();
        controller.HandleGravity();
        timePassed += Time.deltaTime;

        var stateInfo = controller._animator.GetCurrentAnimatorStateInfo(1);
        var clipInfo = controller._animator.GetCurrentAnimatorClipInfo(1);

        if (clipInfo.Length > 0)
        {
            clipLength = clipInfo[0].clip.length;
            clipSpeed = stateInfo.speed;

            if (timePassed >= (clipLength / clipSpeed))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    timePassed = 0f;
                    controller._animator.SetTrigger("attack");
                }
                else
                {
                    IsAttackFinished = true;
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
        controller._animator.applyRootMotion = false;
    }
}
