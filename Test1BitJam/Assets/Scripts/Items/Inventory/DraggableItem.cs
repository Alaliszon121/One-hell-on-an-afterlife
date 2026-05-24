using System;
using Items.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour , IDragHandler, IBeginDragHandler, IEndDragHandler,  IPointerEnterHandler, IPointerExitHandler
{
    
    private RectTransform _itemRectTransform;
    private Vector2 _startPosition;
    private RectTransform _itemArea;
    private bool onTrigger = false;
    private bool canPlace = false;
    private Vector2Int _gridPosition;
    private InventoryItemPlaceholder _inventoryItemPlaceholder;
    
    string _itemName;
    string _itemDescription;
    
    [Obsolete("Obsolete")]
    private void Start()
    {
        _itemRectTransform = GetComponent<RectTransform>();
        _itemArea = transform.parent.GetComponent<RectTransform>();
        var inventoryItemScript =  GetComponent<InventoryItemScript>();
        
        _inventoryItemPlaceholder = GetComponentInParent<InventoryItemPlaceholder>();
        
        _itemName = inventoryItemScript.itemName;
        _itemDescription = inventoryItemScript.itemDescription;
        
        Debug.Log(_itemName);
        Debug.Log(_itemDescription);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _itemRectTransform.anchoredPosition += eventData.delta;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            InventoryUI.Instance.GridRectTransform,
            Camera.main.WorldToScreenPoint(_itemRectTransform.position),
            Camera.main,
            out Vector2 localPoint);

        canPlace = InventoryUI.Instance.CanPlaceInGrid(localPoint);

        if (!canPlace)
        {
            _gridPosition = new Vector2Int(-1, -1);
            return;
        }
        

        _gridPosition = InventoryUI.Instance.CalculateGrid(localPoint);
        
        Debug.Log(InventoryUI.Instance.CalculateGrid(localPoint));
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        onTrigger = true;
       transform.SetAsLastSibling();
       TooltipInstance.instance.HideTooltip();
       _startPosition = _itemRectTransform.anchoredPosition;
       
       
       Cursor.visible = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Cursor.visible = true;
        onTrigger = false;
        
        bool validPlace =
            RectTransformUtility.RectangleContainsScreenPoint(
                _itemArea,
                Input.mousePosition,
                Camera.main
            );
        
        Debug.Log(validPlace);
        
        
        if (validPlace) TooltipInstance.instance.ShowTooltip(_itemName, _itemDescription);
        else if (canPlace && InventoryUI.Instance._suitcaseScript.spaceInSuitcase[_gridPosition] == ItemType.None && _gridPosition != new  Vector2Int(-1, -1))
        {
            Debug.Log(_itemName);
            _itemRectTransform.position = InventoryUI.Instance._slotsRectTransform[_gridPosition].position;
            InventoryUI.Instance._suitcaseScript.spaceInSuitcase[_gridPosition] = Enum.Parse<ItemType>(_itemName);
            
            _inventoryItemPlaceholder.TryToRemoveItem(gameObject);
            
            TooltipInstance.instance.ShowTooltip(_itemName, _itemDescription);
        }
        else SetBackItem();
        
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

    private void SetBackItem()
    {
        LeanTween.value(
            gameObject,
            _itemRectTransform.anchoredPosition,
            _startPosition,
            0.25f
        ).setOnUpdate((Vector2 value) =>
        {
            _itemRectTransform.anchoredPosition = value;
        }).setEaseOutBack();
    }
}
