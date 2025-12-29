using System;
using System.Collections.Generic;
using UnityEngine;
using LooterShooter.Item;

namespace LooterShooter.Player
{
    /// <summary>
    /// Manages equipment data: what's equipped in slots and what's in the backpack.
    /// This is DATA ONLY - no weapon spawning or visuals. See WeaponHandler for that.
    /// </summary>
    public class EquipmentManager : MonoBehaviour, IItemStorage<EquipmentInstance>
    {
        public static EquipmentManager Instance { get; private set; }

        [Header("Backpack Settings")]
        [SerializeField] private int backpackCapacity = 20;

        // Equipped items by slot
        private Dictionary<EquipmentSlotType, EquipmentInstance> _equippedItems = new();

        // Backpack storage
        private List<EquipmentInstance> _backpack = new();

        // Currently active weapon slot (Primary/Special/Heavy)
        private EquipmentSlotType _activeWeaponSlot = EquipmentSlotType.Primary;

        // Events
        public event Action<EquipmentSlotType, EquipmentInstance> OnSlotChanged;
        public event Action OnBackpackChanged;
        public event Action<EquipmentSlotType> OnActiveWeaponSlotChanged;

        // IItemStorage implementation (for backpack)
        public event Action OnChanged;
        public bool IsFull => _backpack.Count >= backpackCapacity;
        public int Count => _backpack.Count;
        public int Capacity => backpackCapacity;

        // Properties
        public EquipmentSlotType ActiveWeaponSlot => _activeWeaponSlot;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        #region IItemStorage Implementation

        /// <summary>
        /// Adds an item to the backpack.
        /// </summary>
        public bool Add(EquipmentInstance item)
        {
            if (item == null || IsFull)
            {
                return false;
            }

            _backpack.Add(item);
            OnBackpackChanged?.Invoke();
            OnChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Removes an item from the backpack.
        /// </summary>
        public bool Remove(EquipmentInstance item)
        {
            if (item == null)
            {
                return false;
            }

            bool removed = _backpack.Remove(item);
            if (removed)
            {
                OnBackpackChanged?.Invoke();
                OnChanged?.Invoke();
            }
            return removed;
        }

        #endregion

        #region Equipment Slot Management

        /// <summary>
        /// Gets the item currently equipped in a slot.
        /// </summary>
        public EquipmentInstance GetEquipped(EquipmentSlotType slot)
        {
            return _equippedItems.GetValueOrDefault(slot);
        }

        /// <summary>
        /// Gets the currently active weapon (in the active weapon slot).
        /// </summary>
        public EquipmentInstance GetActiveWeapon()
        {
            return GetEquipped(_activeWeaponSlot);
        }

        /// <summary>
        /// Equips an item from the backpack to the appropriate slot.
        /// If the slot is occupied, the old item moves to backpack.
        /// </summary>
        public bool Equip(EquipmentInstance item)
        {
            if (item == null || item.Data == null)
            {
                return false;
            }

            EquipmentSlotType targetSlot = item.Data.slotType;
            if (targetSlot == EquipmentSlotType.None)
            {
                return false;
            }

            // Remove from backpack if present
            _backpack.Remove(item);

            // If slot is occupied, move old item to backpack
            if (_equippedItems.TryGetValue(targetSlot, out var currentlyEquipped))
            {
                _backpack.Add(currentlyEquipped);
            }

            // Equip the new item
            _equippedItems[targetSlot] = item;

            OnSlotChanged?.Invoke(targetSlot, item);
            OnBackpackChanged?.Invoke();
            OnChanged?.Invoke();

            return true;
        }

        /// <summary>
        /// Unequips an item from a slot and moves it to the backpack.
        /// </summary>
        public bool Unequip(EquipmentSlotType slot)
        {
            if (!_equippedItems.TryGetValue(slot, out var item))
            {
                return false;
            }

            if (IsFull)
            {
                Debug.Log("Cannot unequip: backpack is full");
                return false;
            }

            _equippedItems.Remove(slot);
            _backpack.Add(item);

            OnSlotChanged?.Invoke(slot, null);
            OnBackpackChanged?.Invoke();
            OnChanged?.Invoke();

            return true;
        }

        /// <summary>
        /// Switches the active weapon slot (Primary/Special/Heavy).
        /// </summary>
        public void SwitchActiveWeaponSlot(EquipmentSlotType slot)
        {
            if (!IsWeaponSlot(slot))
            {
                return;
            }

            if (_activeWeaponSlot == slot)
            {
                return;
            }

            _activeWeaponSlot = slot;
            OnActiveWeaponSlotChanged?.Invoke(slot);
        }

        /// <summary>
        /// Cycles to the next weapon slot that has a weapon equipped.
        /// </summary>
        public void CycleWeaponSlot(int direction)
        {
            EquipmentSlotType[] weaponSlots = { EquipmentSlotType.Primary, EquipmentSlotType.Special, EquipmentSlotType.Heavy };
            int currentIndex = Array.IndexOf(weaponSlots, _activeWeaponSlot);

            // Find next slot with a weapon
            for (int i = 1; i <= weaponSlots.Length; i++)
            {
                int nextIndex = (currentIndex + i * direction + weaponSlots.Length) % weaponSlots.Length;
                EquipmentSlotType nextSlot = weaponSlots[nextIndex];

                if (_equippedItems.ContainsKey(nextSlot))
                {
                    SwitchActiveWeaponSlot(nextSlot);
                    return;
                }
            }
        }

        #endregion

        #region Backpack Access

        /// <summary>
        /// Gets a copy of all items in the backpack.
        /// </summary>
        public List<EquipmentInstance> GetBackpackItems()
        {
            return new List<EquipmentInstance>(_backpack);
        }

        /// <summary>
        /// Gets the item at a specific backpack index.
        /// </summary>
        public EquipmentInstance GetBackpackItem(int index)
        {
            if (index < 0 || index >= _backpack.Count)
            {
                return null;
            }
            return _backpack[index];
        }

        #endregion

        #region Utility

        /// <summary>
        /// Checks if a slot type is a weapon slot.
        /// </summary>
        public static bool IsWeaponSlot(EquipmentSlotType slot)
        {
            return slot == EquipmentSlotType.Primary ||
                   slot == EquipmentSlotType.Special ||
                   slot == EquipmentSlotType.Heavy;
        }

        /// <summary>
        /// Checks if a slot type is an armor slot.
        /// </summary>
        public static bool IsArmorSlot(EquipmentSlotType slot)
        {
            return slot == EquipmentSlotType.Helmet ||
                   slot == EquipmentSlotType.Gauntlets ||
                   slot == EquipmentSlotType.Chest ||
                   slot == EquipmentSlotType.Boots;
        }

        #endregion
    }
}
