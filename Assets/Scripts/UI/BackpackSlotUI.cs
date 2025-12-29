using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LooterShooter.Player;
using LooterShooter.Item;

namespace LooterShooter.UI
{
    /// <summary>
    /// UI component for a backpack inventory slot.
    /// Click to equip items to their appropriate slot.
    /// </summary>
    public class BackpackSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image rarityBorder;

        [Header("Colors")]
        [SerializeField] private Color emptyColor = new Color(0.15f, 0.15f, 0.15f, 0.5f);
        [SerializeField] private Color filledColor = new Color(0.25f, 0.25f, 0.25f, 0.8f);
        [SerializeField] private Color highlightColor = new Color(0.35f, 0.35f, 0.35f, 1f);

        private int _slotIndex;
        private EquipmentInstance _currentItem;

        public int SlotIndex => _slotIndex;
        public EquipmentInstance CurrentItem => _currentItem;

        /// <summary>
        /// Initializes the slot with its index.
        /// </summary>
        public void Initialize(int index)
        {
            _slotIndex = index;
            ClearSlot();
        }

        /// <summary>
        /// Sets the item displayed in this slot.
        /// </summary>
        public void SetItem(EquipmentInstance item)
        {
            _currentItem = item;

            if (item == null || item.Data == null)
            {
                ClearSlot();
                return;
            }

            // Show icon
            if (iconImage != null)
            {
                iconImage.sprite = item.Data.icon;
                iconImage.enabled = item.Data.icon != null;
                iconImage.color = Color.white;
            }

            // Set rarity border color
            if (rarityBorder != null)
            {
                rarityBorder.color = item.Data.GetRarityColor();
                rarityBorder.enabled = true;
            }

            // Update background
            if (backgroundImage != null)
            {
                backgroundImage.color = filledColor;
            }
        }

        /// <summary>
        /// Clears the slot display.
        /// </summary>
        public void ClearSlot()
        {
            _currentItem = null;

            if (iconImage != null)
            {
                iconImage.sprite = null;
                iconImage.enabled = false;
            }

            if (rarityBorder != null)
            {
                rarityBorder.enabled = false;
            }

            if (backgroundImage != null)
            {
                backgroundImage.color = emptyColor;
            }
        }

        #region Event Handlers

        public void OnPointerClick(PointerEventData eventData)
        {
            // Click to equip - item goes to its designated slot
            if (_currentItem == null) return;

            var em = EquipmentManager.Instance;
            if (em != null)
            {
                em.Equip(_currentItem);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = highlightColor;
            }

            // TODO: Show tooltip
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (backgroundImage != null)
            {
                backgroundImage.color = _currentItem != null ? filledColor : emptyColor;
            }

            // TODO: Hide tooltip
        }

        #endregion
    }
}
