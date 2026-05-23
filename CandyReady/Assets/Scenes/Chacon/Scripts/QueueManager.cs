
using UnityEngine;
using System.Collections.Generic;


public class QueueManager : MonoBehaviour
{
    [SerializeField] Transform[] queuePositions;
    public List<NPC> npcQueue = new List<NPC>();

    void Start()
    {
        
    }

    public void AddToQueue(NPC npc)
    {
        npcQueue.Add(npc);
        UpdateQueuePositions();
    }

    public void UpdateQueuePositions()
    {
        for (int i = 0; i < npcQueue.Count; i++)
        {
            if (i >= queuePositions.Length)
            {
                break;
            }

            npcQueue[i].MoveToPosition(queuePositions[i].position);
        }
    }

    public void RemoveFromQueue(NPC npc)
    {
        npcQueue.Remove(npc);

        UpdateQueuePositions();
    }




    void Update()
    {
        
    }
}
