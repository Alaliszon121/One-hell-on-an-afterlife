using UnityEngine;

public class EnemyAggroTrigger : MonoBehaviour
{
    [Header("Skrypt AgentAI przeciwnika")]
    [SerializeField] AgentAI agentAI;
    
    private EnemyChase _enemyChase;
    
    private void Awake()
    {
        _enemyChase = GetComponentInParent<EnemyChase>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agentAI.isChasing = true;
            _enemyChase.StartChasing(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            agentAI.isChasing = false;
            _enemyChase.StopChasing();
        }
    }
}
