using UnityEngine;
using UnityEngine.AI;
public class NPC : MonoBehaviour
{
    [SerializeField] Transform seatPosition;

    private NavMeshAgent navMeshAgent;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    void Start()
    {
        
    }


    void Update()
    {
        navMeshAgent.destination = seatPosition.position;
    }
}
