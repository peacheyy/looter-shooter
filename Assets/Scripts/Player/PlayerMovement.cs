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
        [SerializeField] private float mouseSensitivity = 100f;
        [SerializeField] private float maxLookAngle = 85f;
        [SerializeField] private Transform cameraTransform;

        private CharacterController _controller;
        private InputSystem_Actions _input;
        private Vector3 _velocity;
        private float _cameraPitch;
        private bool _jumpPressed;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = new InputSystem_Actions();
        }

        private void OnEnable()
        {
            _input.Enable();
            _input.Player.Jump.performed += OnJump;
        }

        private void OnDisable()
        {
            _input.Player.Jump.performed -= OnJump;
            _input.Disable();
        }

        private void OnJump(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            _jumpPressed = true;
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (cameraTransform == null)
            {
                cameraTransform = Camera.main?.transform;
            }
        }

        private void Update()
        {
            HandleMouseLook();
            HandleMovement();
        }

        private void HandleMouseLook()
        {
            if (cameraTransform == null) return;

            Vector2 lookInput = _input.Player.Look.ReadValue<Vector2>();

            // Multiply by deltaTime for frame-rate independent look
            float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
            float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

            // Horizontal rotation (rotate the player body)
            transform.Rotate(Vector3.up, mouseX);

            // Vertical rotation (rotate the camera)
            _cameraPitch -= mouseY;
            _cameraPitch = Mathf.Clamp(_cameraPitch, -maxLookAngle, maxLookAngle);
            cameraTransform.localRotation = Quaternion.Euler(_cameraPitch, 0f, 0f);
        }

        private void HandleMovement()
        {
            // Ground check
            bool isGrounded = _controller.isGrounded;
            if (isGrounded && _velocity.y < 0f)
            {
                _velocity.y = -2f; // Small downward force to keep grounded
            }

            // Read input directly from wrapper class
            Vector2 moveInput = _input.Player.Move.ReadValue<Vector2>();
            bool sprintHeld = _input.Player.Sprint.IsPressed();

            // Calculate movement direction
            float currentSpeed = sprintHeld ? sprintSpeed : walkSpeed;
            Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
            _controller.Move(move * (currentSpeed * Time.deltaTime));

            // Jumping
            if (_jumpPressed && isGrounded)
            {
                _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                _jumpPressed = false;
            }

            // Apply gravity
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
