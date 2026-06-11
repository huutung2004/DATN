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

    [HideInInspector]
    public Transform Target;

    [HideInInspector]
    public Vector3 SpawnPosition;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        Animator = GetComponent<Animator>();

        SpawnPosition = transform.position;
        CurrentHp = MaxHp;
        if (Target == null)
        {
            Target = GameObject.FindGameObjectWithTag("Player").transform;
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

    public void OnFootstep()
    {

    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(
            Application.isPlaying
                ? SpawnPosition
                : transform.position,
            PatrolRadius);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(
            transform.position,
            DetectRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(
            transform.position,
            AttackRange);

        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(
            transform.position,
            LoseTargetRange);
    }
    public void StartDealDamage()
    {

        var dameDealer = gameObject.GetComponentInChildren<DameDealer>();
        if (dameDealer)
        {
            dameDealer.StartDealDamage();
        }
    }
    public void EndDealDamage()
    {

        var dameDealer = gameObject.GetComponentInChildren<DameDealer>();
        if (dameDealer)
        {
            dameDealer.EndDealDamage();
        }
    }
}