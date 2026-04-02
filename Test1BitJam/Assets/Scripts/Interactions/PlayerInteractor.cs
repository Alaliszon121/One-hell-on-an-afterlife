using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerStateManager stateManager;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Highlight Settings")]
    [SerializeField] private Material highlightMaterial;

    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private IInteractable currentClosestInteractable;
    private Renderer highlightedRenderer;
    private Material originalMaterial;

    private void OnEnable()
    {
        if (interactAction != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += HandleInteractInput;
        }
    }

    private void OnDisable()
    {
        if (interactAction != null)
        {
            interactAction.action.performed -= HandleInteractInput;
            interactAction.action.Disable();
        }
    }

    private void Update()
    {
        HandleClosestInteractable();
    }

    private void HandleClosestInteractable()
    {
        if (interactablesInRange.Count == 0)
        {
            ClearCurrentTarget();
            return;
        }

        IInteractable closest = null;
        float minDistance = float.MaxValue;

        foreach (IInteractable interactable in interactablesInRange)
        {
            GameObject obj = interactable.GetTransform().gameObject;

            if (!stateManager.IsLayerInteractable(obj.layer))
            {
                continue;
            }

            float distance = Vector3.Distance(transform.position, obj.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                closest = interactable;
            }
        }

        if (closest == null)
        {
            ClearCurrentTarget();
            return;
        }

        if (closest != currentClosestInteractable)
        {
            RemoveHighlight();
            currentClosestInteractable = closest;
            ApplyHighlight(currentClosestInteractable);
        }
    }

    private void ClearCurrentTarget()
    {
        if (currentClosestInteractable != null)
        {
            RemoveHighlight();
            currentClosestInteractable = null;
        }
    }

    private void ApplyHighlight(IInteractable interactable)
    {
        highlightedRenderer = interactable.GetTransform().GetComponent<Renderer>();
        if (highlightedRenderer != null)
        {
            originalMaterial = highlightedRenderer.material;
            highlightedRenderer.material = highlightMaterial;
        }
    }

    private void RemoveHighlight()
    {
        if (highlightedRenderer != null && originalMaterial != null)
        {
            highlightedRenderer.material = originalMaterial;
            highlightedRenderer = null;
            originalMaterial = null;
        }
    }

    private void HandleInteractInput(InputAction.CallbackContext context)
    {
        if (currentClosestInteractable != null)
        {
            currentClosestInteractable.OnInteract(this.gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (!interactablesInRange.Contains(interactable))
            {
                interactablesInRange.Add(interactable);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (interactablesInRange.Contains(interactable))
            {
                interactablesInRange.Remove(interactable);
            }
        }
    }
}