using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LooterShooter.Item;
using LooterShooter.Weapons;

namespace LooterShooter.Player
{
    /// <summary>
    /// Handles weapon visuals and gameplay: spawning prefabs, firing, reloading.
    /// Listens to EquipmentManager for data changes.
    /// </summary>
    public class WeaponHandler : MonoBehaviour
    {
        [SerializeField] private Transform weaponHolder;
        [SerializeField] private Transform cameraTransform;

        // Spawned weapon instances keyed by slot
        private Dictionary<EquipmentSlotType, Weapon> _spawnedWeapons = new();

        // Currently active weapon
        private Weapon _currentWeapon;

        // Input
        private InputSystem_Actions _input;

        // Events
        public event Action<Weapon> OnWeaponChanged;

        // Properties
        public Weapon CurrentWeapon => _currentWeapon;

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

        private void Start()
        {
            // Subscribe to EquipmentManager events
            var em = EquipmentManager.Instance;
            if (em != null)
            {
                em.OnSlotChanged += HandleSlotChanged;
                em.OnActiveWeaponSlotChanged += HandleActiveWeaponSlotChanged;

                // Initialize with current equipment state
                RefreshActiveWeapon();
            }
        }

        private void OnDestroy()
        {
            _input?.Dispose();

            var em = EquipmentManager.Instance;
            if (em != null)
            {
                em.OnSlotChanged -= HandleSlotChanged;
                em.OnActiveWeaponSlotChanged -= HandleActiveWeaponSlotChanged;
            }
        }

        private void Update()
        {
            HandleFireInput();
            HandleWeaponSwitchInput();
        }

        #region Input Handling

        private void HandleFireInput()
        {
            if (_currentWeapon == null) return;

            // Automatic fire - hold to fire
            if (_input.Player.Attack.IsPressed())
            {
                if (_currentWeapon.Data.fireMode == FireMode.Automatic)
                {
                    _currentWeapon.TryFire(cameraTransform);
                }
            }

            // Single/Burst - press to fire
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

            // Reload
            if (_input.Player.Reload.WasPressedThisFrame())
            {
                _currentWeapon.StartReload();
            }
        }

        private void HandleWeaponSwitchInput()
        {
            var em = EquipmentManager.Instance;
            if (em == null) return;

            // Cycle weapons with scroll/next/previous
            if (_input.Player.Next.WasPressedThisFrame())
            {
                em.CycleWeaponSlot(1);
            }

            if (_input.Player.Previous.WasPressedThisFrame())
            {
                em.CycleWeaponSlot(-1);
            }

            // Direct slot selection (1, 2, 3 keys) - requires input actions to be set up
            // For now, this can be handled by extending the input system
        }

        private IEnumerator FireBurst()
        {
            for (int i = 0; i < _currentWeapon.Data.burstCount; i++)
            {
                if (!_currentWeapon.TryFire(cameraTransform))
                    break;

                yield return new WaitForSeconds(_currentWeapon.Data.burstDelay);
            }
        }

        #endregion

        #region Equipment Event Handlers

        private void HandleSlotChanged(EquipmentSlotType slot, EquipmentInstance item)
        {
            // Only handle weapon slots
            if (!EquipmentManager.IsWeaponSlot(slot)) return;

            // Destroy old weapon if it exists
            if (_spawnedWeapons.TryGetValue(slot, out var oldWeapon))
            {
                if (oldWeapon != null)
                {
                    Destroy(oldWeapon.gameObject);
                }
                _spawnedWeapons.Remove(slot);
            }

            // Spawn new weapon if item exists
            if (item != null && item.Data is WeaponEquipmentData weaponEquip)
            {
                SpawnWeapon(slot, weaponEquip);
            }

            // Refresh active weapon if this was the active slot
            if (slot == EquipmentManager.Instance.ActiveWeaponSlot)
            {
                RefreshActiveWeapon();
            }
        }

        private void HandleActiveWeaponSlotChanged(EquipmentSlotType newSlot)
        {
            RefreshActiveWeapon();
        }

        #endregion

        #region Weapon Spawning

        private void SpawnWeapon(EquipmentSlotType slot, WeaponEquipmentData equipData)
        {
            if (equipData.weaponData == null || equipData.weaponData.weaponPrefab == null)
            {
                Debug.LogWarning($"Cannot spawn weapon: missing WeaponData or prefab for {equipData.itemName}");
                return;
            }

            WeaponData weaponData = equipData.weaponData;

            // Instantiate prefab
            GameObject weaponObj = Instantiate(weaponData.weaponPrefab, weaponHolder);
            weaponObj.transform.localPosition = Vector3.zero;
            weaponObj.transform.localRotation = Quaternion.identity;

            // Get or add weapon component
            Weapon weapon = weaponObj.GetComponent<Weapon>();
            if (weapon == null)
            {
                weapon = AddWeaponComponent(weaponObj, weaponData.weaponType);
            }

            weapon.Initialize(weaponData);
            weapon.OnUnequip(); // Start hidden

            _spawnedWeapons[slot] = weapon;
        }

        private Weapon AddWeaponComponent(GameObject obj, WeaponType type)
        {
            return type switch
            {
                WeaponType.Projectile => obj.AddComponent<ProjectileWeapon>(),
                WeaponType.Shotgun => obj.AddComponent<ProjectileShotgunWeapon>(),
                WeaponType.Sniper => obj.AddComponent<SniperWeapon>(),
                _ => obj.AddComponent<ProjectileWeapon>()
            };
        }

        private void RefreshActiveWeapon()
        {
            var em = EquipmentManager.Instance;
            if (em == null) return;

            // Unequip current weapon visually
            _currentWeapon?.OnUnequip();

            // Get the weapon in the active slot
            EquipmentSlotType activeSlot = em.ActiveWeaponSlot;
            _spawnedWeapons.TryGetValue(activeSlot, out _currentWeapon);

            // Equip new weapon visually
            _currentWeapon?.OnEquip();

            OnWeaponChanged?.Invoke(_currentWeapon);
        }

        #endregion

        #region Public API

        /// <summary>
        /// Triggers a reload on the current weapon.
        /// </summary>
        public void Reload()
        {
            _currentWeapon?.StartReload();
        }

        #endregion
    }
}
