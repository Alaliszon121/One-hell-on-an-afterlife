using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField] private PlayerStateManager stateManager;

    [Header("Input Settings")]
    [SerializeField] private InputActionReference interactAction;

    [Header("Highlight Settings")]
    [Tooltip("How much lighter the object becomes. Multiplies the base color for emission.")]
    [SerializeField] [Range(0f, 5f)] private float highlightEmissionIntensity = 1.25f;

    private List<IInteractable> interactablesInRange = new List<IInteractable>();
    private IInteractable currentClosestInteractable;
    private Renderer highlightedRenderer;

    private Material originalSharedMaterial;
    private Material instancedHighlightMaterial;

    [Header("End Game Settings")]
    [SerializeField] private string nextSceneName;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private Volume globalVolume;

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

    private void Start()
    {
        
    }
    private void Update()
    {
        HandleClosestInteractable();
    }

    private void HandleClosestInteractable()
    {
        interactablesInRange.RemoveAll(interactable => interactable as UnityEngine.Object == null);

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
            originalSharedMaterial = highlightedRenderer.sharedMaterial;

            instancedHighlightMaterial = highlightedRenderer.material;

            instancedHighlightMaterial.EnableKeyword("_EMISSION");

            Color baseColor = Color.white;
            if (instancedHighlightMaterial.HasProperty("_BaseColor"))
            {
                baseColor = instancedHighlightMaterial.GetColor("_BaseColor");
            }
            else if (instancedHighlightMaterial.HasProperty("_Color"))
            {
                baseColor = instancedHighlightMaterial.GetColor("_Color");
            }

            instancedHighlightMaterial.SetColor("_EmissionColor", baseColor * highlightEmissionIntensity);
        }
    }

    private void RemoveHighlight()
    {
        if (highlightedRenderer != null && originalSharedMaterial != null)
        {
            highlightedRenderer.sharedMaterial = originalSharedMaterial;

            if (instancedHighlightMaterial != null)
            {
                Destroy(instancedHighlightMaterial);
            }

            highlightedRenderer = null;
            originalSharedMaterial = null;
            instancedHighlightMaterial = null;
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
        if (other.CompareTag("EndGame"))
        {
            other.enabled = false;
            StartCoroutine(FadeAndEndGame());
            return;
        }
    }

    private IEnumerator FadeAndEndGame()
    {
        if (globalVolume.profile.TryGet(out ColorAdjustments colorAdjustments))
        {
            Color initialColor = colorAdjustments.colorFilter.value;
            float time = 0;
            while (time < fadeDuration)
            {
                colorAdjustments.colorFilter.value = Color.Lerp(initialColor, Color.black, time / fadeDuration);
                time += Time.deltaTime;
                yield return null;
            }
            colorAdjustments.colorFilter.value = Color.black;
        }
        SceneManager.LoadScene(nextSceneName);
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