using UnityEngine;

public class Counter : MonoBehaviour
{
    [Header("Counter Settings")]
    [SerializeField] float dropRate = 0.15f;

    private float dropTimer = 0f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStack playerStack = other.GetComponent<PlayerStack>();

            if (playerStack != null && playerStack.colletablesItem.Count > 0)
            {
                dropTimer += Time.deltaTime;

                if (dropTimer >= dropRate)
                {
                    GameObject pizzaDropped = playerStack.DropItem();

                    if (pizzaDropped != null)
                    {
                        Destroy(pizzaDropped);
                        Player.moneyScore += 75;
                        print("Money: " + Player.moneyScore);
                    }

                    dropTimer = 0f;
                }
            }
        }
    }
}
