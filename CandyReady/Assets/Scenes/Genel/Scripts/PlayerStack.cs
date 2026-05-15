using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStack : MonoBehaviour
{
    [Header("Stack Settings")]
    [SerializeField] Transform stackItemTranform;
    [SerializeField] public float itemHeight = 0.25f;
    [SerializeField] public int maxItems = 5;

    public List<GameObject> colletablesItem = new List<GameObject>();

    public bool CanCollect()
    {
        return colletablesItem.Count < maxItems;
    }

    public void CollectItem(GameObject item)
    {
        if (!CanCollect()) return;
        
        colletablesItem.Add(item);
        item.transform.SetParent(stackItemTranform);

        item.transform.localPosition = new Vector3(0, colletablesItem.Count * itemHeight, 0);
        item.transform.localRotation = Quaternion.identity;
    }

    public GameObject DropItem()
    {
        if (colletablesItem.Count == 0) return null;

        int lastIndex = colletablesItem.Count - 1;
        GameObject itemToDrop = colletablesItem[lastIndex];

        colletablesItem.RemoveAt(lastIndex);
        itemToDrop.transform.SetParent(null);

        return itemToDrop;
    }
}
