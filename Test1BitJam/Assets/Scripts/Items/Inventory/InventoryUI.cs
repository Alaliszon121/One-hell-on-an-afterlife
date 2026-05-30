using System.Collections.Generic;
using Items.Inventory;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [SerializeField] private RectTransform _gridRectTransform;
    [SerializeField] private InventoryGrid _inventoryGrid;
    [SerializeField] public SuitcaseScript _suitcaseScript;
    [SerializeField] public RectTransform _itemSpawn;
    [SerializeField] public InventoryItemPlaceholder _inventoryItemPlaceholder;
    
    [Header("Item spawner")]
    [SerializeField] public Transform _itemSpawnTransform;
    [SerializeField] public LayerMask blockingLayers;

    public RectTransform GridRectTransform => _gridRectTransform;
    public Dictionary<Vector2Int, RectTransform> _slotsRectTransform => _inventoryGrid._grid;
    
    

    private void Awake()
    {
        Instance = this;
    }

    public bool CanPlaceInGrid(Vector2 gridPosition, int height, int width)
    {
       return _inventoryGrid.CanPlaceInGrid(gridPosition, height, width);
    }

    public Vector2Int CalculateGrid(Vector2 gridPosition)
    {
        return _inventoryGrid.CalculateGrid(gridPosition);
    }
    
    public bool DoesItemOverlap(Vector2Int gridPosition, int height, int width)
    {
        return _inventoryGrid.DoesItemOverlap(gridPosition,  height,  width);
    }

    public void PlaceInGrid(Vector2Int gridPosition, int height, int width, ItemType itemType, bool[,] itemSpace)
    {
        _inventoryGrid.PlaceInGrid( gridPosition,  height,  width, itemType, itemSpace);
    }
    
    public void RemoveFromGrid(Vector2Int gridPosition, int height, int width, bool[,] itemSpace)
    {
        _inventoryGrid.RemoveFromGrid(gridPosition, height, width, itemSpace);
    }
}
