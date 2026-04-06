using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Spawn Info")]
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private Transform enemyContainer;
    [SerializeField] private int spawnAmount = 1;
    [SerializeField] private float spawnRate = 20f;
    
    [Header("Transform gracza")]
    [SerializeField] private Transform player;
    
    [Header("Waypointy")]
    [SerializeField] private List<Transform> wayPoints;

    [Header("Którego przeciwnika ma najpierw zrespić (0 lub 1)")]
    [SerializeField] private int idx = 0;

    private float time;
    private int count = 0;
    
    
    void  Start()
    {
        SpawnEnemy();
    }

    void Update()
    {
       time += Time.deltaTime;
       if (time > spawnRate && count < spawnAmount)
       {
           SpawnEnemy();
           time = 0;
       }
    }

    void SpawnEnemy()
    {
        var newEnemy = Instantiate(enemyPrefab[idx], enemyContainer);
        AgentAI newEnemyAgentAI = newEnemy.GetComponentInChildren<AgentAI>();
        newEnemyAgentAI.SetVariables(wayPoints,  player);
        idx = (idx + 1) % 2;
        count++;
    }
}
