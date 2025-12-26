using System;
using System.Collections.Generic;
using UnityEngine;

namespace LooterShooter.Weapons
{
    public class WeaponManager : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private List<WeaponData> startingWeapons = new List<WeaponData>();

        private readonly List<Weapon> _weapons = new List<Weapon>();
        private int _currentWeaponIndex = -1;
        private Weapon _currentWeapon;
        private InputSystem_Actions _input;

        public Weapon CurrentWeapon => _currentWeapon;
        public int CurrentWeaponIndex => _currentWeaponIndex;
        public int WeaponCount => _weapons.Count;

        public event Action<Weapon> OnWeaponChanged;

        private void Awake()
        {
            _input = new InputSystem_Actions();

            if (cameraTransform == null)
            {
                cameraTransform = Camera.main?.transform;
            }
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

        private void Start()
        {
            foreach (var weaponData in startingWeapons)
            {
                AddWeapon(weaponData);
            }

            if (_weapons.Count > 0)
            {
                EquipWeapon(0);
            }
        }

        private void Update()
        {
            HandleInput();
        }

        private void HandleInput()
        {
            if (_currentWeapon == null) return;

            if (_input.Player.Attack.IsPressed())
            {
                if (_currentWeapon.Data.fireMode == FireMode.Automatic)
                {
                    _currentWeapon.TryFire(cameraTransform);
                }
            }

            if (_input.Player.Attack.WasPressedThisFrame())
            {
                if (_currentWeapon.Data.fireMode == FireMode.Single)
                {
                    _currentWeapon.TryFire(cameraTransform);
                }
                else if (_currentWeapon.Data.fireMode == FireMode.Burst)
                {
                    StartCoroutine(FireBurst());
                }
            }

            if (_input.Player.Next.WasPressedThisFrame())
            {
                CycleWeapon(1);
            }

            if (_input.Player.Previous.WasPressedThisFrame())
            {
                CycleWeapon(-1);
            }
        }

        private System.Collections.IEnumerator FireBurst()
        {
            for (int i = 0; i < _currentWeapon.Data.burstCount; i++)
            {
                if (!_currentWeapon.TryFire(cameraTransform))
                    break;

                yield return new WaitForSeconds(_currentWeapon.Data.burstDelay);
            }
        }

        public void AddWeapon(WeaponData weaponData)
        {
            if (weaponData == null || weaponData.weaponPrefab == null)
            {
                Debug.LogWarning("Cannot add weapon: missing data or prefab");
                return;
            }

            GameObject weaponObj = Instantiate(weaponData.weaponPrefab, weaponHolder);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;

            Weapon weapon = weaponObj.GetComponent<Weapon>();
            if (weapon == null)
            {
                weapon = AddWeaponComponent(weaponObj, weaponData.weaponType);
            }

            weapon.Initialize(weaponData);
            weapon.OnUnequip();

            _weapons.Add(weapon);
        }

        private Weapon AddWeaponComponent(GameObject obj, WeaponType type)
        {
            return type switch
            {
                WeaponType.Hitscan => obj.AddComponent<HitscanWeapon>(),
                WeaponType.Projectile => obj.AddComponent<ProjectileWeapon>(),
                WeaponType.Shotgun => obj.AddComponent<ShotgunWeapon>(),
                _ => obj.AddComponent<HitscanWeapon>()
            };
        }

        public void EquipWeapon(int index)
        {
            if (index < 0 || index >= _weapons.Count) return;
            if (index == _currentWeaponIndex) return;

            _currentWeapon?.OnUnequip();

            _currentWeaponIndex = index;
            _currentWeapon = _weapons[index];
            _currentWeapon.OnEquip();

            OnWeaponChanged?.Invoke(_currentWeapon);
        }

        public void CycleWeapon(int direction)
        {
            if (_weapons.Count <= 1) return;

            int newIndex = (_currentWeaponIndex + direction + _weapons.Count) % _weapons.Count;
            EquipWeapon(newIndex);
        }

        public void Reload()
        {
            _currentWeapon?.StartReload();
        }
    }
}
