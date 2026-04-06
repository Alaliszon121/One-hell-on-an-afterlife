using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Info")]
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private Transform enemyContainer;
    
    [Header("Transform gracza")]
    [SerializeField] private Transform player;
    
    [Header("Waypointy")]
    [SerializeField] private List<Transform> wayPoints;

    [Header("Którego przeciwnika ma najpierw zrespić (0 lub 1)")]
    [SerializeField] private int idx = 0;

    private float time;
    
    
    void  Start()
    {
        SpawnEnemy();
    }

    void Update()
    {
       /*time += Time.deltaTime;
       if (time > 10)
       {
           SpawnEnemy();
           time = 0;
       }*/
    }

    void SpawnEnemy()
    {
        var newEnemy = Instantiate(enemyPrefab[idx], enemyContainer);
        AgentAI newEnemyAgentAI = newEnemy.GetComponentInChildren<AgentAI>();
        newEnemyAgentAI.SetVariables(wayPoints,  player);
        idx = (idx + 1) % 2;
    }
}
