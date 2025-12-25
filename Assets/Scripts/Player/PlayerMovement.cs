using UnityEngine;

namespace LooterShooter.Player
{
    /// <summary>
    /// Handles first-person player movement including walking, sprinting, jumping, and mouse look.
    /// Requires a CharacterController component on the same GameObject.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float walkSpeed = 5f;
        [SerializeField] private float sprintSpeed = 8f;
        [SerializeField] private float gravity = -15f;
        [SerializeField] private float jumpHeight = 1.5f;

        [Header("Mouse Look")]
        [SerializeField] private float mouseSensitivity = 0.35f;
        [SerializeField] private float maxLookAngle = 85f;

        private CharacterController _controller;
        private InputSystem_Actions _input;
        private Vector3 _velocity;
        private float _yaw;
        private float _pitch;
        private bool _jumpPressed;

        private Vector2 _moveInput;
        private bool _sprintHeld;

        public float Yaw => _yaw;
        public float Pitch => _pitch;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _yaw = transform.eulerAngles.y;
        }

        private void Update()
        {
            HandleMouseLook();

            if (_input.Player.Jump.WasPressedThisFrame())
            {
                _jumpPressed = true;
            }

            _moveInput = _input.Player.Move.ReadValue<Vector2>();
            _sprintHeld = _input.Player.Sprint.IsPressed();

            HandleMovement();
        }

        private void HandleMouseLook()
        {
            Vector2 lookInput = _input.Player.Look.ReadValue<Vector2>();

            _yaw += lookInput.x * mouseSensitivity;
            _pitch -= lookInput.y * mouseSensitivity;
            _pitch = Mathf.Clamp(_pitch, -maxLookAngle, maxLookAngle);

            transform.rotation = Quaternion.Euler(0f, _yaw, 0f);
        }

        private void HandleMovement()
        {
            bool isGrounded = _controller.isGrounded;
            if (isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f;
            }

            float currentSpeed = _sprintHeld ? sprintSpeed : walkSpeed;
            Vector3 move = transform.right * _moveInput.x + transform.forward * _moveInput.y;
            _controller.Move(move * (currentSpeed * Time.deltaTime));

            if (_jumpPressed && isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _jumpPressed = false;
            }

            _velocity.y += gravity * Time.deltaTime;
            _controller.Move(_velocity * Time.deltaTime);
        }

        private void OnDestroy()
        {
            _input?.Dispose();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
