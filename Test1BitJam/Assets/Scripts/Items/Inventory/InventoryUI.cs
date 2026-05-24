using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance;

    [SerializeField] private RectTransform _gridRectTransform;
    [SerializeField] private InventoryGrid _inventoryGrid;
    [SerializeField] public SuitcaseScript _suitcaseScript;

    public RectTransform GridRectTransform => _gridRectTransform;
    public Dictionary<Vector2Int, RectTransform> _slotsRectTransform => _inventoryGrid._grid;
    

    private void Awake()
    {
        Instance = this;
    }

    public bool CanPlaceInGrid(Vector2 gridPosition)
    {
       return _inventoryGrid.CanPlaceInGrid(gridPosition);
    }

    public Vector2Int CalculateGrid(Vector2 gridPosition)
    {
        return _inventoryGrid.CalculateGrid(gridPosition);
    }
}
