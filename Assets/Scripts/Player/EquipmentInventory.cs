using System;
using System.Collections.Generic;
using UnityEngine;
using LooterShooter.Item;
using LooterShooter.Weapons;

namespace LooterShooter.Player
{
    /// <summary>
    /// Destiny 1-style equipment inventory with 3 weapon slots and 4 armor slots.
    /// </summary>
    public class EquipmentInventory : MonoBehaviour
    {
        public static EquipmentInventory Instance { get; private set; }

        private Dictionary<WeaponSlot, WeaponData> _equippedWeapons = new Dictionary<WeaponSlot, WeaponData>();
        private Dictionary<ArmorSlot, ArmorData> _equippedArmor = new Dictionary<ArmorSlot, ArmorData>();

        public event Action OnEquipmentChanged;
        public event Action<WeaponSlot, WeaponData> OnWeaponEquipped;
        public event Action<ArmorSlot, ArmorData> OnArmorEquipped;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeSlots();
        }

        private void InitializeSlots()
        {
            foreach (WeaponSlot slot in Enum.GetValues(typeof(WeaponSlot)))
            {
                _equippedWeapons[slot] = null;
            }

            foreach (ArmorSlot slot in Enum.GetValues(typeof(ArmorSlot)))
            {
                _equippedArmor[slot] = null;
            }
        }

        /// <summary>
        /// Equips a weapon to its designated slot. Returns the previously equipped weapon (if any).
        /// </summary>
        public WeaponData EquipWeapon(WeaponData weapon)
        {
            if (weapon == null) return null;

            WeaponSlot slot = weapon.weaponSlot;
            WeaponData previousWeapon = _equippedWeapons[slot];
            _equippedWeapons[slot] = weapon;

            Debug.Log($"Equipped {weapon.weaponName} to {slot} slot");

            OnWeaponEquipped?.Invoke(slot, weapon);
            OnEquipmentChanged?.Invoke();

            return previousWeapon;
        }

        /// <summary>
        /// Removes the weapon from a specific slot.
        /// </summary>
        public WeaponData UnequipWeapon(WeaponSlot slot)
        {
            WeaponData weapon = _equippedWeapons[slot];
            _equippedWeapons[slot] = null;

            if (weapon != null)
            {
                Debug.Log($"Unequipped {weapon.weaponName} from {slot} slot");
                OnWeaponEquipped?.Invoke(slot, null);
                OnEquipmentChanged?.Invoke();
            }

            return weapon;
        }

        /// <summary>
        /// Gets the weapon equipped in a specific slot.
        /// </summary>
        public WeaponData GetWeapon(WeaponSlot slot)
        {
            return _equippedWeapons.TryGetValue(slot, out WeaponData weapon) ? weapon : null;
        }

        /// <summary>
        /// Gets all equipped weapons.
        /// </summary>
        public Dictionary<WeaponSlot, WeaponData> GetAllWeapons()
        {
            return new Dictionary<WeaponSlot, WeaponData>(_equippedWeapons);
        }

        /// <summary>
        /// Equips armor to its designated slot. Returns the previously equipped armor (if any).
        /// </summary>
        public ArmorData EquipArmor(ArmorData armor)
        {
            if (armor == null) return null;

            ArmorSlot slot = armor.armorSlot;
            ArmorData previousArmor = _equippedArmor[slot];
            _equippedArmor[slot] = armor;

            Debug.Log($"Equipped {armor.armorName} to {slot} slot");

            OnArmorEquipped?.Invoke(slot, armor);
            OnEquipmentChanged?.Invoke();

            return previousArmor;
        }

        /// <summary>
        /// Removes armor from a specific slot.
        /// </summary>
        public ArmorData UnequipArmor(ArmorSlot slot)
        {
            ArmorData armor = _equippedArmor[slot];
            _equippedArmor[slot] = null;

            if (armor != null)
            {
                Debug.Log($"Unequipped {armor.armorName} from {slot} slot");
                OnArmorEquipped?.Invoke(slot, null);
                OnEquipmentChanged?.Invoke();
            }

            return armor;
        }

        /// <summary>
        /// Gets the armor equipped in a specific slot.
        /// </summary>
        public ArmorData GetArmor(ArmorSlot slot)
        {
            return _equippedArmor.TryGetValue(slot, out ArmorData armor) ? armor : null;
        }

        /// <summary>
        /// Gets all equipped armor.
        /// </summary>
        public Dictionary<ArmorSlot, ArmorData> GetAllArmor()
        {
            return new Dictionary<ArmorSlot, ArmorData>(_equippedArmor);
        }
    }
}
