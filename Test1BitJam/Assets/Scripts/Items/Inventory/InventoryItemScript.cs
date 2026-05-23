using Items.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemScript : MonoBehaviour
{
    [Header("ScriptableObject")]
    [SerializeField] private ItemStats itemStats;
    
    private RectTransform _itemsArea;
    private Image _image;
    private InventoryItemPlaceholder _itemPlaceholder;
    private RectTransform _itemRect;
    public string itemName;
    public string itemDescription;
    
    private void Awake()
    {
        _itemPlaceholder =
            GetComponentInParent<InventoryItemPlaceholder>();
        Debug.Log(_itemPlaceholder);
        _itemPlaceholder.itemsNotInSuitcase.Add(gameObject);
        _image = GetComponent<Image>();
        _image.sprite = itemStats.sprites[0];
        
        _itemsArea = transform.parent.GetComponent<RectTransform>();

        itemName = itemStats.itemType.ToString();
        itemDescription = itemStats.description;
        
        _itemRect = GetComponent<RectTransform>();
        SetLocation(_itemRect);
    }

    private void SetLocation(RectTransform itemRect)
    {
        float width = _itemsArea.rect.width;
        float height = _itemsArea.rect.height;

        float randomX = Random.Range(-width / 2, width / 2);
        float randomY = Random.Range(-height / 2, height / 2);

        itemRect.anchoredPosition = new Vector2(randomX, randomY);
    }

    public void TryToRemove()
    {
        if (_itemPlaceholder == null) return;
        if(_itemPlaceholder.itemsNotInSuitcase.Contains(gameObject)) Destroy(gameObject);
    }
}
