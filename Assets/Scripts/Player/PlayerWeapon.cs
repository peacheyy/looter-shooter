using UnityEngine;
using LooterShooter.Weapons;

namespace LooterShooter.Player
{
    [RequireComponent(typeof(WeaponManager))]
    public class PlayerWeapon : MonoBehaviour
    {
        private WeaponManager _weaponManager;
        private InputSystem_Actions _input;

        public WeaponManager WeaponManager => _weaponManager;

        private void Awake()
        {
            _weaponManager = GetComponent<WeaponManager>();
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

        private void OnDestroy()
        {
            _input?.Dispose();
        }

        private void Update()
        {
            if (_input.Player.Reload.WasPressedThisFrame())
            {
                _weaponManager.Reload();
            }
        }
    }
}
