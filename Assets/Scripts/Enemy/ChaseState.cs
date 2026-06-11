using UnityEngine;

public class ChaseState : IState
{
    private readonly EnemyBrain brain;

    public ChaseState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void EnterState()
    {
    }

    public void Update()
    {
        if (brain.Target == null)
            return;

        brain.Agent.SetDestination(
            brain.Target.position);
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}