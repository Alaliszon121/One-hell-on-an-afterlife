using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class Equipment : MonoBehaviour
    {
        [Header("Dependencies")]
        [SerializeField] private InputActionReference openInventoryAction;
        [SerializeField] private GameObject suitcaseTop;
        [SerializeField] private GameObject suitcaseBottom;
    
        [Header("Animation")]
        [SerializeField] private float animTime = 2;
        [SerializeField] private float cameraDistance = 3;
    
        [Header("Canvas")]
        [SerializeField] private GameObject inventoryCanvas;
        [SerializeField] private CanvasGroup inventoryCanvasGroup;
        
        [Header("Grid")]
        [SerializeField] private RectTransform inventoryPanelRectTransform;
        [SerializeField] private CanvasGroup suitcasePanelCanvasGroup;
        [SerializeField] private Transform suitcaseCenterTransform;
        
    
        private Transform cameraTransform;
        private float cameraHeight;
        private RectTransform _inventoryCanvasRectTransform;
        private Camera _camera;


        void Start()
        {
            _camera = Camera.main;
            cameraHeight = Camera.main.orthographicSize * 2;
            inventoryCanvas.GetComponent<Canvas>().planeDistance = cameraDistance+0.3f;
            _inventoryCanvasRectTransform = inventoryCanvas.GetComponent<RectTransform>();
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    
        private void OnEnable()
        {
            if (openInventoryAction == null) return;
        
            openInventoryAction.action.Enable();
            openInventoryAction.action.performed += OpenInventory;
        
        }

        private void OnDisable()
        {
            if (openInventoryAction == null) return;
        
            openInventoryAction.action.performed -= OpenInventory;
            openInventoryAction.action.Disable();
        
        
        }
    
        private void OpenInventory(InputAction.CallbackContext context)
        {
            if (!context.performed) return;
        
            if(!GameManager.instance.playerStatus.isInventoryOpen) OpenInventoryAnimation();
            else CloseInventoryAnimation();
        }

        private void OpenInventoryAnimation()
        {
            LTSeq seq = LeanTween.sequence();
        
            GameManager.instance.playerStatus.isInventoryOpen = !GameManager.instance.playerStatus.isInventoryOpen;
            suitcaseTop.SetActive(true);
            suitcaseBottom.SetActive(true);
            inventoryCanvasGroup.alpha = 1;
            inventoryCanvasGroup.blocksRaycasts = true;
            inventoryCanvasGroup.interactable = true;
        
            Vector3 cameraCenter = Camera.main.ViewportToWorldPoint(new Vector3(0.35f, 0.85f, cameraDistance));
            Vector3 cameraBottom = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f, cameraDistance));
            //Vector3 canvasPos = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, cameraDistance+0.5f));
        
            suitcaseTop.transform.position = cameraBottom ;
            suitcaseBottom.transform.position = cameraBottom ;
            //inventoryCanvas.transform.position = canvasPos;
        
            seq.append(() => {
                LeanTween.move(suitcaseTop, cameraCenter, animTime).setEaseOutBounce();
                LeanTween.move(suitcaseBottom, cameraCenter, animTime).setEaseOutBounce();
            });
        
            seq.append(animTime+0.2f);

            seq.append(() =>
            {
                suitcasePanelCanvasGroup.alpha = 1;
                
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _inventoryCanvasRectTransform,
                    _camera.WorldToScreenPoint(suitcaseCenterTransform.position),
                    _camera,
                    out var localPoint);

                inventoryPanelRectTransform.anchoredPosition = localPoint;
            });

            seq.append(LeanTween.rotateAroundLocal(suitcaseTop, Vector3.right, 90f, animTime).setEaseOutElastic());

        }
    
        private void CloseInventoryAnimation()
        {
            TooltipInstance.instance.HideTooltip();
            
            LTSeq seq = LeanTween.sequence();
        
            LeanTween.cancel(suitcaseTop);
            LeanTween.cancel(suitcaseBottom);
        
            Vector3 cameraBottom = Camera.main.ViewportToWorldPoint(new Vector3(0.5f, 0f, cameraDistance));
        
            seq.append(LeanTween.rotateLocal(suitcaseTop, new Vector3(-110f, 0f, 0f), animTime).setEaseOutExpo());
        
            seq.append(() => {
                LeanTween.move(suitcaseTop, cameraBottom, animTime).setEaseInOutBack();
                LeanTween.move(suitcaseBottom, cameraBottom, animTime).setEaseInOutBack();
            });
        
            seq.append(animTime);

            seq.append(() =>
            {
                InventoryUI.Instance._suitcaseScript.DestroyWorldItemsInSuitCase();
                InventoryUI.Instance._inventoryItemPlaceholder.SpawnItems();
            });

            suitcasePanelCanvasGroup.alpha = 0;

            seq.append(() =>
            {
                suitcaseTop.SetActive(false);
                suitcaseBottom.SetActive(false);
                inventoryCanvasGroup.alpha = 0;
                inventoryCanvasGroup.blocksRaycasts = false;
                inventoryCanvasGroup.interactable = false;
                
                GameManager.instance.playerStatus.isInventoryOpen = !GameManager.instance.playerStatus.isInventoryOpen;
            });


        }
    }
}
