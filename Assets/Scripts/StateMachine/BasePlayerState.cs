using StarterAssets;
using UnityEngine;
public abstract class BasePlayerState : IState
{
    protected readonly ThirdPersonController controller;
    protected readonly StateMachine stateMachine;

    protected BasePlayerState(ThirdPersonController controller, StateMachine stateMachine)
    {
        this.controller = controller;
        this.stateMachine = stateMachine;
    }
    public virtual void EnterState()
    {
    }

    public virtual void Exit()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void Update()
    {
        controller.HandleGroundedCheck();
        controller.HandleGravity();
        controller.HandleMove();
    }
}
