using UnityEngine;

namespace LooterShooter.Player
{
    /// <summary>
    /// Smooth camera follow for first-person view.
    /// Attach to the Camera object (not as a child of the player).
    /// </summary>
    public class PlayerCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 0.6f, 0f);

        private PlayerMovement _playerMovement;

        private void Start()
        {
            if (target == null && PlayerReference.Instance != null)
            {
                target = PlayerReference.Instance.Transform;
                _playerMovement = PlayerReference.Instance.GameObject.GetComponent<PlayerMovement>();
            }
            else if (target != null)
            {
                _playerMovement = target.GetComponent<PlayerMovement>();
            }
        }

        private void LateUpdate()
        {
            if (target == null || _playerMovement == null) return;

            transform.position = target.position + offset;
            transform.rotation = Quaternion.Euler(_playerMovement.Pitch, _playerMovement.Yaw, 0f);
        }
    }
}
