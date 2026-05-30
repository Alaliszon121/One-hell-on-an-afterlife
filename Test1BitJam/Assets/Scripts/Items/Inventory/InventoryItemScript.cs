using Items.Inventory;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemScript : MonoBehaviour
{
    [SerializeField] public RectTransform gridAnchorRect;
    
    public ItemStats itemStats;
    
    private Image _image;
    private InventoryItemPlaceholder _itemPlaceholder;
    private RectTransform _itemRect;
    public int _rotationIndex = 0;
    private GameObject _worldItem;
    
    public bool[,] _slotArray;
    
    public bool isDragging = false;
    private bool rotating = false;
    
    [Header("Opis")]
    public string itemName;
    public string itemDescription;

    [Header("Dane")] 
    public int _height;
    public int _width;
    public float _rotation = 0;

    [Header("Pivots")] 
    public Vector3 firstRotationOffset;
    public Vector3 secondRotationOffset = Vector3.zero;

    private void Update()
    {
        if(isDragging && Input.GetKeyDown(KeyCode.R)) RotateItem();
    }
    
    private void Awake()
    {
        _itemPlaceholder = InventoryUI.Instance._inventoryItemPlaceholder;
        _itemPlaceholder.itemsNotInSuitcase.Add(this);
        Debug.Log(_itemPlaceholder.itemsNotInSuitcase.Count);
        _image = GetComponent<Image>();
        
        _itemRect = GetComponent<RectTransform>();
        
        SetLocation(_itemRect);
    }
    

    private void SetLocation(RectTransform itemRect)
    {
        float width = InventoryUI.Instance._itemSpawn.rect.width;
        float height = InventoryUI.Instance._itemSpawn.rect.height;

        float randomX = Random.Range(-width / 2, width / 2);
        float randomY = Random.Range(-height / 2, height / 2);

        itemRect.anchoredPosition = new Vector2(randomX, randomY);
        
        
    }

    private void ConvertListToAray()
    {
        _slotArray =  new bool[_height,_width];
        
        for(int y = 0; y < _height; y++)
        {
            for(int x = 0; x < _width; x++)
            {
                _slotArray[y, x] = itemStats.dimensions[y*_width + x];
            }
        }
    }

    public void TryToRemove()
    {
        if (_itemPlaceholder == null) return;
        if (_itemPlaceholder.itemsNotInSuitcase.Contains(this))
        {
            _itemPlaceholder.TryToRemoveItem(this);
            Destroy(gameObject);
        }
    }

    public void SetStats(GameObject worldItem)
    {
        
        _worldItem = worldItem;
        
        //Debug.Log(_worldItem);
        
        _image.sprite = itemStats.sprites[0];
        _image.SetNativeSize();
        
        gridAnchorRect.anchorMin = Vector2.zero;
        gridAnchorRect.anchorMax = Vector2.one;
        
        gridAnchorRect.offsetMax = Vector2.zero;
        gridAnchorRect.offsetMin = Vector2.zero;
        
        _width =  itemStats.width;
        _height = itemStats.height;
        
        gridAnchorRect.pivot = new Vector2(1/(float)itemStats.width/2,1 - (1/(float)itemStats.height/2));
        
        firstRotationOffset = _itemRect.position - gridAnchorRect.position;
        
        itemName = itemStats.itemType.ToString();
        itemDescription = itemStats.description;
        
        ConvertListToAray();
    }

    public void RotateItem()
    {
        if(rotating) return;
        
        rotating = true;
        
        _rotationIndex = (_rotationIndex + 1) % 4;
        
        transform.localEulerAngles =
            new Vector3(
                transform.localEulerAngles.x,
                transform.localEulerAngles.y,
                -90 * _rotationIndex
            );

        RotateArray();
        
        //gridAnchorRect.pivot = new Vector2(1/(float)itemStats.width/2,1 - (1/(float)itemStats.height/2));

        if (secondRotationOffset == Vector3.zero)
        {
            gridAnchorRect.pivot = new Vector2(1/(float)itemStats.width/2,(1/(float)itemStats.height/2));
            secondRotationOffset = _itemRect.position - gridAnchorRect.position;
        }
        
        rotating =  false;
    }

    private void RotateArray()
    {
        
        bool[,] newArray = new bool[_width,_height];

        for (int y = 0; y < _height; y++)
        {
            for (int x = 0; x < _width; x++)
            {
                newArray[x, _height-1-y] = _slotArray[y, x];
            }
        }
        
        _slotArray = newArray;
        
        (_width, _height) = (_height, _width);
    }

    public void DestroyItemInWorld()
    {
        if (_worldItem.activeSelf)
        {
            _worldItem.SetActive(false);
        }
    }

    public void SpawnItemInWorld()
    {
        if (!_worldItem.activeSelf)
        {
            
            Collider[] hits;
            Vector3 spawnPosition;
            int attempts = 0;
            float multiplier = 0.5f;
            
            do
            {
                Vector2 random2D = Random.insideUnitCircle * multiplier;
            
                spawnPosition = InventoryUI.Instance._itemSpawnTransform.position +  new Vector3(random2D.x, 0, random2D.y);
            
                 hits = Physics.OverlapSphere(spawnPosition, 0.5f, InventoryUI.Instance.blockingLayers);
                 
                 attempts++;
                 
                 if(attempts > 5)
                 {
                     attempts = 0;
                     multiplier += 0.5f;
                 }
                 
                 if(multiplier > 2) break;
            }while(hits.Length != 0);
            
            _worldItem.transform.position = spawnPosition;
            _worldItem.SetActive(true);
        }
    }

    public void SetLastRotation(float rotation, bool[,] newArray, int height, int width, int rotationIndex)
    {
        Vector3 rot = transform.localEulerAngles;

        rot.z = rotation;
        
        transform.localEulerAngles = rot;
        
        _slotArray =  newArray;
        
        _height = height;
        _width = width;
        
        _rotationIndex = rotationIndex;
    }
}
