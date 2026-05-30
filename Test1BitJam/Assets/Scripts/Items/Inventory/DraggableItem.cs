using System;
using Items.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DraggableItem : MonoBehaviour , IDragHandler, IBeginDragHandler, IEndDragHandler,  IPointerEnterHandler, IPointerExitHandler
{
    
    private RectTransform _itemRectTransform;
    private Vector2 _startPosition;
    private RectTransform _itemArea;
    private bool onTrigger = false;
    private bool canPlace = false;
    private Vector2Int _gridPosition;
    private InventoryItemPlaceholder _inventoryItemPlaceholder;
    private SuitcaseScript _suitcaseScript;
    private InventoryItemScript _inventoryItemScript;
    
    [Header("LastPositionStats")]
    private float tempRot;
    private bool[,] tempArray;
    private int tempHeight;
    private int tempWidth;
    private Vector2Int tempItemPosition;
    private int tempRotationIndex;
    
    string _itemName;
    string _itemDescription;
    
    [Obsolete("Obsolete")]
    private void Start()
    {
        _itemRectTransform = GetComponent<RectTransform>();
        _itemArea = transform.parent.GetComponent<RectTransform>();
        _inventoryItemScript =  GetComponent<InventoryItemScript>();

        _inventoryItemPlaceholder = InventoryUI.Instance._inventoryItemPlaceholder;
        _suitcaseScript = InventoryUI.Instance._suitcaseScript;
        
        _itemName = _inventoryItemScript.itemName;
        _itemDescription = _inventoryItemScript.itemDescription;
        
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _itemRectTransform.anchoredPosition += eventData.delta;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector3 pos = transform.localPosition;

        pos.z = -35;
        
        transform.localPosition = pos;
        
        tempRot = _inventoryItemScript._rotation; 
        tempArray = _inventoryItemScript._slotArray;
        tempHeight = _inventoryItemScript._height;
        tempWidth = _inventoryItemScript._width;
        tempRotationIndex = _inventoryItemScript._rotationIndex;
        
        
        _inventoryItemScript.isDragging = true;
        onTrigger = true;
        transform.SetParent(InventoryUI.Instance._itemSpawn);
       transform.SetAsLastSibling();
       TooltipInstance.instance.HideTooltip();
       _startPosition = _itemRectTransform.anchoredPosition;
       
       if(_gridPosition != new  Vector2Int(-1, -1)) InventoryUI.Instance.RemoveFromGrid(_gridPosition, _inventoryItemScript._height, _inventoryItemScript._width, _inventoryItemScript._slotArray);
       
       
       Cursor.visible = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        if (_inventoryItemScript._rotationIndex % 2 == 1)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                InventoryUI.Instance.GridRectTransform,
                Camera.main.WorldToScreenPoint(_itemRectTransform.position-_inventoryItemScript.secondRotationOffset),
                Camera.main,
                out Vector2 point);
            
            localPoint = point;
        }
        else
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                InventoryUI.Instance.GridRectTransform,
                Camera.main.WorldToScreenPoint(_itemRectTransform.position-_inventoryItemScript.firstRotationOffset),
                Camera.main,
                out Vector2 point);
            
            localPoint = point;
        }
        
        Debug.Log(localPoint);

        canPlace = InventoryUI.Instance.CanPlaceInGrid(localPoint, _inventoryItemScript._height,  _inventoryItemScript._width);

        if (!canPlace)
        {
            _gridPosition = new Vector2Int(-1, -1);
        }
        

        _gridPosition = InventoryUI.Instance.CalculateGrid(localPoint);
        
        _inventoryItemScript.isDragging = false;
        Cursor.visible = true;
        onTrigger = false;
        
        bool validPlace =
            RectTransformUtility.RectangleContainsScreenPoint(
                _itemArea,
                Input.mousePosition,
                Camera.main
            );

        if (validPlace)
        {
            _inventoryItemPlaceholder.TryToAddItem(_inventoryItemScript);
            _suitcaseScript.TryToRemoveFromSuitcase(_inventoryItemScript);
            TooltipInstance.instance.ShowTooltip(_itemName, _itemDescription);
        }
        else if (canPlace && InventoryUI.Instance.DoesItemOverlap(_gridPosition, _inventoryItemScript._height, _inventoryItemScript._width) && _gridPosition != new  Vector2Int(-1, -1))
        {
            if (_inventoryItemScript._rotationIndex % 2 == 1) _itemRectTransform.position = InventoryUI.Instance._slotsRectTransform[_gridPosition].position + _inventoryItemScript.secondRotationOffset;
            else _itemRectTransform.position = InventoryUI.Instance._slotsRectTransform[_gridPosition].position + _inventoryItemScript.firstRotationOffset ;
            
            InventoryUI.Instance.PlaceInGrid(_gridPosition, _inventoryItemScript._height, _inventoryItemScript._width, Enum.Parse<ItemType>(_itemName), _inventoryItemScript._slotArray);

            tempItemPosition = _gridPosition;
            
            _suitcaseScript.TryToAddToSuitcase(_inventoryItemScript);
            
            _inventoryItemPlaceholder.TryToRemoveItem(_inventoryItemScript);
            
            transform.SetParent(InventoryUI.Instance._suitcaseScript.transform);
            
            TooltipInstance.instance.ShowTooltip(_itemName, _itemDescription);
        }
        else SetBackItem(validPlace);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onTrigger) return;
        TooltipInstance.instance.ShowTooltip(_itemName, _itemDescription);
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipInstance.instance.HideTooltip();
        
    }

    private void SetBackItem(bool validPlace)
    {
        _inventoryItemScript.SetLastRotation(tempRot, tempArray, tempHeight, tempWidth, tempRotationIndex);

        LeanTween.value(
            gameObject,
            _itemRectTransform.anchoredPosition,
            _startPosition,
            0.25f
        ).setOnUpdate((Vector2 value) =>
        {
            _itemRectTransform.anchoredPosition = value;
        }).setEaseOutBack().setOnComplete(() =>
        {
            if (!validPlace)
            {
                InventoryUI.Instance.PlaceInGrid(tempItemPosition, tempHeight, tempWidth, Enum.Parse<ItemType>(_itemName), tempArray);
                _gridPosition = tempItemPosition;
                transform.SetParent(InventoryUI.Instance._suitcaseScript.transform);
            }
        });
        
        

        
    }
}
