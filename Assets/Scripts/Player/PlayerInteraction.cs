using UnityEngine;

namespace LooterShooter.Player
{
    public interface IInteractable
    {
        string GetInteractPrompt();
        void Interact();
    }

    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactRange = 3f;
        [SerializeField] private Transform cameraTransform;

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

        private void OnGUI()
        {
            if (_currentInteractable != null)
            {
                string prompt = _currentInteractable.GetInteractPrompt();
                GUIStyle style = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 24,
                    alignment = TextAnchor.MiddleCenter,
                    normal = { textColor = Color.white }
                };

                float width = 300;
                float height = 50;
                float x = (Screen.width - width) / 2;
                float y = Screen.height * 0.6f;

                GUI.Label(new Rect(x, y, width, height), prompt, style);
            }
        }
    }
}
