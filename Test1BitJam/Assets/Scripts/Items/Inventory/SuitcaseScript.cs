using UnityEngine;

public class SuitcaseScript : MonoBehaviour
{
    [SerializeField] RectTransform gridRect;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (GameManager.instance.playerStatus.isInventoryOpen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect,
                Input.mousePosition,
                _camera,
                out Vector2 localPoint);
            
            Debug.Log(localPoint);
        }
    }
}
