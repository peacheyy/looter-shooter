using UnityEngine;

namespace LooterShooter.Weapons
{
    public class SniperWeapon : Weapon
    {
        [SerializeField] private GameObject weaponModel;

        private Camera _mainCamera;
        private float _defaultFOV;
        private bool _isScoped;
        private InputSystem_Actions _input;
        private Renderer[] _renderers;

        public bool IsScoped => _isScoped;

        protected override void Awake()
        {
            base.Awake();
            _input = new InputSystem_Actions();
            _mainCamera = Camera.main;
            if (_mainCamera != null)
            {
                _defaultFOV = _mainCamera.fieldOfView;
            }

            // Cache renderers for hiding when scoped
            if (weaponModel != null)
                _renderers = weaponModel.GetComponentsInChildren<Renderer>();
            else
                _renderers = GetComponentsInChildren<Renderer>();
        }

        private void OnEnable()
        {
            _input.Enable();
        }

        private void OnDisable()
        {
            _input.Disable();
            ExitScope();
        }

        private void OnDestroy()
        {
            _input?.Dispose();
        }

        protected override void Update()
        {
            base.Update();
            HandleScopeInput();
        }

        private void HandleScopeInput()
        {
            if (weaponData == null || !weaponData.hasScope) return;

            if (_input.Player.Aim.WasPressedThisFrame())
            {
                ToggleScope();
            }
        }

        private void ToggleScope()
        {
            if (_isScoped)
                ExitScope();
            else
                EnterScope();
        }

        private void EnterScope()
        {
            if (_mainCamera == null || weaponData == null) return;

            _isScoped = true;
            _mainCamera.fieldOfView = weaponData.scopedFOV;
            SetWeaponVisible(false);
        }

        private void ExitScope()
        {
            if (_mainCamera == null) return;

            _isScoped = false;
            _mainCamera.fieldOfView = _defaultFOV;
            SetWeaponVisible(true);
        }

        private void SetWeaponVisible(bool visible)
        {
            if (_renderers == null) return;

            foreach (var renderer in _renderers)
            {
                if (renderer != null)
                    renderer.enabled = visible;
            }
        }

        protected override void Fire(Transform cameraTransform)
        {
            if (weaponData.projectilePrefab == null)
            {
                Debug.LogWarning($"[{weaponData.weaponName}] No projectile prefab assigned!");
                return;
            }

            Vector3 spawnPosition = firePoint != null ? firePoint.position : cameraTransform.position;
            Vector3 direction = ApplySpread(cameraTransform.forward);

            GameObject projectile = Instantiate(
                weaponData.projectilePrefab,
                spawnPosition,
                Quaternion.LookRotation(direction)
            );

            if (projectile.TryGetComponent<Projectile>(out var proj))
            {
                proj.Initialize(direction, weaponData.projectileSpeed, weaponData.damage, weaponData.range);
            }
            else if (projectile.TryGetComponent<Rigidbody>(out var rb))
            {
                rb.linearVelocity = direction * weaponData.projectileSpeed;
                Destroy(projectile, weaponData.range / weaponData.projectileSpeed);
            }
        }

        public override void OnUnequip()
        {
            ExitScope();
            base.OnUnequip();
        }
    }
}
