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
    
    public Dictionary<Vector2Int, RectTransform> _grid = new  Dictionary<Vector2Int, RectTransform>();

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
                _grid[new Vector2Int(i,l)] = Instantiate(slotUIPrefab, transform).GetComponent<RectTransform>();
            }
        }
    }

    private void Start()
    {
        
        foreach (KeyValuePair<Vector2Int, RectTransform> kvp in _grid)
        {
            InventoryUI.Instance._suitcaseScript.spaceInSuitcase[kvp.Key] = ItemType.None;
        }
    }

    public bool CanPlaceInGrid(Vector2 gridPosition, int  height, int width)
    {
        if(gridPosition.x + (width-1)*slotWidth < slotWidth*columns/2 &&
           gridPosition.x > -(slotWidth*columns/2) &&
           gridPosition.y < slotHeight*rows/2 &&
           gridPosition.y - (height-1)*slotHeight> -(slotHeight*rows/2)) return true;
        else 
            return false;
    }

    public Vector2Int CalculateGrid(Vector2 gridPosition)
    {
        int y = Mathf.FloorToInt(((slotWidth*columns/2) + gridPosition.x)/slotWidth);
        int x = Mathf.FloorToInt(((slotHeight*rows/2) - gridPosition.y)/slotHeight);
        return new Vector2Int(x, y);
    }

    public bool DoesItemOverlap(Vector2Int gridPosition, int height, int width)
    {
        if(gridPosition ==  new Vector2Int(-1, -1)) return false;
        
        for (int x = gridPosition.x; x < gridPosition.x + height; x++)
        {
            for (int y = gridPosition.y; y < gridPosition.y + width; y++)
            {
                Debug.Log(InventoryUI.Instance._suitcaseScript.spaceInSuitcase[new Vector2Int(x, y)]);
                if(InventoryUI.Instance._suitcaseScript.spaceInSuitcase[new Vector2Int(x, y)] != ItemType.None) return false;
                
            }
        }
        return true;
    }

    public void PlaceInGrid(Vector2Int gridPosition, int height, int width, ItemType itemType, bool[,] itemSpace)
    {
        if(gridPosition ==  new Vector2Int(-1, -1)) return;
        
        for (int x = gridPosition.x; x < gridPosition.x + height; x++)
        {
            for (int y = gridPosition.y; y < gridPosition.y + width; y++)
            {
                if(itemSpace[x-gridPosition.x,y-gridPosition.y] == true) InventoryUI.Instance._suitcaseScript.spaceInSuitcase[new Vector2Int(x, y)] =  itemType;
            }
        }
    }
    
    public void RemoveFromGrid(Vector2Int gridPosition, int height, int width, bool[,] itemSpace)
    {
        if(gridPosition ==  new Vector2Int(-1, -1)) return;
        
        for (int x = gridPosition.x; x < gridPosition.x + height; x++)
        {
            for (int y = gridPosition.y; y < gridPosition.y + width; y++)
            {
                if(itemSpace[x-gridPosition.x,y-gridPosition.y] == true) InventoryUI.Instance._suitcaseScript.spaceInSuitcase[new Vector2Int(x, y)] = ItemType.None;
            }
        }
    }
}
