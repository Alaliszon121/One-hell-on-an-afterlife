using UnityEngine;

public class WorldItemScript : MonoBehaviour
{
    [Header("Selected item prefab")]
    [SerializeField] public GameObject itemPrefab;
    
    [Header("Selected item scriptable object")]
    [SerializeField] public ItemStats itemStats;
    
    [Header("Object in inventory")]
    public GameObject inventoryItemGameObject = null;
}
