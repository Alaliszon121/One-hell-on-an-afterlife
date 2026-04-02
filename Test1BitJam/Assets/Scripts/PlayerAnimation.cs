using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Input Actions")]
    [Tooltip("Vector2 action for WASD/Left Stick")]
    public InputActionReference moveAction;
    [Tooltip("Button action for Sprinting (e.g., Left Shift)")]
    public InputActionReference runAction;
    
    private Animator _animator;
    private RuntimeAnimatorController _controller;

    void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _controller = _animator.runtimeAnimatorController;
    }

    void Update()
    {
        Vector2 moveInput = moveAction.action.ReadValue<Vector2>();
        bool isMovingNow = moveInput.sqrMagnitude > 0.1f;
        bool isRunningNow = runAction.action.IsPressed() 
                            && GameManager.instance.playerStatus.isWalking 
                            && GameManager.instance.playerStatus.isRunning;
        
        if (isMovingNow != GameManager.instance.playerStatus.isWalking)
        {
            GameManager.instance.playerStatus.isWalking = isMovingNow;
            _animator.SetBool("IsWalking", isMovingNow);
        }

        if (isRunningNow != GameManager.instance.playerStatus.isRunning)
        {
            GameManager.instance.playerStatus.isRunning = isRunningNow;
            _animator.SetBool("IsRunning", isRunningNow);
        }
    }
}
