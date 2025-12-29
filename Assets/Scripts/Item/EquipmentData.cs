using UnityEngine;

namespace LooterShooter.Item
{
    /// <summary>
    /// Base ScriptableObject for all equipment (weapons and armor).
    /// Defines common properties shared across all equipment types.
    /// </summary>
    public abstract class EquipmentData : ScriptableObject
    {
        [Header("Basic Info")]
        public string itemName;
        [TextArea(2, 4)]
        public string description;
        public Sprite icon;

        [Header("Equipment")]
        public EquipmentSlotType slotType;
        public ItemRarity rarity = ItemRarity.Common;

        [Header("Visuals")]
        public Color glowColor = Color.white;

        /// <summary>
        /// Returns the rarity color for UI display.
        /// </summary>
        public Color GetRarityColor()
        {
            return rarity switch
            {
                ItemRarity.Common => Color.white,
                ItemRarity.Uncommon => Color.green,
                ItemRarity.Rare => Color.blue,
                ItemRarity.Legendary => new Color(0.5f, 0f, 0.5f), // Purple
                ItemRarity.Exotic => Color.yellow,
                _ => Color.white
            };
        }
    }
}
