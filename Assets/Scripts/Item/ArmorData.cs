using UnityEngine;

namespace LooterShooter.Item
{
    /// <summary>
    /// ScriptableObject for armor pieces (Helmet, Gauntlets, Chest, Boots).
    /// MVP: No stats, just visual representation in UI.
    /// </summary>
    [CreateAssetMenu(fileName = "New Armor", menuName = "Looter Shooter/Armor Data")]
    public class ArmorData : EquipmentData
    {
        // MVP: No stats - armor is UI-only for now
        // Future: defense value, stat modifiers, perks, etc.
    }
}
