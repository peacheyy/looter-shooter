using UnityEngine;
using TMPro;

namespace LooterShooter.Player
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private TextMeshProUGUI interactPromptText;

        private InputSystem_Actions _inputActions;
        private IInteractable _currentInteractable;

        private void Awake()
        {
            _inputActions = new InputSystem_Actions();

            if (cameraTransform == null)
            {
                cameraTransform = Camera.main?.transform;
            }
        }

        private void OnEnable()
        {
            _inputActions.Player.Enable();
        }

        private void OnDisable()
        {
            _inputActions.Player.Disable();
        }

        private void OnDestroy()
        {
            _inputActions?.Dispose();
        }

        private void Update()
        {
            CheckForInteractable();

            if (_currentInteractable != null && _inputActions.Player.Interact.WasPressedThisFrame())
            {
                _currentInteractable.Interact();
                _currentInteractable = null;
            }

            UpdatePromptUI();
        }

        private void CheckForInteractable()
        {
            _currentInteractable = null;

            if (cameraTransform == null) return;

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, interactRange))
            {
                _currentInteractable = hit.collider.GetComponent<IInteractable>();
            }
        }

        private void UpdatePromptUI()
        {
            if (interactPromptText == null) return;

            if (_currentInteractable != null)
            {
                interactPromptText.text = _currentInteractable.GetInteractPrompt();
                interactPromptText.gameObject.SetActive(true);
            }
            else
            {
                interactPromptText.gameObject.SetActive(false);
            }
        }
    }
}
