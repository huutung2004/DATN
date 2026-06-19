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
        attackTimer = AttackCooldown;
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
        }
    }

    public void FixedUpdate()
    {
    }

    public void Exit()
    {
    }
}