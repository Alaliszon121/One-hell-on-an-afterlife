using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AgentAI : MonoBehaviour
{
    [Header("Wszyskie waypointy")]
    [SerializeField] public List<Transform> wayPoints;
    
    [Header("Wybrany waypoint")]
    [SerializeField] public int currentWaypoint = 0;
    
    private NavMeshAgent _navMeshAgent;
    public bool isChasing = false;
    
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if(!isChasing) Walking();
    }

    void Walking()
    {
        if(wayPoints.Count == 0) return;
        
        float distanceToWaypoint = Vector3.Distance(wayPoints[currentWaypoint].position, transform.position);

        if (distanceToWaypoint <= 1) currentWaypoint = (currentWaypoint + 1) % wayPoints.Count;
        
        _navMeshAgent.SetDestination(wayPoints[currentWaypoint].position);
        
    }
}
