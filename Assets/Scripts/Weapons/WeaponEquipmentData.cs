using UnityEngine;
using LooterShooter.Item;

namespace LooterShooter.Weapons
{
    /// <summary>
    /// Equipment wrapper for WeaponData.
    /// Links weapon configuration to the equipment/inventory system.
    /// </summary>
    [CreateAssetMenu(fileName = "New Weapon Equipment", menuName = "Looter Shooter/Weapon Equipment")]
    public class WeaponEquipmentData : EquipmentData
    {
        [Header("Weapon Reference")]
        [Tooltip("The WeaponData containing firing mechanics, damage, etc.")]
        public WeaponData weaponData;

        private void OnValidate()
        {
            // Auto-sync name and icon from WeaponData if not set
            if (weaponData != null)
            {
                if (string.IsNullOrEmpty(itemName))
                {
                    itemName = weaponData.weaponName;
                }
                if (icon == null && weaponData.icon != null)
                {
                    icon = weaponData.icon;
                }
            }

            // Ensure slot type is a weapon slot
            if (slotType != EquipmentSlotType.Primary &&
                slotType != EquipmentSlotType.Special &&
                slotType != EquipmentSlotType.Heavy)
            {
                slotType = EquipmentSlotType.Primary;
            }
        }
    }
}
