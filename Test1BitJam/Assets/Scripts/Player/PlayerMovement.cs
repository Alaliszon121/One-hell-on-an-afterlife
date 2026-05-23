using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Vector2 action for WASD/Left Stick")]
    public InputActionReference moveAction;
    [Tooltip("Button action for Sprinting (e.g., Left Shift)")]
    public InputActionReference runAction;
    [Tooltip("Vector2 action for Mouse Position")]
    public InputActionReference mousePositionAction;

    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Stamina Settings")]
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 15f;

    [SerializeField] private float currentStamina;

    private CharacterController controller;
    private float lockedYPosition;
    private bool isExhausted = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lockedYPosition = transform.position.y;
    }

    void Update()
    {
        if(GameManager.instance.playerStatus.isInventoryOpen) return;
        HandleMovementAndStamina();
    }

    void LateUpdate()
    {
        if (transform.position.y != lockedYPosition)
        {
            transform.position = new Vector3(transform.position.x, lockedYPosition, transform.position.z);
        }
    }

    private void HandleMovementAndStamina()
    {
        // ZABEZPIECZENIE DIALOGOWE: Je�li trwa wa�ny dialog, ca�kowicie blokujemy ruch
        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive && DialogueManager.Instance.BlocksMovement)
        {
            GameManager.instance.playerStatus.isWalking = false;
            GameManager.instance.playerStatus.isRunning = false;
            return;
        }

        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        bool isAttemptingToRun = runAction.action.IsPressed();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        GameManager.instance.playerStatus.isWalking = inputVector.sqrMagnitude > 0.1f;

        if (GameManager.instance.playerStatus.currentStamina <= 0f)
        {
            isExhausted = true;
        }
        else if (GameManager.instance.playerStatus.currentStamina >= GameManager.instance.playerStatus.maxStamina)
        {
            isExhausted = false;
        }

        if (isAttemptingToRun && GameManager.instance.playerStatus.isWalking && !isExhausted)
        {
            GameManager.instance.playerStatus.isRunning = true;
            GameManager.instance.playerStatus.currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else
        {
            GameManager.instance.playerStatus.isRunning = false;

            if (GameManager.instance.playerStatus.currentStamina < GameManager.instance.playerStatus.maxStamina)
            {
                GameManager.instance.playerStatus.currentStamina += staminaRegenRate * Time.deltaTime;
            }
        }

        GameManager.instance.playerStatus.currentStamina = Mathf.Clamp(
            GameManager.instance.playerStatus.currentStamina,
            0f,
            GameManager.instance.playerStatus.maxStamina
        );

        float currentSpeed = GameManager.instance.playerStatus.isRunning ? runSpeed : walkSpeed;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }
}