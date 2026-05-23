using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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

        rectTransform.localPosition = localPoint;
        
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
        itemName.text = _itemName;
        itemDescription.text = _itemDescription;
        canvasGroup.alpha = 1;
    }

    public void HideTooltip()
    {
        canvasGroup.alpha = 0;
    }
}
