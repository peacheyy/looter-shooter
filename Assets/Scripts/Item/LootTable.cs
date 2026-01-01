using System;
using System.Collections.Generic;
using UnityEngine;

namespace LooterShooter.Item
{
    /// <summary>
    /// Defines what loot an enemy can drop and the chances for each item.
    /// Create via: Right-click in Project → Create → Looter Shooter → Loot Table
    /// </summary>
    [CreateAssetMenu(fileName = "LootTable", menuName = "Looter Shooter/Loot Table")]
    public class LootTable : ScriptableObject
    {
        [Serializable]
        public class LootEntry
        {
            public EquipmentData equipment;
            [Range(0f, 100f)] public float dropChance = 10f;
        }

        [Header("Equipment Drops")]
        [Tooltip("List of equipment that can drop. Each item rolls independently.")]
        public List<LootEntry> possibleDrops = new();

        [Header("Consumable Settings")]
        [Tooltip("Whether to always drop a consumable (ammo/health/armor).")]
        public bool alwaysDropConsumable = true;

        [Tooltip("Chance to drop consumable if alwaysDropConsumable is false.")]
        [Range(0f, 100f)] public float consumableDropChance = 80f;

        [Header("Guaranteed Drop (Optional)")]
        [Tooltip("If set, one item from this list will always drop (weighted by drop chance).")]
        public List<LootEntry> guaranteedDropPool = new();

        /// <summary>
        /// Spawns loot at the given position based on this loot table's configuration.
        /// </summary>
        public void DropLoot(Vector3 position)
        {
            // Handle guaranteed drop pool
            if (guaranteedDropPool.Count > 0)
            {
                EquipmentData guaranteedDrop = GetWeightedRandom(guaranteedDropPool);
                if (guaranteedDrop != null)
                {
                    EquipmentPickup.SpawnAt(position + GetRandomOffset(), guaranteedDrop);
                }
            }

            // Roll for each equipment piece independently
            foreach (var entry in possibleDrops)
            {
                if (entry.equipment != null && UnityEngine.Random.Range(0f, 100f) < entry.dropChance)
                {
                    EquipmentPickup.SpawnAt(position + GetRandomOffset(), entry.equipment);
                }
            }

            // Drop consumables
            if (alwaysDropConsumable || UnityEngine.Random.Range(0f, 100f) < consumableDropChance)
            {
                Item.SpawnAt(position + GetRandomOffset());
            }
        }

        /// <summary>
        /// Selects a random item from a list, weighted by drop chance values.
        /// </summary>
        private EquipmentData GetWeightedRandom(List<LootEntry> entries)
        {
            float totalWeight = 0f;
            foreach (var entry in entries)
            {
                if (entry.equipment != null)
                    totalWeight += entry.dropChance;
            }

            if (totalWeight <= 0f) return null;

            float roll = UnityEngine.Random.Range(0f, totalWeight);
            float cumulative = 0f;

            foreach (var entry in entries)
            {
                if (entry.equipment == null) continue;
                cumulative += entry.dropChance;
                if (roll < cumulative)
                    return entry.equipment;
            }

            return entries[entries.Count - 1].equipment;
        }

        /// <summary>
        /// Returns a small random offset to spread out multiple drops.
        /// </summary>
        private Vector3 GetRandomOffset()
        {
            return new Vector3(
                UnityEngine.Random.Range(-0.5f, 0.5f),
                0f,
                UnityEngine.Random.Range(-0.5f, 0.5f)
            );
        }
    }
}
