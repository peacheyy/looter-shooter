#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using LooterShooter.Item;
using LooterShooter.Weapons;
using LooterShooter.Player;
using System.IO;

namespace LooterShooter.Editor
{
    public static class TestEquipmentGenerator
    {
        private const string OUTPUT_PATH = "Assets/Data/TestEquipment";

        [MenuItem("Looter Shooter/Generate Test Equipment")]
        public static void GenerateTestEquipment()
        {
            // Create output directory
            if (!AssetDatabase.IsValidFolder("Assets/Data"))
            {
                AssetDatabase.CreateFolder("Assets", "Data");
            }
            if (!AssetDatabase.IsValidFolder(OUTPUT_PATH))
            {
                AssetDatabase.CreateFolder("Assets/Data", "TestEquipment");
            }

            // Generate Weapons
            CreateWeaponEquipment("Assault Rifle", EquipmentSlotType.Primary, ItemRarity.Common, Color.gray);
            CreateWeaponEquipment("Scout Rifle", EquipmentSlotType.Primary, ItemRarity.Uncommon, Color.green);
            CreateWeaponEquipment("Hand Cannon", EquipmentSlotType.Primary, ItemRarity.Rare, Color.blue);

            CreateWeaponEquipment("Shotgun", EquipmentSlotType.Special, ItemRarity.Common, Color.gray);
            CreateWeaponEquipment("Sniper Rifle", EquipmentSlotType.Special, ItemRarity.Rare, Color.blue);
            CreateWeaponEquipment("Fusion Rifle", EquipmentSlotType.Special, ItemRarity.Legendary, new Color(0.5f, 0f, 0.5f));

            CreateWeaponEquipment("Rocket Launcher", EquipmentSlotType.Heavy, ItemRarity.Rare, Color.blue);
            CreateWeaponEquipment("Machine Gun", EquipmentSlotType.Heavy, ItemRarity.Legendary, new Color(0.5f, 0f, 0.5f));
            CreateWeaponEquipment("Sword", EquipmentSlotType.Heavy, ItemRarity.Exotic, Color.yellow);

            // Generate Armor
            CreateArmor("Iron Helmet", EquipmentSlotType.Helmet, ItemRarity.Common, Color.gray);
            CreateArmor("Warrior Helm", EquipmentSlotType.Helmet, ItemRarity.Rare, Color.blue);
            CreateArmor("Exotic Crown", EquipmentSlotType.Helmet, ItemRarity.Exotic, Color.yellow);

            CreateArmor("Leather Gauntlets", EquipmentSlotType.Gauntlets, ItemRarity.Common, Color.gray);
            CreateArmor("Plated Gauntlets", EquipmentSlotType.Gauntlets, ItemRarity.Uncommon, Color.green);
            CreateArmor("Exotic Gloves", EquipmentSlotType.Gauntlets, ItemRarity.Exotic, Color.yellow);

            CreateArmor("Cloth Vest", EquipmentSlotType.Chest, ItemRarity.Common, Color.gray);
            CreateArmor("Combat Armor", EquipmentSlotType.Chest, ItemRarity.Rare, Color.blue);
            CreateArmor("Legendary Chestplate", EquipmentSlotType.Chest, ItemRarity.Legendary, new Color(0.5f, 0f, 0.5f));

            CreateArmor("Basic Boots", EquipmentSlotType.Boots, ItemRarity.Common, Color.gray);
            CreateArmor("Combat Boots", EquipmentSlotType.Boots, ItemRarity.Uncommon, Color.green);
            CreateArmor("Exotic Treads", EquipmentSlotType.Boots, ItemRarity.Exotic, Color.yellow);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Generated test equipment in {OUTPUT_PATH}");
            EditorUtility.DisplayDialog("Success", $"Generated test equipment in:\n{OUTPUT_PATH}", "OK");
        }

        private static void CreateWeaponEquipment(string name, EquipmentSlotType slot, ItemRarity rarity, Color glowColor)
        {
            string safeName = name.Replace(" ", "");
            string path = $"{OUTPUT_PATH}/Weapon_{safeName}.asset";

            // Skip if already exists
            if (AssetDatabase.LoadAssetAtPath<WeaponEquipmentData>(path) != null)
            {
                Debug.Log($"Skipping {name} - already exists");
                return;
            }

            var asset = ScriptableObject.CreateInstance<WeaponEquipmentData>();
            asset.itemName = name;
            asset.description = $"A {rarity.ToString().ToLower()} {slot.ToString().ToLower()} weapon.";
            asset.slotType = slot;
            asset.rarity = rarity;
            asset.glowColor = glowColor;
            // Note: weaponData reference will need to be set manually to link firing mechanics

            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"Created: {path}");
        }

        private static void CreateArmor(string name, EquipmentSlotType slot, ItemRarity rarity, Color glowColor)
        {
            string safeName = name.Replace(" ", "");
            string path = $"{OUTPUT_PATH}/Armor_{safeName}.asset";

            // Skip if already exists
            if (AssetDatabase.LoadAssetAtPath<ArmorData>(path) != null)
            {
                Debug.Log($"Skipping {name} - already exists");
                return;
            }

            var asset = ScriptableObject.CreateInstance<ArmorData>();
            asset.itemName = name;
            asset.description = $"A {rarity.ToString().ToLower()} {slot.ToString().ToLower()} armor piece.";
            asset.slotType = slot;
            asset.rarity = rarity;
            asset.glowColor = glowColor;

            AssetDatabase.CreateAsset(asset, path);
            Debug.Log($"Created: {path}");
        }

        [MenuItem("Looter Shooter/Link WeaponData To Equipment")]
        public static void LinkWeaponData()
        {
            // Load existing WeaponData assets
            var pistol = AssetDatabase.LoadAssetAtPath<WeaponData>("Assets/Prefabs/Pistol.asset");
            var shotgun = AssetDatabase.LoadAssetAtPath<WeaponData>("Assets/Prefabs/Shotgun.asset");
            var sniper = AssetDatabase.LoadAssetAtPath<WeaponData>("Assets/Prefabs/Sniper.asset");

            int linked = 0;

            // Link to matching WeaponEquipmentData
            // Primary weapons - use Pistol
            linked += TryLinkWeapon("Assets/Data/TestEquipment/Weapon_AssaultRifle.asset", pistol);
            linked += TryLinkWeapon("Assets/Data/TestEquipment/Weapon_ScoutRifle.asset", pistol);
            linked += TryLinkWeapon("Assets/Data/TestEquipment/Weapon_HandCannon.asset", pistol);

            // Special weapons - use Shotgun or Sniper
            linked += TryLinkWeapon("Assets/Data/TestEquipment/Weapon_Shotgun.asset", shotgun);
            linked += TryLinkWeapon("Assets/Data/TestEquipment/Weapon_SniperRifle.asset", sniper);
            linked += TryLinkWeapon("Assets/Data/TestEquipment/Weapon_FusionRifle.asset", shotgun);

            // Heavy weapons - use Sniper (placeholder)
            linked += TryLinkWeapon("Assets/Data/TestEquipment/Weapon_RocketLauncher.asset", sniper);
            linked += TryLinkWeapon("Assets/Data/TestEquipment/Weapon_MachineGun.asset", pistol);
            linked += TryLinkWeapon("Assets/Data/TestEquipment/Weapon_Sword.asset", pistol);

            AssetDatabase.SaveAssets();
            Debug.Log($"Linked {linked} weapons to WeaponData");
            EditorUtility.DisplayDialog("Success", $"Linked {linked} weapons to WeaponData assets", "OK");
        }

        private static int TryLinkWeapon(string path, WeaponData weaponData)
        {
            if (weaponData == null) return 0;

            var equipment = AssetDatabase.LoadAssetAtPath<WeaponEquipmentData>(path);
            if (equipment == null) return 0;

            equipment.weaponData = weaponData;
            EditorUtility.SetDirty(equipment);
            Debug.Log($"Linked {equipment.itemName} -> {weaponData.weaponName}");
            return 1;
        }

        [MenuItem("Looter Shooter/Add Test Items To Backpack")]
        public static void AddTestItemsToBackpack()
        {
            if (!Application.isPlaying)
            {
                EditorUtility.DisplayDialog("Error", "Enter Play Mode first!", "OK");
                return;
            }

            var em = EquipmentManager.Instance;
            if (em == null)
            {
                EditorUtility.DisplayDialog("Error", "EquipmentManager not found!", "OK");
                return;
            }

            // Load all test equipment and add to backpack
            string[] guids = AssetDatabase.FindAssets("t:EquipmentData", new[] { OUTPUT_PATH });
            int added = 0;

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                var data = AssetDatabase.LoadAssetAtPath<EquipmentData>(assetPath);

                if (data != null && !em.IsFull)
                {
                    var instance = new EquipmentInstance(data);
                    if (em.Add(instance))
                    {
                        added++;
                    }
                }
            }

            Debug.Log($"Added {added} items to backpack");
        }
    }
}
#endif
