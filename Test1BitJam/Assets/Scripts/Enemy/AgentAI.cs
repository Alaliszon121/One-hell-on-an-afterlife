using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAI : MonoBehaviour
{
    [Header("Wybrany waypoint")]
    [SerializeField] public int currentWaypoint = 0;
    
    [Header("Animator przeciwnika")]
    [SerializeField] private Animator animator;
    
    private List<Transform> wayPoints;
    private Transform player;
    
    private NavMeshAgent navMeshAgent;
    public bool isChasing = false;
    
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (wayPoints == null || player == null) return;
        if(!isChasing) Walking();
        else
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            if (distanceToPlayer <= 2)
            {
                Vector3 spawnPointPosition = new  Vector3(GameManager.instance.spawnPoint.position.x, player.position.y, GameManager.instance.spawnPoint.position.z);
                player.position = spawnPointPosition;
            }
            else navMeshAgent.SetDestination(player.position);
        }
    }

    void Walking()
    {
        if(wayPoints.Count == 0) return;
        
        float distanceToWaypoint = Vector3.Distance(wayPoints[currentWaypoint].position, transform.position);

        if (distanceToWaypoint <= 1) currentWaypoint = (currentWaypoint + 1) % wayPoints.Count;
        
        navMeshAgent.SetDestination(wayPoints[currentWaypoint].position);
        
    }
    
    public void StartChasing()
    {
        isChasing = true;
        navMeshAgent.speed = GameManager.instance.runSpeed;
        animator.SetBool("Spotted", true);
    }
    
    public void StopChasing()
    {
        isChasing = false;
        navMeshAgent.speed = GameManager.instance.walkSpeed;
        animator.SetBool("Spotted", false);
    }
    
    public void SetVariables(List<Transform> wayPoints, Transform player)
    {
        this.wayPoints = wayPoints;
        this.player = player;
    }
}
