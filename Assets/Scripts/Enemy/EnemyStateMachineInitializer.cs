using UnityEngine;
using UnityEngine.AI;

public class EnemyStateMachineInitializer : MonoBehaviour
{
    private StateMachine stateMachine;

    private PatrolState patrolState;
    private ChaseState chaseState;
    private EnemyAttackState attackPlayerState;
    private AttackTowerState attackTowerState;
    private AttackFenceState attackFenceState;
    private JumpState jumpState;
    private ReturnState returnState;

    private EnemyBrain brain;

    [Header("Debug")]
    [SerializeField] private bool showDebugLog = true;
    [SerializeField] private bool showDebugGizmo = true;

    [Header("Path Check")]
    [SerializeField] private float pathCheckInterval = 0.3f;

    [Header("Fence Cooldown")]
    [Tooltip("Sau khi bỏ fence, enemy không quay lại đánh fence trong N giây")]
    [SerializeField] private float fenceAbandonCooldown = 2f;

    // ── Runtime ────────────────────────────────────────────────────────────────
    private EnemyIntent frameIntent = EnemyIntent.NoTarget;
    private string currentStateName = "None";
    private EnemyIntent lastIntent = EnemyIntent.NoTarget;

    private float pathCheckTimer = 0f;
    private bool hasOpenPath = false;
    private Vector3 lastPathTarget = Vector3.positiveInfinity;

    // Cooldown chống flip-flop AttackFence ↔ Chase
    private float fenceCooldownTimer = 0f;
    private bool inFenceCooldown => fenceCooldownTimer > 0f;

    // ── Lifecycle ──────────────────────────────────────────────────────────────
    private void Awake()
    {
        brain = GetComponent<EnemyBrain>();
        stateMachine = new StateMachine();

        patrolState = new PatrolState(brain);
        chaseState = new ChaseState(brain);
        attackPlayerState = new EnemyAttackState(brain);
        attackTowerState = new AttackTowerState(brain);
        attackFenceState = new AttackFenceState(brain);
        jumpState = new JumpState(brain);
        returnState = new ReturnState(brain);

        RegisterTransitions();
        stateMachine.SetState(patrolState);
    }

    private void Update()
    {
        UpdatePathCheck();

        frameIntent = TargetResolver.Resolve(brain);

        UpdateFenceCooldown();

        stateMachine.Update();
        DebugLog();
    }

    private void UpdateFenceCooldown()
    {
        if (fenceCooldownTimer > 0f)
            fenceCooldownTimer -= Time.deltaTime;
    }

    private void StartFenceCooldown()
    {
        fenceCooldownTimer = fenceAbandonCooldown;
    }

    private void UpdatePathCheck()
    {

        pathCheckTimer += Time.deltaTime;
        if (pathCheckTimer < pathCheckInterval) return;
        pathCheckTimer = 0f;

        if (!TryGetPriorityTargetPos(out Vector3 targetPos))
        {
            hasOpenPath = false;
            brain.HasOpenPath = false;
            return;
        }

        if (Vector3.Distance(targetPos, lastPathTarget) < 0.5f) return;
        lastPathTarget = targetPos;

        var path = new NavMeshPath();
        bool newResult = NavMesh.CalculatePath(
            transform.position, targetPos,
            NavMesh.AllAreas, path) &&
            path.status == NavMeshPathStatus.PathComplete;

        if (newResult != hasOpenPath && showDebugLog)
            Debug.Log($"[{gameObject.name}] Path: {(newResult ? "<color=lime>OPEN</color>" : "<color=red>BLOCKED</color>")}");

        hasOpenPath = newResult;
        brain.HasOpenPath = newResult;
    }

    private bool TryGetPriorityTargetPos(out Vector3 pos)
    {
        if (brain.Target != null)
        {
            pos = brain.Target.position;
            return true;
        }

        if (brain.TowerTarget != null)
        {
            pos = brain.TowerTarget.position;
            return true;
        }

        pos = Vector3.zero;
        return false;
    }

    private void RegisterTransitions()
    {
        stateMachine.AddTransition(patrolState, chaseState,
            new FuncPredicate(HasAnyTarget));

        stateMachine.AddTransition(chaseState, attackPlayerState,
            new FuncPredicate(CanAttackPlayer));

        stateMachine.AddTransition(chaseState, attackTowerState,
            new FuncPredicate(CanAttackTower));

        // Chase → AttackFence: chỉ khi KHÔNG trong cooldown
        stateMachine.AddTransition(chaseState, jumpState,
        new FuncPredicate(ShouldJumpFence));

        stateMachine.AddTransition(chaseState, attackFenceState,
            new FuncPredicate(ShouldAttackFence));

        stateMachine.AddTransition(attackPlayerState, chaseState,
            new FuncPredicate(PlayerOutAttackRange));
        stateMachine.AddTransition(attackPlayerState, returnState,
            new FuncPredicate(LostAllTargets));

        stateMachine.AddTransition(attackTowerState, chaseState,
            new FuncPredicate(ShouldLeaveTower));
        stateMachine.AddTransition(attackTowerState, returnState,
            new FuncPredicate(LostAllTargets));

        // AttackFence → Chase
        stateMachine.AddTransition(attackFenceState, chaseState,
            new FuncPredicate(ShouldAbandonFence));

        stateMachine.AddTransition(jumpState, chaseState,
            new FuncPredicate(() => jumpState.Finished));

        stateMachine.AddTransition(chaseState, returnState,
            new FuncPredicate(LostAllTargets));

        stateMachine.AddTransition(returnState, patrolState,
            new FuncPredicate(ReturnCompleted));
        stateMachine.AddTransition(returnState, chaseState,
            new FuncPredicate(HasAnyTarget));
    }


    private bool HasAnyTarget()
        => frameIntent != EnemyIntent.NoTarget;

    private bool CanAttackPlayer()
    {
        if (brain.Target == null) return false;
        if (frameIntent != EnemyIntent.ChasePlayer) return false;
        return Vector3.Distance(transform.position, brain.Target.position) <= brain.AttackRange;
    }

    private bool CanAttackTower()
    {
        if (brain.TowerTarget == null) return false;
        if (frameIntent != EnemyIntent.ChaseTower) return false;
        return Vector3.Distance(transform.position, brain.TowerTarget.position) <= brain.AttackRange;
    }

    private bool ShouldAttackFence()
    {
        if (inFenceCooldown) return false;
        if (!brain.CanAttackFence || brain.CurrentFence == null) return false;
        if (hasOpenPath) return false;
        if (brain.CanJumpFence) return false;


        return frameIntent == EnemyIntent.FenceBlockingPlayer ||
               frameIntent == EnemyIntent.FenceBlockingTower ||
               frameIntent == EnemyIntent.FenceOnly;
    }

    private bool ShouldJumpFence()
    {
        if (inFenceCooldown) return false;
        if (!brain.CanJumpFence || brain.CurrentFence == null) return false;

        return frameIntent == EnemyIntent.FenceBlockingPlayer ||
               frameIntent == EnemyIntent.FenceBlockingTower ||
               frameIntent == EnemyIntent.FenceOnly;
    }

    private bool ShouldAbandonFence()
    {
        if (brain.CurrentFence == null)
        {
            StartFenceCooldown();
            return true;
        }

        if (Vector3.Distance(transform.position,
            brain.CurrentFence.transform.position) > brain.AttackRange)
        {
            StartFenceCooldown();
            return true;
        }
        if (brain.CurrentFence == null)
        {
            StartFenceCooldown();
            return true;
        }

        if (hasOpenPath)
        {
            StartFenceCooldown();
            return true;
        }

        if (frameIntent == EnemyIntent.ChasePlayer ||
            frameIntent == EnemyIntent.ChaseTower)
        {
            StartFenceCooldown();
            return true;
        }

        return false;
    }
    private bool PlayerOutAttackRange()
    {
        if (brain.Target == null) return true;
        return Vector3.Distance(transform.position, brain.Target.position) > brain.AttackRange;
    }

    private bool ShouldLeaveTower()
    {
        if (brain.TowerTarget == null)
            return true;

        if (Vector3.Distance(transform.position, brain.TowerTarget.position) > brain.AttackRange)
            return true;

        if (brain.Target != null &&
            Vector3.Distance(transform.position, brain.Target.position) <= brain.DetectRange)
            return true;

        return false;
    }
    private bool LostAllTargets()
        => frameIntent == EnemyIntent.NoTarget;

    private bool ReturnCompleted()
        => !brain.Agent.pathPending && brain.Agent.remainingDistance <= 0.5f;

    // ── Debug ──────────────────────────────────────────────────────────────────
    private void DebugLog()
    {
        string stateName = stateMachine.CurrentStateName;
        if (!showDebugLog) { currentStateName = stateName; lastIntent = frameIntent; return; }

        if (stateName != currentStateName || frameIntent != lastIntent)
        {
            string fence = brain.CurrentFence != null ? brain.CurrentFence.name : "null";
            string player = brain.Target != null
                ? $"{Vector3.Distance(transform.position, brain.Target.position):F1}m" : "null";
            string cd = inFenceCooldown ? $"<color=orange>CD={fenceCooldownTimer:F1}s</color>" : "CD=off";

            Debug.Log(
                $"[{gameObject.name}] " +
                $"<color=yellow>{currentStateName}</color>→<color=cyan>{stateName}</color> | " +
                $"<color=orange>{lastIntent}</color>→<color=lime>{frameIntent}</color> | " +
                $"Player={player} Fence={fence} Path={(hasOpenPath ? "<color=lime>OPEN</color>" : "<color=red>BLK</color>")} {cd}",
                gameObject);

            currentStateName = stateName;
            lastIntent = frameIntent;
        }
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmo || brain == null) return;

#if UNITY_EDITOR
        string cdText = inFenceCooldown ? $"\nCD: {fenceCooldownTimer:F1}s" : "";
        UnityEditor.Handles.color = inFenceCooldown ? Color.yellow : (hasOpenPath ? Color.green : Color.red);
        UnityEditor.Handles.Label(
            transform.position + Vector3.up * 2.5f,
            $"{currentStateName}\n{frameIntent}\nPath:{(hasOpenPath ? "OPEN" : "BLK")}{cdText}");
#endif

        if (brain.CurrentFence != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, brain.CurrentFence.transform.position);
        }

        if (brain.Target != null)
        {
            bool los = !Physics.Raycast(
                transform.position + Vector3.up * 0.5f,
                (brain.Target.position - transform.position).normalized,
                Vector3.Distance(transform.position, brain.Target.position),
                brain.FenceLayer);
            Gizmos.color = los ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, brain.Target.position);
        }
    }
}