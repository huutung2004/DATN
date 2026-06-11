using UnityEngine;

public class ReturnState : IState
{
    private readonly EnemyBrain brain;

    public ReturnState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void EnterState()
    {
        brain.Agent.SetDestination(
            brain.SpawnPosition);
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}