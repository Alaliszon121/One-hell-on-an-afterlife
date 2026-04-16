using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class AgentAI : MonoBehaviour
{
    [Header("Wybrany waypoint")]
    [SerializeField] public int currentWaypoint = 0;
    
    [Header("Animator przeciwnika")]
    [SerializeField] private Animator animator;

    [Header("Jak szybko obraca się przeciwnik")]
    [SerializeField] private float rotationSpeed = 4f;

    [Header("Obiekt do rotacji")] 
    [SerializeField] private Transform enemyObject;
    
    
    
    private List<Transform> wayPoints;
    private Transform player;
    private PlayerStateManager playerStateManager;
    
    private NavMeshAgent navMeshAgent;
    public bool isChasing = false;
    
    void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    
    private void Start()
    {
        playerStateManager.OnStateChanged += SetEnemyLightParameter;
    }

    // Update is called once per frame
    void Update()
    {
        navMeshAgent.gameObject.transform.position = enemyObject.position;
        if (wayPoints == null || player == null || playerStateManager == null) return;
        if(!isChasing) Walking();
        else
        {
            float distanceToPlayer = Vector3.Distance(player.position, transform.position);
            
            if (distanceToPlayer <= 3.5 && !animator.GetBool("Stop")) StopEnemyParameter(true);
            
            if (distanceToPlayer <= 2)
            {
                Vector3 spawnPointPosition = new  Vector3(GameManager.instance.spawnPoint.position.x, player.position.y, GameManager.instance.spawnPoint.position.z);
                player.position = spawnPointPosition;
                AudioManager.instance.PLaySFX(AudioManager.instance.boneSnap);
            }

            if (navMeshAgent.pathStatus == NavMeshPathStatus.PathInvalid)
            {
                Walking();
                StopChasing();
            }
            else navMeshAgent.SetDestination(player.position);
        }
        
        Vector3 direction = navMeshAgent.desiredVelocity;
        direction.y = 0f;

        if (direction.magnitude > 0.1f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            enemyObject.rotation = Quaternion.Slerp(enemyObject.rotation, targetRotation, rotationSpeed * Time.deltaTime);
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
    
    public void SetVariables(List<Transform> wayPoints, Transform player, PlayerStateManager playerStateManager)
    {
        this.wayPoints = wayPoints;
        this.player = player;
        this.playerStateManager = playerStateManager;
    }

    void SetEnemyLightParameter(PlayerColorState state)
    {
        if((int)state == 2) animator.SetBool("Light", true);
        else
        {
            StopEnemyParameter(false);
            animator.SetBool("Light", false);
        }
    }
    
    public void StopEnemyParameter(bool state)
    {
        animator.SetBool("Stop", state);
        Debug.Log(state);
    }
}
