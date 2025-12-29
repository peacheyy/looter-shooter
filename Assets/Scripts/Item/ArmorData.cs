using UnityEngine;

namespace LooterShooter.Item
{
    [CreateAssetMenu(fileName = "New Armor", menuName = "Looter Shooter/Armor Data")]
    public class ArmorData : ScriptableObject
    {
        public string armorName;
        public ArmorSlot armorSlot;
        public Sprite icon;
    }
}
