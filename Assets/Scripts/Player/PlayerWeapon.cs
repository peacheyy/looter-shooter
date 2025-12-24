using UnityEngine;

namespace LooterShooter.Player
{
    public class PlayerWeapon : MonoBehaviour
    {
        [Header("Weapon Settings")]
        [SerializeField] private float damage = 10f;
        [SerializeField] private float range = 100f;
        [SerializeField] private float fireRate = 10f;

        [Header("References")]
        [SerializeField] private Transform cameraTransform;

        private InputSystem_Actions _inputActions;
        private float _nextFireTime;

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
            if (_inputActions.Player.Attack.IsPressed() && Time.time >= _nextFireTime)
            {
                Shoot();
                _nextFireTime = Time.time + 1f / fireRate;
            }
        }

        private void Shoot()
        {
            if (cameraTransform == null) return;

            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, range))
            {
                Debug.Log($"Hit: {hit.collider.name} at {hit.point}");

                var damageable = hit.collider.GetComponent<IDamageable>();
                damageable?.TakeDamage(damage);
            }

            Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 0.1f);
        }
    }

    public interface IDamageable
    {
        void TakeDamage(float amount);
    }
}
