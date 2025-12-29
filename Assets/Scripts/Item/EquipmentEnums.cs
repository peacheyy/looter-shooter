namespace LooterShooter.Item
{
    /// <summary>
    /// Defines the equipment slot types for both weapons and armor.
    /// </summary>
    public enum EquipmentSlotType
    {
        None,
        // Weapon slots
        Primary,
        Special,
        Heavy,
        // Armor slots
        Helmet,
        Gauntlets,
        Chest,
        Boots
    }

    /// <summary>
    /// Item rarity tiers for visual distinction and future loot tables.
    /// </summary>
    public enum ItemRarity
    {
        Common,
        Uncommon,
        Rare,
        Legendary,
        Exotic
    }
}
