using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TooltipInstance : MonoBehaviour
{
    public static TooltipInstance instance;
    
    public TMP_Text itemName;
    public TMP_Text itemDescription;
    private CanvasGroup canvasGroup;
    private Canvas canvas;
    private RectTransform rectTransform;
    
    private CanvasGroup inventoryCanvasGroup;

    private void Awake()
    {
        instance = this;
        
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        inventoryCanvasGroup = canvas.GetComponent<CanvasGroup>();
    }

    void Update()
    {
        if(inventoryCanvasGroup.alpha == 0) return;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            Input.mousePosition,
            canvas.worldCamera,
            out var localPoint
        );

        rectTransform.localPosition = (Vector3)localPoint + new Vector3(0,0,-35);
        
        Vector2 mousePos = Input.mousePosition;

        bool overflowRight =
            mousePos.x + rectTransform.rect.width > Screen.width;

        if (overflowRight)
        {
            rectTransform.pivot = new Vector2(1, 1);
        }
        else
        {
            rectTransform.pivot = new Vector2(0, 1);
        }
    }

    public void ShowTooltip(string _itemName, string _itemDescription)
    {
        itemName.text = _itemName.Replace("_", " ");
        itemDescription.text = _itemDescription;
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        
        canvasGroup.alpha = 1;
    }

    public void HideTooltip()
    {
        canvasGroup.alpha = 0;
    }
}
