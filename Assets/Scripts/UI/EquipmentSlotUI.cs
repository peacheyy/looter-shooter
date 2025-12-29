using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using LooterShooter.Player;
using LooterShooter.Item;

namespace LooterShooter.UI
{
    /// <summary>
    /// UI component for an equipment slot (weapon or armor).
    /// Click to unequip items back to backpack.
    /// </summary>
    public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("UI Elements")]
        [SerializeField] private Image iconImage;
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image rarityBorder;
        [SerializeField] private TextMeshProUGUI slotLabel;
        [SerializeField] private GameObject emptyIndicator;

        [Header("Colors")]
        [SerializeField] private Color emptyColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        [SerializeField] private Color filledColor = new Color(0.3f, 0.3f, 0.3f, 0.8f);
        [SerializeField] private Color highlightColor = new Color(0.4f, 0.4f, 0.4f, 1f);

        private EquipmentSlotType _slotType;
        private EquipmentInstance _currentItem;

        public EquipmentSlotType SlotType => _slotType;
        public EquipmentInstance CurrentItem => _currentItem;

        /// <summary>
        /// Initializes the slot with its type.
        /// </summary>
        public void Initialize(EquipmentSlotType slotType)
        {
            _slotType = slotType;

            if (slotLabel != null)
            {
                slotLabel.text = slotType.ToString();
            }

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

            // Hide empty indicator
            if (emptyIndicator != null)
            {
                emptyIndicator.SetActive(false);
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

            if (emptyIndicator != null)
            {
                emptyIndicator.SetActive(true);
            }
        }

        #region Event Handlers

        public void OnPointerClick(PointerEventData eventData)
        {
            // Click to unequip - item goes back to backpack
            if (_currentItem == null) return;

            var em = EquipmentManager.Instance;
            if (em != null)
            {
                em.Unequip(_slotType);
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
