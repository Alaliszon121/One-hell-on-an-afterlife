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

    [Header("Top-Down Aiming")]
    [Tooltip("The child object that will rotate to face the mouse")]
    public Transform objectToRotate;

    [Header("Movement Speeds")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;

    [Header("Stamina Settings")]
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 15f;

    [SerializeField] private float currentStamina;

    private CharacterController controller;
    private Camera mainCamera;
    private float lockedYPosition;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        mainCamera = Camera.main;
        lockedYPosition = transform.position.y;
    }

    void Update()
    {
        HandleMovementAndStamina();
        HandleTopDownAiming();
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
        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        bool isAttemptingToRun = runAction.action.IsPressed();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }

        GameManager.instance.playerStatus.isWalking = inputVector.sqrMagnitude > 0.1f;
        

        if (isAttemptingToRun && GameManager.instance.playerStatus.isWalking && GameManager.instance.playerStatus.currentStamina > 0f)
        {
            GameManager.instance.playerStatus.isRunning = true;
            GameManager.instance.playerStatus.currentStamina -= staminaDrainRate * Time.deltaTime;
        }
        else if (GameManager.instance.playerStatus.currentStamina < GameManager.instance.playerStatus.maxStamina)
        {
            GameManager.instance.playerStatus.isRunning = false;
            GameManager.instance.playerStatus.currentStamina += staminaRegenRate * Time.deltaTime;
        }

        GameManager.instance.playerStatus.currentStamina = Mathf.Clamp(GameManager.instance.playerStatus.currentStamina, 0f, GameManager.instance.playerStatus.maxStamina);

        float currentSpeed = GameManager.instance.playerStatus.isRunning ? runSpeed : walkSpeed;
        controller.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    private void HandleTopDownAiming()
    {
        if (objectToRotate == null || mainCamera == null) return;

        Vector2 mouseScreenPosition = mousePositionAction.action.ReadValue<Vector2>();

        Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);

        Plane groundPlane = new Plane(Vector3.up, new Vector3(0, objectToRotate.position.y, 0));

        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 pointOfIntersection = ray.GetPoint(rayDistance);

            Vector3 lookDirection = pointOfIntersection - objectToRotate.position;
            lookDirection.y = 0f;

            if (lookDirection.sqrMagnitude > 0.01f)
            {
                objectToRotate.rotation = Quaternion.LookRotation(lookDirection);
            }
        }
    }
}