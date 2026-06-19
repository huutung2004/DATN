using UnityEngine;
using UnityEngine.AI;

public class PatrolState : IState
{
    private readonly EnemyBrain brain;

    public PatrolState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void EnterState()
    {

        brain.StartCoroutine(EnterNextFrame());
    }

    private System.Collections.IEnumerator EnterNextFrame()
    {
        yield return null;

        if (IsAgentReady())
            MoveToRandomPoint();
    }

    public void Update()
    {
        if (!IsAgentReady()) return;
        if (brain.Agent.pathPending) return;

        if (brain.Agent.remainingDistance <= 0.5f)
            MoveToRandomPoint();
    }

    public void FixedUpdate() { }

    public void Exit() { }
    private bool IsAgentReady()
    {
        return brain.Agent != null
            && brain.Agent.isActiveAndEnabled
            && brain.Agent.isOnNavMesh;
    }

    private void MoveToRandomPoint()
    {
        Vector3 point = RandomNavMeshPoint(brain.SpawnPosition, brain.PatrolRadius);
        brain.Agent.SetDestination(point);
    }

    private Vector3 RandomNavMeshPoint(Vector3 center, float radius)
    {
        for (int i = 0; i < 20; i++)
        {
            Vector3 randomPoint = center + Random.insideUnitSphere * radius;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, radius, NavMesh.AllAreas))
                return hit.position;
        }
        return center;
    }
}