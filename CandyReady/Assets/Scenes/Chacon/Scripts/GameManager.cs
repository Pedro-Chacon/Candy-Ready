using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField] QueueManager queueManager;

    void Start()
    {
        
    }


    void Update()
    {
    if (queueManager.npcQueue.Count <= 0)
        {
            print("nao tem npcs na cena");
        }    
    }



}
