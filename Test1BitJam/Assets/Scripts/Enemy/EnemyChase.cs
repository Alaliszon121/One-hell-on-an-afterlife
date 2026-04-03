using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 1f;
    [SerializeField] private float runSpeed = 3f;
    
    [Header("Nav mesh przeciwnika")]
    [SerializeField] private NavMeshAgent navMeshAgent;
    
    private Transform player = null;
    
    [Header("Animator przeciwnika")]
    [SerializeField] private Animator animator;
    
    public bool isChasing = false;

    private void Update()
    {
        if (!isChasing || player == null) return;
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);
        navMeshAgent.SetDestination(player.position);
    }

    public void StartChasing(Transform target)
    {
        player = target;
        isChasing = true;
        navMeshAgent.speed = runSpeed;
        animator.SetBool("Spotted", true);
    }

    public void StopChasing()
    {
        player = null;
        isChasing = false;
        navMeshAgent.speed = walkSpeed;
        animator.SetBool("Spotted", false);
    }
}
