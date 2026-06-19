using UnityEngine;

public class ChaseState : IState
{
    private readonly EnemyBrain brain;

    public ChaseState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void EnterState() { }

    public void Update()
    {
        if (!brain.Agent.isActiveAndEnabled ||
            !brain.Agent.isOnNavMesh)
            return;
        if (brain.CurrentFence != null)
        {
            brain.Agent.SetDestination(brain.CurrentFence.transform.position);
            return;
        }

        if (brain.TowerTarget != null &&
            brain.Target != null == false)
        {
            brain.Agent.SetDestination(brain.TowerTarget.position);
            return;
        }

        if (brain.Target != null)
            brain.Agent.SetDestination(brain.Target.position);
    }

    public void FixedUpdate() { }

    public void Exit() { }
}