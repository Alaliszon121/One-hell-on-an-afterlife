using System.Collections.Generic;
using UnityEngine;

namespace Items.Inventory
{
    public class InventoryItemPlaceholder : MonoBehaviour
    {
        [SerializeField] public List<GameObject> itemsNotInSuitcase;

        private void Awake()
        {
            itemsNotInSuitcase = new List<GameObject>();
        }

        public void TryToRemoveItem(GameObject item)
        {
            itemsNotInSuitcase.Remove(item);
        }
    }
}
