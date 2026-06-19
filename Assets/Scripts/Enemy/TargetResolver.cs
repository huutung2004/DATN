using UnityEngine;

public static class TargetResolver
{
    public static EnemyIntent Resolve(EnemyBrain brain)
    {
        if (brain.CurrentFence == null)
        {
            brain.CurrentFence = null;
        }

        if (brain.TowerTarget == null)
        {
            brain.TowerTarget = null;
        }

        if (brain.Target != null)
        {
            float distToPlayer = Vector3.Distance(brain.transform.position, brain.Target.position);

            if (distToPlayer > brain.LoseTargetRange)
            {
                brain.Target = null;
            }
        }
        if (brain.Target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                float d = Vector3.Distance(brain.transform.position, player.transform.position);
                if (d <= brain.DetectRange)
                    brain.Target = player.transform;
            }
        }
        if (brain.Target != null)
        {
            float distToPlayer = Vector3.Distance(brain.transform.position, brain.Target.position);
            if (distToPlayer <= brain.AttackRange)
            {
                brain.CurrentFence = null;
                return EnemyIntent.ChasePlayer;
            }
        }

        if (brain.Target != null && IsVisible(brain, brain.Target))
        {
            brain.CurrentFence = null;
            return EnemyIntent.ChasePlayer;
        }

        if (brain.Target != null && brain.HasOpenPath)
        {
            brain.CurrentFence = null;
            return EnemyIntent.ChasePlayer;
        }

        if (brain.Target != null && !brain.HasOpenPath)
        {
            Fence fenceBlockingPlayer = GetFenceBlocking(brain, brain.Target.position);
            if (fenceBlockingPlayer != null)
            {
                brain.CurrentFence = fenceBlockingPlayer;
                return EnemyIntent.FenceBlockingPlayer;
            }
        }

        TowerBase tower = FindNearestTower(brain);

        if (tower == null)
        {
            brain.TowerTarget = null;
        }
        else
        {
            brain.TowerTarget = tower.transform;

            float distToTower = Vector3.Distance(
                brain.transform.position,
                tower.transform.position);

            if (distToTower <= brain.AttackRange)
            {
                brain.CurrentFence = null;
                return EnemyIntent.ChaseTower;
            }

            if (IsVisible(brain, tower.transform))
            {
                brain.CurrentFence = null;
                return EnemyIntent.ChaseTower;
            }

            if (!brain.HasOpenPath)
            {
                Fence fenceBlockingTower =
                    GetFenceBlocking(brain, tower.transform.position);

                if (fenceBlockingTower != null)
                {
                    brain.CurrentFence = fenceBlockingTower;
                    return EnemyIntent.FenceBlockingTower;
                }
            }
        }

        Fence nearestFence = FindNearestFence(brain);

        if (nearestFence == null)
        {
            brain.CurrentFence = null;
        }
        else
        {
            brain.CurrentFence = nearestFence;
            return EnemyIntent.FenceOnly;
        }

        brain.CurrentFence = null;
        brain.TowerTarget = null;
        return EnemyIntent.NoTarget;
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static bool IsVisible(EnemyBrain brain, Transform target)
    {
        Vector3 start = brain.transform.position + Vector3.up * 0.5f;
        Vector3 dir = (target.position - start).normalized;
        float dist = Vector3.Distance(brain.transform.position, target.position);
        return !Physics.Raycast(start, dir, dist, brain.FenceLayer);
    }

    private static Fence GetFenceBlocking(EnemyBrain brain, Vector3 targetPos)
    {
        Vector3 start = brain.transform.position + Vector3.up * 0.5f;
        Vector3 dir = (targetPos - start).normalized;
        float dist = Vector3.Distance(brain.transform.position, targetPos);

        if (Physics.Raycast(start, dir, out RaycastHit hit, dist, brain.FenceLayer))
            return hit.collider.GetComponent<Fence>();

        return null;
    }

    private static TowerBase FindNearestTower(EnemyBrain brain)
    {
        TowerBase[] all = Object.FindObjectsOfType<TowerBase>();
        TowerBase best = null;
        float nearest = brain.DetectRange;

        foreach (TowerBase t in all)
        {
            float d = Vector3.Distance(brain.transform.position, t.transform.position);
            if (d < nearest) { nearest = d; best = t; }
        }
        return best;
    }

    private static Fence FindNearestFence(EnemyBrain brain)
    {
        Collider[] hits = Physics.OverlapSphere(
            brain.transform.position, brain.FenceSearchRadius, brain.FenceLayer);

        Fence best = null;
        float nearest = float.MaxValue;

        foreach (Collider c in hits)
        {
            Fence fence = c.GetComponent<Fence>();
            if (fence == null) continue;
            float d = Vector3.Distance(brain.transform.position, fence.transform.position);
            if (d < nearest) { nearest = d; best = fence; }
        }
        return best;
    }
}

public enum EnemyIntent
{
    NoTarget,
    ChasePlayer,
    ChaseTower,
    FenceBlockingPlayer,
    FenceBlockingTower,
    FenceOnly
}