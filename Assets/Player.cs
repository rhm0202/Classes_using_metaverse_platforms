using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody), typeof(SphereCollider))]
public class Player : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float torque = 10f;
    [SerializeField] private float maxSpeed = 5f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float groundCheckDistance = 0.55f;
    [SerializeField] private LayerMask groundMask;

    private Rigidbody _rb;
    private Vector2 _moveInput;
    private bool _jumpPressed;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ReadInput();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        HandleJump();
    }

    private void ReadInput()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null)
        {
            _moveInput = Vector2.zero;
            _jumpPressed = false;
            return;
        }

        float horizontal = 0f;
        float vertical = 0f;

        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) horizontal -= 1f;
        if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) horizontal += 1f;

        if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) vertical -= 1f;
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) vertical += 1f;

        _moveInput = new Vector2(horizontal, vertical);
        _moveInput = Vector2.ClampMagnitude(_moveInput, 1f);

        if (keyboard.spaceKey.wasPressedThisFrame)
        {
            _jumpPressed = true;
        }
    }

    private void HandleMovement()
    {
        // 입력 방향에 따라 공을 회전시키는 토크를 가함 (굴러가는 느낌)
        Vector3 torqueAxis = new Vector3(_moveInput.y, 0f, -_moveInput.x);
        _rb.AddTorque(torqueAxis * torque, ForceMode.Force);

        // 너무 빨라지지 않게 속도 클램프
        Vector3 vel = _rb.linearVelocity;
        Vector3 horizontalVel = new Vector3(vel.x, 0f, vel.z);
        if (horizontalVel.magnitude > maxSpeed)
        {
            horizontalVel = horizontalVel.normalized * maxSpeed;
            _rb.linearVelocity = new Vector3(horizontalVel.x, vel.y, horizontalVel.z);
        }
    }

    private void HandleJump()
    {
        if (_jumpPressed && IsGrounded())
        {
            Vector3 vel = _rb.linearVelocity;
            vel.y = 0f;
            _rb.linearVelocity = vel;
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        _jumpPressed = false;
    }

    private bool IsGrounded()
    {
        // 구 중심에서 아래로 레이캐스트해서 바닥 체크
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);
    }
}


