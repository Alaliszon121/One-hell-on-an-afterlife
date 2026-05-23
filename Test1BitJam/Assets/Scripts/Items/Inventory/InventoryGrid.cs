using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryGrid : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] GameObject slotUIPrefab;
    [SerializeField] int slotHeight = 100;
    [SerializeField] int slotWidth = 100;
    [SerializeField] int columns = 10;
    [SerializeField] int rows = 10;
    
    private GridLayoutGroup _gridLayoutGroup;
    private RectTransform _rectTransform;
    
    private Dictionary<Vector2Int, GameObject> _grid = new Dictionary<Vector2Int, GameObject>();

    private void Awake()
    {
        _gridLayoutGroup = GetComponent<GridLayoutGroup>();
        _rectTransform = GetComponent<RectTransform>();
        
        _gridLayoutGroup.cellSize = new Vector2(slotWidth, slotHeight);
        _rectTransform.sizeDelta = new Vector2(slotWidth*columns, slotHeight*rows);

        for (int i = 0; i < rows; i++)
        {
            for (int l = 0; l < columns; l++)
            {
                _grid[new Vector2Int(i,l)] = Instantiate(slotUIPrefab, transform);
            }
        }
    }
    
    
}
