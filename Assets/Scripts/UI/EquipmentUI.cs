using UnityEngine;
using UnityEngine.InputSystem;
using LooterShooter.Player;
using LooterShooter.Item;

namespace LooterShooter.UI
{
    /// <summary>
    /// Main controller for the equipment inventory UI.
    /// Handles Tab toggle, cursor lock/unlock, and panel visibility.
    /// </summary>
    public class EquipmentUI : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private CanvasGroup canvasGroup;

        [Header("Slot References")]
        [SerializeField] private EquipmentSlotUI primarySlot;
        [SerializeField] private EquipmentSlotUI specialSlot;
        [SerializeField] private EquipmentSlotUI heavySlot;
        [SerializeField] private EquipmentSlotUI helmetSlot;
        [SerializeField] private EquipmentSlotUI gauntletsSlot;
        [SerializeField] private EquipmentSlotUI chestSlot;
        [SerializeField] private EquipmentSlotUI bootsSlot;

        [Header("Backpack")]
        [SerializeField] private BackpackUI backpackUI;

        private bool _isOpen = false;

        public static EquipmentUI Instance { get; private set; }
        public bool IsOpen => _isOpen;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            // Initialize slot types
            InitializeSlots();

            // Subscribe to equipment manager events
            var em = EquipmentManager.Instance;
            if (em != null)
            {
                em.OnSlotChanged += HandleSlotChanged;
                em.OnBackpackChanged += HandleBackpackChanged;
            }

            // Start closed
            CloseInventory();
        }

        private void OnDestroy()
        {
            var em = EquipmentManager.Instance;
            if (em != null)
            {
                em.OnSlotChanged -= HandleSlotChanged;
                em.OnBackpackChanged -= HandleBackpackChanged;
            }
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            {
                ToggleInventory();
            }
        }

        private void InitializeSlots()
        {
            if (primarySlot != null) primarySlot.Initialize(EquipmentSlotType.Primary);
            if (specialSlot != null) specialSlot.Initialize(EquipmentSlotType.Special);
            if (heavySlot != null) heavySlot.Initialize(EquipmentSlotType.Heavy);
            if (helmetSlot != null) helmetSlot.Initialize(EquipmentSlotType.Helmet);
            if (gauntletsSlot != null) gauntletsSlot.Initialize(EquipmentSlotType.Gauntlets);
            if (chestSlot != null) chestSlot.Initialize(EquipmentSlotType.Chest);
            if (bootsSlot != null) bootsSlot.Initialize(EquipmentSlotType.Boots);
        }

        #region Inventory Toggle

        public void ToggleInventory()
        {
            if (_isOpen)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }

        public void OpenInventory()
        {
            _isOpen = true;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(true);
            }

            // Unlock cursor for UI interaction
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            // Refresh UI state
            RefreshAllSlots();
            backpackUI?.Refresh();
        }

        public void CloseInventory()
        {
            _isOpen = false;

            if (inventoryPanel != null)
            {
                inventoryPanel.SetActive(false);
            }

            // Lock cursor for gameplay
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        #endregion

        #region Event Handlers

        private void HandleSlotChanged(EquipmentSlotType slot, EquipmentInstance item)
        {
            if (!_isOpen) return;

            // Update the specific slot
            EquipmentSlotUI slotUI = GetSlotUI(slot);
            if (slotUI != null)
            {
                slotUI.SetItem(item);
            }
        }

        private void HandleBackpackChanged()
        {
            if (!_isOpen) return;
            backpackUI?.Refresh();
        }

        #endregion

        #region Slot Management

        private EquipmentSlotUI GetSlotUI(EquipmentSlotType slot)
        {
            return slot switch
            {
                EquipmentSlotType.Primary => primarySlot,
                EquipmentSlotType.Special => specialSlot,
                EquipmentSlotType.Heavy => heavySlot,
                EquipmentSlotType.Helmet => helmetSlot,
                EquipmentSlotType.Gauntlets => gauntletsSlot,
                EquipmentSlotType.Chest => chestSlot,
                EquipmentSlotType.Boots => bootsSlot,
                _ => null
            };
        }

        private void RefreshAllSlots()
        {
            var em = EquipmentManager.Instance;
            if (em == null) return;

            RefreshSlot(primarySlot, EquipmentSlotType.Primary);
            RefreshSlot(specialSlot, EquipmentSlotType.Special);
            RefreshSlot(heavySlot, EquipmentSlotType.Heavy);
            RefreshSlot(helmetSlot, EquipmentSlotType.Helmet);
            RefreshSlot(gauntletsSlot, EquipmentSlotType.Gauntlets);
            RefreshSlot(chestSlot, EquipmentSlotType.Chest);
            RefreshSlot(bootsSlot, EquipmentSlotType.Boots);
        }

        private void RefreshSlot(EquipmentSlotUI slotUI, EquipmentSlotType slotType)
        {
            if (slotUI == null) return;

            var em = EquipmentManager.Instance;
            var item = em?.GetEquipped(slotType);
            slotUI.SetItem(item);
        }

        #endregion

    }
}
