using UnityEngine;

public class EnemyAttackState : IState
{
    private readonly EnemyBrain brain;

    private float attackTimer;
    private const float AttackCooldown = 1f;

    public EnemyAttackState(EnemyBrain brain)
    {
        this.brain = brain;
    }

    public void EnterState()
    {
        brain.Agent.ResetPath();
        attackTimer = 0;
    }

    public void Update()
    {
        if (brain.Target == null)
            return;

        attackTimer += Time.deltaTime;

        if (attackTimer >= AttackCooldown)
        {
            attackTimer = 0;
            brain.Animator.SetTrigger("attack");
            Debug.Log("Attack");
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}