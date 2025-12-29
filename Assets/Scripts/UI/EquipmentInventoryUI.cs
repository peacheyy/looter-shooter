using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using LooterShooter.Player;
using LooterShooter.Item;
using LooterShooter.Weapons;

namespace LooterShooter.UI
{
    /// <summary>
    /// Destiny 1-style equipment UI showing 3 weapon slots and 4 armor slots.
    /// Toggle with 'I' key.
    /// </summary>
    public class EquipmentInventoryUI : MonoBehaviour
    {
        [Header("Panel")]
        [SerializeField] private GameObject equipmentPanel;

        [Header("Weapon Slot Text")]
        [SerializeField] private TextMeshProUGUI primaryWeaponText;
        [SerializeField] private TextMeshProUGUI specialWeaponText;
        [SerializeField] private TextMeshProUGUI heavyWeaponText;

        [Header("Armor Slot Text")]
        [SerializeField] private TextMeshProUGUI helmetText;
        [SerializeField] private TextMeshProUGUI gauntletsText;
        [SerializeField] private TextMeshProUGUI chestText;
        [SerializeField] private TextMeshProUGUI bootsText;

        [Header("Colors")]
        [SerializeField] private Color emptySlotColor = new Color(0.5f, 0.5f, 0.5f);
        [SerializeField] private Color primaryColor = new Color(1f, 1f, 1f);
        [SerializeField] private Color specialColor = new Color(0.2f, 1f, 0.2f);
        [SerializeField] private Color heavyColor = new Color(0.6f, 0.2f, 1f);
        [SerializeField] private Color armorColor = new Color(0.3f, 0.7f, 1f);

        private EquipmentInventory _equipment;
        private bool _isVisible = false;

        private void Start()
        {
            _equipment = EquipmentInventory.Instance;

            if (_equipment != null)
            {
                _equipment.OnEquipmentChanged += UpdateUI;
            }

            UpdateUI();
            UpdateVisibility();
        }

        private void OnDestroy()
        {
            if (_equipment != null)
            {
                _equipment.OnEquipmentChanged -= UpdateUI;
            }
        }

        private void Update()
        {
            if (Keyboard.current != null && Keyboard.current.tabKey.wasPressedThisFrame)
            {
                _isVisible = !_isVisible;
                UpdateVisibility();
            }
        }

        private void UpdateVisibility()
        {
            if (equipmentPanel != null)
            {
                equipmentPanel.SetActive(_isVisible);
            }
        }

        private void UpdateUI()
        {
            UpdateWeaponSlots();
            UpdateArmorSlots();
        }

        private void UpdateWeaponSlots()
        {
            if (_equipment == null) return;

            UpdateWeaponSlotText(primaryWeaponText, _equipment.GetWeapon(WeaponSlot.Primary), "PRIMARY", primaryColor);
            UpdateWeaponSlotText(specialWeaponText, _equipment.GetWeapon(WeaponSlot.Special), "SPECIAL", specialColor);
            UpdateWeaponSlotText(heavyWeaponText, _equipment.GetWeapon(WeaponSlot.Heavy), "HEAVY", heavyColor);
        }

        private void UpdateWeaponSlotText(TextMeshProUGUI text, WeaponData weapon, string slotName, Color slotColor)
        {
            if (text == null) return;

            if (weapon != null)
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(slotColor);
                text.text = $"<color=#{hexColor}>[{slotName}]</color>\n{weapon.weaponName}";
            }
            else
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(emptySlotColor);
                text.text = $"<color=#{hexColor}>[{slotName}]\n- Empty -</color>";
            }
        }

        private void UpdateArmorSlots()
        {
            if (_equipment == null) return;

            UpdateArmorSlotText(helmetText, _equipment.GetArmor(ArmorSlot.Helmet), "HELMET");
            UpdateArmorSlotText(gauntletsText, _equipment.GetArmor(ArmorSlot.Gauntlets), "GAUNTLETS");
            UpdateArmorSlotText(chestText, _equipment.GetArmor(ArmorSlot.Chest), "CHEST");
            UpdateArmorSlotText(bootsText, _equipment.GetArmor(ArmorSlot.Boots), "BOOTS");
        }

        private void UpdateArmorSlotText(TextMeshProUGUI text, ArmorData armor, string slotName)
        {
            if (text == null) return;

            if (armor != null)
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(armorColor);
                text.text = $"<color=#{hexColor}>[{slotName}]</color>\n{armor.armorName}";
            }
            else
            {
                string hexColor = ColorUtility.ToHtmlStringRGB(emptySlotColor);
                text.text = $"<color=#{hexColor}>[{slotName}]\n- Empty -</color>";
            }
        }
    }
}
