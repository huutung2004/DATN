using UnityEngine;

public class JumpState : IState
{
    private readonly EnemyBrain brain;

    private Vector3 startPos;
    private Vector3 targetPos;

    private readonly float animDuration = 0.833f;

    private float timer;
    private bool  landed;

    public bool Finished { get; private set; }

    public JumpState(EnemyBrain brain) { this.brain = brain; }

    public void EnterState()
    {
        Finished = false;
        landed   = false;
        timer    = 0f;

        startPos = brain.transform.position;

        Vector3 aimPos = brain.Target      != null ? brain.Target.position
                       : brain.TowerTarget != null ? brain.TowerTarget.position
                       : startPos + brain.transform.forward * brain.JumpDistance;

        Vector3 dir = (aimPos - startPos);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.001f)
            brain.transform.rotation = Quaternion.LookRotation(dir);

        targetPos = startPos + dir.normalized * brain.JumpDistance;

        brain.Agent.enabled = false;

        if (brain.Animator != null)
            brain.Animator.SetTrigger("Jumping");
    }

    public void Update()
    {
        timer += Time.deltaTime;
        float t = Mathf.Clamp01(timer / animDuration);

        Vector3 pos = Vector3.Lerp(startPos, targetPos, t);
        pos.y += Mathf.Sin(t * Mathf.PI) * brain.JumpHeight;
        brain.transform.position = pos;

        if (t >= 1f && !landed)
        {
            landed   = true;
            Finished = true;

            brain.Agent.enabled = true;
            brain.Agent.Warp(brain.transform.position);
            brain.CurrentFence = null;
        }
    }

    public void FixedUpdate() { }

    public void Exit()
    {
        Finished = false;
        landed   = false;

        if (!brain.Agent.enabled)
        {
            brain.Agent.enabled = true;
            brain.Agent.Warp(brain.transform.position);
        }
    }
}