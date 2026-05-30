using System.Collections.Generic;
using UnityEngine;

namespace Items.Inventory
{
    public class InventoryItemPlaceholder : MonoBehaviour
    {
        [SerializeField] public List<InventoryItemScript> itemsNotInSuitcase;

        private void Awake()
        {
            itemsNotInSuitcase = new List<InventoryItemScript>();
        }

        public void TryToRemoveItem(InventoryItemScript item)
        {
            itemsNotInSuitcase.Remove(item);
        }

        public void TryToAddItem(InventoryItemScript item)
        {
            if(!itemsNotInSuitcase.Contains(item)) 
            {
                itemsNotInSuitcase.Add(item);
            }
            
            return;
        }

        public void SpawnItems()
        {
            foreach (InventoryItemScript item in itemsNotInSuitcase)
            {
                item.SpawnItemInWorld();
            }
        }
    }
}
