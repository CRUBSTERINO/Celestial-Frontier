using UnityEngine;
using static UnityEngine.InputSystem.InputAction;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Animator _animator;

    private PlayerInputActions _inputActions;
    private Vector2 _movementVector;
    private Rigidbody2D _rb;

    private int _animatorHorizontalParameterHash;
    private int _animatorVerticalParameterHash;
    private int _animatorIsMovingParameterHash;

    public bool CanMove { get; set; }
    public bool IsMoving { get; set; }

    private void Awake()
    {
        _animatorHorizontalParameterHash = Animator.StringToHash("Horizontal");
        _animatorVerticalParameterHash = Animator.StringToHash("Vertical");
        _animatorIsMovingParameterHash = Animator.StringToHash("IsMoving");
    }

    private void Start()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Enable();

        _inputActions.GeneralControls.Movement.performed += OnMove;
        _inputActions.GeneralControls.Movement.canceled += OnMoveStop;

        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnDestroy()
    {
        _inputActions.Disable();

        _inputActions.GeneralControls.Movement.performed -= OnMove;
        _inputActions.GeneralControls.Movement.canceled -= OnMoveStop;
    }

    private void FixedUpdate()
    {
        if (!CanMove) return;

        Vector2 movementVectorClamped = Vector2.ClampMagnitude(_movementVector * _speed, _speed);
        Vector2 deltaPosition = movementVectorClamped * Time.fixedDeltaTime;
        _rb.MovePosition(deltaPosition + _rb.position);
    }

    private void OnMove(CallbackContext callbackContext)
    {
        _movementVector = callbackContext.ReadValue<Vector2>();

        if (!CanMove) return;

        IsMoving = true;

        float horizontal = 0f;
        float vertical = 0f;

        _animator.SetBool(_animatorIsMovingParameterHash, true);

        // 0.01f is just a magic number to exclude float inaccuracies
        if (_movementVector.x > 0.01f || _movementVector.x < -0.01f)
        {
            horizontal = (_movementVector.x > 0) ? 1f : -1f;
        }

        // 0.01f is just a magic number to exclude float inaccuracies
        if (_movementVector.y > 0.01f || _movementVector.y < -0.01f)
        {
            vertical = (_movementVector.y > 0) ? 1f : -1f;
        }

        if (horizontal != 0f && vertical != 0f)
        {
            vertical = 0f;
        }

        _animator.SetFloat(_animatorHorizontalParameterHash, horizontal);
        _animator.SetFloat(_animatorVerticalParameterHash, vertical);
    }

    private void OnMoveStop(CallbackContext callbackContext)
    {
        _movementVector = Vector2.zero;
        IsMoving = false;
        
        _animator.SetBool(_animatorIsMovingParameterHash, false);
    }
}
