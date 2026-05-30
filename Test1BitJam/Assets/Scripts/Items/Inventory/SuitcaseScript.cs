using System.Collections.Generic;
using UnityEngine;

public class SuitcaseScript : MonoBehaviour
{
    public List<InventoryItemScript> itemsInSuitcase = new List<InventoryItemScript>();
    public Dictionary<Vector2Int, ItemType> spaceInSuitcase =  new Dictionary<Vector2Int, ItemType>();

    public void DestroyWorldItemsInSuitCase()
    {
        foreach (var item in itemsInSuitcase)
        {
            item.DestroyItemInWorld();
        }
    }

    public void TryToAddToSuitcase(InventoryItemScript item)
    {
        if(!itemsInSuitcase.Contains(item)) 
        {
            itemsInSuitcase.Add(item);
        }
    }

    public void TryToRemoveFromSuitcase(InventoryItemScript item)
    {
        if(itemsInSuitcase.Contains(item)) 
        {
            itemsInSuitcase.Remove(item);
        }

        return;
    }
}
