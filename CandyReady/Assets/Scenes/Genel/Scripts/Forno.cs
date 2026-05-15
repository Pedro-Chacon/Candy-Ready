using System.Collections;
using UnityEngine;

public class Forno : MonoBehaviour
{
    public GameObject pizzaPrefab;
    public int maxPizzasInOven = 10;
    public float timeToBake = 1f; 
    public float transferRate = 0.15f; 

    private int currentPizzasReady = 0;
    private float bakeTimer = 0f;
    private float transferTimer = 0f;

    void Update()
    {
        if (currentPizzasReady < maxPizzasInOven)
        {
            bakeTimer += Time.deltaTime;
            if (bakeTimer >= timeToBake)
            {
                currentPizzasReady++;
                bakeTimer = 0f;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && currentPizzasReady > 0)
        {
            transferTimer += Time.deltaTime;

            if (transferTimer >= transferRate)
            {
                PlayerStack playerStack = other.GetComponent<PlayerStack>();

                if (playerStack != null && playerStack.CanCollect())
                {
                    currentPizzasReady--;
                    GameObject newPizza = Instantiate(pizzaPrefab);
                    playerStack.CollectItem(newPizza);

                    transferTimer = 0f;
                }
            }
        }
    }
}
