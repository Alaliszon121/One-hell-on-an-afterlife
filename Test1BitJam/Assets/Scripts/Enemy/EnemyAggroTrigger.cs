using UnityEngine;

public class EnemyAggroTrigger : MonoBehaviour
{
    private SphereCollider sphereCollider;

    void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            
            AgentAI agentAI = other.gameObject.GetComponentInChildren<AgentAI>();
            
            if (agentAI.isChasing) return;
            
            Vector3 target = other.transform.position;
            Vector3 direction = (target - transform.position).normalized;
            float distance = Vector3.Distance(transform.position, target);

            if (Physics.Raycast(transform.position, direction,out var hit, distance))
            {
                Debug.Log(hit.collider.gameObject.name);
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
            AgentAI agentAI = other.gameObject.GetComponentInChildren<AgentAI>();
            agentAI.StopChasing();
            agentAI.StopEnemyParameter(false);
        }
    }
}
