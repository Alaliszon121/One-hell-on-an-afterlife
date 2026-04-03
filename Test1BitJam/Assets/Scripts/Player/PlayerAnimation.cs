using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Vector2 action for WASD/Left Stick")]
    public InputActionReference moveAction;
    [Tooltip("Button action for Sprinting (e.g., Left Shift)")]
    public InputActionReference runAction;
    
    [Header("Movement")]
    [SerializeField] private float rotationSpeed = 5f;
    
    private Animator _animator;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        Vector2 inputVector = moveAction.action.ReadValue<Vector2>();
        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        
        if (moveDirection.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float snappedAngle = Mathf.Round(angle / 45f) * 45f;

            Quaternion targetRotation = Quaternion.Euler(0f, snappedAngle, 0f);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
        
        if (_animator.GetBool("IsWalking") != GameManager.instance.playerStatus.isWalking)
        {
            _animator.SetBool("IsWalking", GameManager.instance.playerStatus.isWalking);
        }

        if (_animator.GetBool("IsRunning") != GameManager.instance.playerStatus.isRunning)
        {
            _animator.SetBool("IsRunning", GameManager.instance.playerStatus.isRunning);
        }
    }
}
