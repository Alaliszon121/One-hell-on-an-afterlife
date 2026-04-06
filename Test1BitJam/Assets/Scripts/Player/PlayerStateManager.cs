using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerStateManager : MonoBehaviour
{
    

    [Header("Current State")]
    public PlayerColorState currentState = PlayerColorState.White;

    [Header("Dependencies")]
    [Tooltip("Input Action to go to the next state")]
    [SerializeField] private InputActionReference nextStateAction;
    [Tooltip("Input Action to go to the previous state")]
    [SerializeField] private InputActionReference previousStateAction;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Volume globalVolume;

    [Header("Layer Names")]
    [SerializeField] private string whiteLayerName = "State_White";
    [SerializeField] private string blueLayerName = "State_Blue";
    [SerializeField] private string redLayerName = "State_Red";

    [Header("Color Settings")]
    [SerializeField] public Color whiteStateColor = Color.white;
    [SerializeField] public Color blueStateColor = new Color(0.5f, 0.7f, 1f);
    [SerializeField] public Color redStateColor = new Color(1f, 0.5f, 0.5f);

    private ColorAdjustments colorAdjustments;

    private int whiteLayer;
    private int blueLayer;
    private int redLayer;

    public System.Action<PlayerColorState> OnStateChanged;

    private bool wasChangedToBlueBefore = false;
    private bool wasChangedToRedBefore = false;

    private void Start()
    {
        whiteLayer = LayerMask.NameToLayer(whiteLayerName);
        blueLayer = LayerMask.NameToLayer(blueLayerName);
        redLayer = LayerMask.NameToLayer(redLayerName);

        if (playerCamera == null) playerCamera = Camera.main;
        if (globalVolume != null) globalVolume.profile.TryGet(out colorAdjustments);

        ApplyVisualChanges();
    }

    private void OnEnable()
    {
        if (nextStateAction != null)
        {
            nextStateAction.action.Enable();
            nextStateAction.action.performed += CycleNextState;
        }

        if (previousStateAction != null)
        {
            previousStateAction.action.Enable();
            previousStateAction.action.performed += CyclePreviousState;
        }
    }

    private void OnDisable()
    {
        if (nextStateAction != null)
        {
            nextStateAction.action.performed -= CycleNextState;
            nextStateAction.action.Disable();
        }

        if (previousStateAction != null)
        {
            previousStateAction.action.performed -= CyclePreviousState;
            previousStateAction.action.Disable();
        }
    }

    private void CycleNextState(InputAction.CallbackContext context)
    {
        int totalStates = System.Enum.GetValues(typeof(PlayerColorState)).Length;
        int nextStateIndex = ((int)currentState + 1) % totalStates;

        SetState((PlayerColorState)nextStateIndex);
    }

    private void CyclePreviousState(InputAction.CallbackContext context)
    {
        int totalStates = System.Enum.GetValues(typeof(PlayerColorState)).Length;
        int prevStateIndex = ((int)currentState - 1 + totalStates) % totalStates;

        SetState((PlayerColorState)prevStateIndex);
    }

    public void SetState(PlayerColorState newState)
    {
        currentState = newState;
        ApplyVisualChanges();
        OnStateChanged?.Invoke(currentState);
        if(!wasChangedToRedBefore && currentState == PlayerColorState.Red)
        {
            QuestManager.Instance.CompleteQuest("2");
            wasChangedToRedBefore = true;
        }
        else if (!wasChangedToBlueBefore && currentState == PlayerColorState.Blue)
        {
            QuestManager.Instance.CompleteQuest("3");
            wasChangedToBlueBefore = true;
        }
    }

    private void ApplyVisualChanges()
    {
        if (colorAdjustments != null)
        {
            switch (currentState)
            {
                case PlayerColorState.White: colorAdjustments.colorFilter.value = whiteStateColor; break;
                case PlayerColorState.Blue: colorAdjustments.colorFilter.value = blueStateColor; break;
                case PlayerColorState.Red: colorAdjustments.colorFilter.value = redStateColor; break;
            }
        }

        if (playerCamera != null)
        {
            playerCamera.cullingMask = (currentState == PlayerColorState.White)
                ? playerCamera.cullingMask | (1 << whiteLayer) : playerCamera.cullingMask & ~(1 << whiteLayer);

            playerCamera.cullingMask = (currentState == PlayerColorState.Blue)
                ? playerCamera.cullingMask | (1 << blueLayer) : playerCamera.cullingMask & ~(1 << blueLayer);

            playerCamera.cullingMask = (currentState == PlayerColorState.Red)
                ? playerCamera.cullingMask | (1 << redLayer) : playerCamera.cullingMask & ~(1 << redLayer);
        }
    }

    public bool IsLayerInteractable(int layer)
    {
        if (layer == whiteLayer) return currentState == PlayerColorState.White;
        if (layer == blueLayer) return currentState == PlayerColorState.Blue;
        if (layer == redLayer) return currentState == PlayerColorState.Red;

        return currentState == PlayerColorState.Blue;
    }
}

public enum PlayerColorState { White, Blue, Red }