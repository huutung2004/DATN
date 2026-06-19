using UnityEngine;

public class AttackFenceState : IState
{
    private readonly EnemyBrain brain;

    private float attackTimer;
    private const float AttackCooldown = 1f;

    public AttackFenceState(EnemyBrain enemyBrain)
    {
        brain = enemyBrain;
    }

    public void EnterState()
    {
        brain.Agent.ResetPath();
       attackTimer = AttackCooldown;
    }

    public void Update()
    {
        if (brain.CurrentFence == null)
            return;

        brain.transform.LookAt(brain.CurrentFence.transform);

        attackTimer += Time.deltaTime;

        if (attackTimer >= AttackCooldown)
        {
            attackTimer = 0;
            brain.Animator.SetTrigger("attack");
        }
    }

    public void FixedUpdate() { }

    public void Exit() { }
}