using UnityEngine;

public class EnemyStateMachineInitializer : MonoBehaviour
{
    private StateMachine stateMachine;

    private PatrolState patrol;
    private ChaseState chase;
    private EnemyAttackState attack;
    private ReturnState returnState;

    private EnemyBrain brain;

    private void Awake()
    {
        brain = GetComponent<EnemyBrain>();

        stateMachine = new StateMachine();

        patrol = new PatrolState(brain);
        chase = new ChaseState(brain);
        attack = new EnemyAttackState(brain);
        returnState = new ReturnState(brain);

        stateMachine.AddTransition(
            patrol,
            chase,
            new FuncPredicate(IsPlayerDetected));

        stateMachine.AddTransition(
            chase,
            attack,
            new FuncPredicate(InAttackRange));

        stateMachine.AddTransition(
            attack,
            chase,
            new FuncPredicate(PlayerOutAttackRange));

        stateMachine.AddTransition(
            chase,
            returnState,
            new FuncPredicate(LostTarget));

        stateMachine.AddTransition(
            attack,
            returnState,
            new FuncPredicate(LostTarget));

        stateMachine.AddTransition(
            returnState,
            patrol,
            new FuncPredicate(ReturnCompleted));

        stateMachine.SetState(patrol);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    private bool IsPlayerDetected()
    {
        if (brain.Target == null)
            return false;

        return Vector3.Distance(
            transform.position,
            brain.Target.position)
            <= brain.DetectRange;
    }

    private bool InAttackRange()
    {
        if (brain.Target == null)
            return false;

        return Vector3.Distance(
            transform.position,
            brain.Target.position)
            <= brain.AttackRange;
    }

    private bool PlayerOutAttackRange()
    {
        if (brain.Target == null)
            return true;

        return Vector3.Distance(
            transform.position,
            brain.Target.position)
            > brain.AttackRange;
    }

    private bool LostTarget()
    {
        if (brain.Target == null)
            return true;

        return Vector3.Distance(
            transform.position,
            brain.Target.position)
            > brain.LoseTargetRange;
    }

    private bool ReturnCompleted()
    {
        return !brain.Agent.pathPending &&
               brain.Agent.remainingDistance <= 0.5f;
    }
}