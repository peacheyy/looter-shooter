using UnityEngine;

namespace LooterShooter.Item
{
    public enum ItemType
    {
        Generic,
        Ammo,
        Health,
        Armor,
        Weapon
    }

    [System.Serializable]
    public class ItemData
    {
        public string itemName;
        public ItemType itemType;
        public int quantity;
        public Color glowColor;

        public ItemData(string name, ItemType type, int qty = 1)
        {
            itemName = name;
            itemType = type;
            quantity = qty;
            glowColor = GetDefaultColor(type);
        }

        public static Color GetDefaultColor(ItemType type)
        {
            return type switch
            {
                ItemType.Ammo => Color.yellow,
                ItemType.Health => Color.green,
                ItemType.Armor => Color.blue,
                ItemType.Weapon => new Color(1f, 0.5f, 0f),
                _ => Color.cyan
            };
        }
    }
}
