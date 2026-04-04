using UnityEngine;

public class EnemyAggroTrigger : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            AgentAI agentAI = other.gameObject.GetComponent<AgentAI>();
            if (agentAI.isChasing) return;
            
            Vector3 target = other.transform.position;
            Vector3 direction = (target - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target);

            if (Physics.Raycast(transform.position, direction,out var hit, distance))
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    agentAI.StartChasing();
                }
            }
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
