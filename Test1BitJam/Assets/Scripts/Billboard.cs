using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Transform mainCameraTransform;

    private void Start()
    {
        if (Camera.main != null)
        {
            mainCameraTransform = Camera.main.transform;
        }
        else
        {
            Debug.LogError("No camera tagged 'MainCamera' found in the scene.");
        }
    }

    private void LateUpdate()
    {
        if (mainCameraTransform == null) return;

        transform.LookAt(transform.position + mainCameraTransform.forward);
    }
}