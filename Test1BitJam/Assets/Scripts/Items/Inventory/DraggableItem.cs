using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggableItem : MonoBehaviour , IDragHandler, IBeginDragHandler, IEndDragHandler,  IPointerEnterHandler, IPointerExitHandler
{
    private RectTransform _itemRectTransform;
    private Vector2 _startPosition;
    private RectTransform _itemArea;
    private bool onTrigger = false;
    
    string _itemName;
    string _itemDescription;
    
    [Obsolete("Obsolete")]
    private void Start()
    {
        _itemRectTransform = GetComponent<RectTransform>();
        _itemArea = transform.parent.GetComponent<RectTransform>();
        var inventoryItemScript =  GetComponent<InventoryItemScript>();
        _itemName = inventoryItemScript.itemName;
        _itemDescription = inventoryItemScript.itemDescription;
        
        Debug.Log(_itemName);
        Debug.Log(_itemDescription);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        _itemRectTransform.anchoredPosition += eventData.delta;
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
        
        
        if (!validPlace)
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
        else TooltipInstance.instance.ShowTooltip(_itemName, _itemDescription);
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
}
