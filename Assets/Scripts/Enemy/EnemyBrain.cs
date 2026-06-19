using UnityEngine;
using UnityEngine.AI;

public class EnemyBrain : MonoBehaviour
{
    [Header("References")]
    public NavMeshAgent Agent;
    public Animator Animator;

    [Header("Stats")]
    public float MaxHp = 20f;
    public float CurrentHp;

    [Header("Detection")]
    public float DetectRange = 10f;
    public float AttackRange = 2f;
    public float LoseTargetRange = 15f;

    [Header("Patrol")]
    public float PatrolRadius = 10f;

    [Header("Fence")]
    public LayerMask FenceLayer;
    public float FenceSearchRadius = 3f;
    [Header("Jump")]
    public float JumpHeight = 2.5f;
    public float JumpDistance = 3f;

    [Header("Enemy Behaviour Type")]
    [Tooltip("Enemy này có thể đập phá hàng rào không?")]
    public bool CanAttackFence = true;

    [Tooltip("Enemy này có thể nhảy qua hàng rào không?")]
    public bool CanJumpFence = false;
    public bool HasOpenPath { get; set; }

    [HideInInspector] public Fence CurrentFence;
    [HideInInspector] public Transform Target;
    [HideInInspector] public Transform TowerTarget;
    [HideInInspector] public Vector3 SpawnPosition;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        SpawnPosition = transform.position;
        CurrentHp = MaxHp;

        if (Target == null)
        {
            var player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) Target = player.transform;
        }
    }

    private void Update()
    {
        if (Animator != null)
        {
            float speed = Agent.velocity.magnitude;
            Animator.SetFloat("Speed", speed);
        }
    }

    public void OnFootstep() { }

    public void StartDealDamage()
    {
        var dealer = GetComponentInChildren<DameDealer>();
        if (dealer) dealer.StartDealDamage();
    }

    public void EndDealDamage()
    {
        var dealer = GetComponentInChildren<DameDealer>();
        if (dealer) dealer.EndDealDamage();
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 origin = Application.isPlaying ? SpawnPosition : transform.position;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(origin, PatrolRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, DetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, AttackRange);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, LoseTargetRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, FenceSearchRadius);
    }
}