using UnityEngine;

public class AttackTowerState : IState
{
    private readonly EnemyBrain brain;

    private float attackTimer;
    private const float AttackCooldown = 1.2f;

    public AttackTowerState(EnemyBrain brain)
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
        if (brain.TowerTarget == null)
        {
            brain.Animator.ResetTrigger("attack");
            return;
        }

        brain.transform.LookAt(brain.TowerTarget);

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