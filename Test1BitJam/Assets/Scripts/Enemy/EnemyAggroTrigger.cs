using UnityEngine;

public class EnemyAggroTrigger : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AgentAI agentAI = other.gameObject.GetComponent<AgentAI>();
            agentAI.StartChasing();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AgentAI agentAI = other.gameObject.GetComponent<AgentAI>();
            agentAI.StopChasing();
        }
    }
}
