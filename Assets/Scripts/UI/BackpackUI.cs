using System.Collections.Generic;
using UnityEngine;
using LooterShooter.Player;
using LooterShooter.Item;

namespace LooterShooter.UI
{
    /// <summary>
    /// Manages the backpack grid UI.
    /// Creates and updates slots based on EquipmentManager backpack state.
    /// </summary>
    public class BackpackUI : MonoBehaviour
    {
        [Header("Grid Settings")]
        [SerializeField] private int columns = 5;
        [SerializeField] private int rows = 4;

        [Header("Prefab")]
        [SerializeField] private GameObject slotPrefab;

        [Header("Container")]
        [SerializeField] private Transform gridContainer;

        private List<BackpackSlotUI> _slots = new List<BackpackSlotUI>();

        private void Start()
        {
            CreateGrid();
        }

        /// <summary>
        /// Creates the backpack grid slots.
        /// </summary>
        private void CreateGrid()
        {
            // Clear existing slots
            foreach (var slot in _slots)
            {
                if (slot != null)
                {
                    Destroy(slot.gameObject);
                }
            }
            _slots.Clear();

            // Validate prefab
            if (slotPrefab == null)
            {
                Debug.LogWarning("BackpackUI: No slot prefab assigned");
                return;
            }

            // Create slots
            int totalSlots = columns * rows;
            for (int i = 0; i < totalSlots; i++)
            {
                GameObject slotObj = Instantiate(slotPrefab, gridContainer);
                slotObj.name = $"BackpackSlot_{i}";

                BackpackSlotUI slot = slotObj.GetComponent<BackpackSlotUI>();
                if (slot == null)
                {
                    slot = slotObj.AddComponent<BackpackSlotUI>();
                }

                slot.Initialize(i);
                _slots.Add(slot);
            }
        }

        /// <summary>
        /// Refreshes the backpack display with current items.
        /// </summary>
        public void Refresh()
        {
            var em = EquipmentManager.Instance;
            if (em == null) return;

            List<EquipmentInstance> items = em.GetBackpackItems();

            // Update each slot
            for (int i = 0; i < _slots.Count; i++)
            {
                if (i < items.Count)
                {
                    _slots[i].SetItem(items[i]);
                }
                else
                {
                    _slots[i].ClearSlot();
                }
            }
        }

        /// <summary>
        /// Gets the slot at the specified index.
        /// </summary>
        public BackpackSlotUI GetSlot(int index)
        {
            if (index < 0 || index >= _slots.Count)
            {
                return null;
            }
            return _slots[index];
        }
    }
}
