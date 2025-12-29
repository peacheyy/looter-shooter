using System;

namespace LooterShooter.Item
{
    /// <summary>
    /// Runtime instance of equipment. Each piece of gear in the game is represented
    /// by an EquipmentInstance, allowing for unique identification even with the same base data.
    /// Future-proofs for durability, perks, and trading.
    /// </summary>
    [Serializable]
    public class EquipmentInstance
    {
        /// <summary>
        /// Unique identifier for this specific item instance.
        /// </summary>
        public string InstanceId { get; private set; }

        /// <summary>
        /// The base equipment data (ScriptableObject) defining this item's properties.
        /// </summary>
        public EquipmentData Data { get; private set; }

        public EquipmentInstance(EquipmentData data)
        {
            InstanceId = Guid.NewGuid().ToString();
            Data = data;
        }

        /// <summary>
        /// Creates an instance with a specific ID (for save/load).
        /// </summary>
        public EquipmentInstance(EquipmentData data, string instanceId)
        {
            InstanceId = instanceId;
            Data = data;
        }

        public override bool Equals(object obj)
        {
            if (obj is EquipmentInstance other)
            {
                return InstanceId == other.InstanceId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return InstanceId.GetHashCode();
        }
    }
}
