using HutongGames.PlayMaker.Actions;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    [Header("Counter Settings")]
    [SerializeField] float dropRate = 0.15f;
    [SerializeField] Transform counterStackTranform;
    [SerializeField] public int maxCapacity = 24;

    [Header("Grid Settings")]
    [SerializeField] float itemHeight = 0.25f;
    [SerializeField] float spacingX = 0.6f;
    [SerializeField] float spacingZ = 0.6f;
    [SerializeField] int gridSizeX = 1;
    [SerializeField] int gridSizeZ = 1;

    private float dropTimer = 0f;

    private List<GameObject> itemsOnCounter = new List<GameObject>();

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStack playerStack = other.GetComponent<PlayerStack>();

            if (playerStack != null && playerStack.colletablesItem.Count > 0 && itemsOnCounter.Count < maxCapacity)
            {
                dropTimer += Time.deltaTime;

                if (dropTimer >= dropRate)
                {
                    GameObject pizzaDropped = playerStack.DropItem();

                    if (pizzaDropped != null)
                    {
                        PlaceItemOnCounter(pizzaDropped);
                    }

                    dropTimer = 0f;
                }
            }
        }
    }

    private void PlaceItemOnCounter(GameObject item)
    {
        int index = itemsOnCounter.Count;

        itemsOnCounter.Add(item);

        item.transform.SetParent(counterStackTranform);

        int x = index % gridSizeX;
        int y = index / (gridSizeZ * gridSizeX);
        int z = (index / gridSizeX) % gridSizeZ;

        Vector3 targetPosition = new Vector3(x * spacingX, y * itemHeight, z * spacingZ);

        item.transform.localPosition = targetPosition;

        item.transform.localRotation = Quaternion.identity;
    }

    public GameObject TakeItem()
    {
        if (itemsOnCounter.Count <= 0)
        {
            return null;
        }

        int lastIndex = itemsOnCounter.Count - 1;

        GameObject item = itemsOnCounter[lastIndex];

        itemsOnCounter.RemoveAt(lastIndex);

        UpdateCounterVisual();

        return item;
    }

    void UpdateCounterVisual()
    {
        for (int index = 0; index < itemsOnCounter.Count; index++)
        {
            GameObject item = itemsOnCounter[index];

            int x = index % gridSizeX;
            int y = index / (gridSizeZ * gridSizeX);
            int z = (index / gridSizeX) % gridSizeZ;

            Vector3 targetPosition = new Vector3(x * spacingX, y * itemHeight, z * spacingZ);

            item.transform.localPosition = targetPosition;
        }
    }

    public int GetPizzaCount()
    {
        return itemsOnCounter.Count;
    }
}