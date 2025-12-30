# Inventory System Reference

## Overview

The project uses two parallel inventory systems:
- **Inventory** - Consumables (ammo, health, armor) stored as type/quantity
- **EquipmentManager** - Unique equipment instances (weapons, armor pieces)

---

## Consumable Inventory

**File:** `Assets/Scripts/Player/Inventory.cs`

Singleton that manages stackable consumables.

### Storage
```csharp
private Dictionary<ItemType, int> _items;
```

### API
| Method | Description |
|--------|-------------|
| `AddItem(ItemData item)` | Add item by type/quantity |
| `RemoveItem(ItemType, int)` | Remove items, returns success |
| `GetItemCount(ItemType)` | Get count of type |
| `GetAllItems()` | Get copy of all items |
| `TotalItemCount()` | Total across all types |

### Events
```csharp
public event Action OnInventoryChanged;
```

### Item Types
```csharp
public enum ItemType { Generic, Ammo, Health, Armor, Weapon }
```

---

## Equipment System

### EquipmentManager

**File:** `Assets/Scripts/Player/EquipmentManager.cs`

Singleton implementing `IItemStorage<EquipmentInstance>`.

### Storage
```csharp
private Dictionary<EquipmentSlotType, EquipmentInstance> _equippedItems;
private List<EquipmentInstance> _backpack;  // Capacity: 20
```

### Slot Types
**Weapons:** Primary, Special, Heavy
**Armor:** Helmet, Gauntlets, Chest, Boots

### API
| Method | Description |
|--------|-------------|
| `Add(EquipmentInstance)` | Add to backpack |
| `Remove(EquipmentInstance)` | Remove from backpack |
| `Equip(EquipmentInstance)` | Move from backpack to slot |
| `Unequip(EquipmentSlotType)` | Move from slot to backpack |
| `GetEquipped(slot)` | Get equipped item |
| `GetActiveWeapon()` | Get active weapon instance |
| `SwitchActiveWeaponSlot(slot)` | Change active weapon |
| `CycleWeaponSlot(direction)` | Cycle through weapons |
| `GetBackpackItems()` | Get all backpack items |

### Properties
```csharp
public bool IsFull { get; }
public int Count { get; }
public int Capacity { get; }
public EquipmentSlotType ActiveWeaponSlot { get; }
```

### Events
```csharp
public event Action<EquipmentSlotType, EquipmentInstance> OnSlotChanged;
public event Action OnBackpackChanged;
public event Action<EquipmentSlotType> OnActiveWeaponSlotChanged;
public event Action OnChanged;
```

---

## Data Structures

### EquipmentInstance

**File:** `Assets/Scripts/Item/EquipmentInstance.cs`

Runtime wrapper with unique ID for tracking individual items.

```csharp
public class EquipmentInstance
{
    public string InstanceId { get; }     // GUID
    public EquipmentData Data { get; }    // Base ScriptableObject
}
```

### EquipmentData (Abstract)

**File:** `Assets/Scripts/Item/EquipmentData.cs`

Base ScriptableObject for all equipment.

```csharp
public abstract class EquipmentData : ScriptableObject
{
    public string itemName;
    public string description;
    public Sprite icon;
    public EquipmentSlotType slotType;
    public ItemRarity rarity;
    public Color glowColor;
}
```

### Derived Types
- **ArmorData** - `Assets/Scripts/Item/ArmorData.cs`
- **WeaponEquipmentData** - `Assets/Scripts/Weapons/WeaponEquipmentData.cs` (references `WeaponData`)

### Rarity
```csharp
public enum ItemRarity { Common, Uncommon, Rare, Legendary, Exotic }
```

---

## World Pickups

### Consumable Pickup

**File:** `Assets/Scripts/Item/Item.cs`

```csharp
public class Item : MonoBehaviour, IInteractable
{
    public void Interact() {
        Inventory.Instance.AddItem(_itemData);
        Destroy(gameObject);
    }

    public static GameObject SpawnAt(Vector3 position, ItemData itemData);
}
```

### Equipment Pickup

**File:** `Assets/Scripts/Item/EquipmentPickup.cs`

```csharp
public class EquipmentPickup : MonoBehaviour, IInteractable
{
    public void Interact() {
        EquipmentManager.Instance.Add(_instance);
        Destroy(gameObject);
    }

    public static GameObject SpawnAt(Vector3 position, EquipmentData data);
    public static GameObject SpawnAt(Vector3 position, EquipmentInstance instance);
}
```

---

## UI Components

### EquipmentUI

**File:** `Assets/Scripts/UI/EquipmentUI.cs`

Main inventory panel. Toggle with Tab key.

```csharp
public bool IsOpen { get; }
public void ToggleInventory();
public void OpenInventory();
public void CloseInventory();
```

### EquipmentSlotUI

**File:** `Assets/Scripts/UI/EquipmentSlotUI.cs`

Individual equipment slot. Click to unequip.

### BackpackUI

**File:** `Assets/Scripts/UI/BackpackUI.cs`

Grid container (5x4 = 20 slots).

### BackpackSlotUI

**File:** `Assets/Scripts/UI/BackpackSlotUI.cs`

Individual backpack slot. Click to equip.

---

## Weapon Integration

**WeaponHandler** (`Assets/Scripts/Player/WeaponHandler.cs`) bridges equipment to actual weapons:
- Subscribes to `OnSlotChanged` and `OnActiveWeaponSlotChanged`
- Spawns/destroys weapon GameObjects based on equipment
- Manages which weapon is currently active

---

## Event Flow

```
Pickup Equipment
    └→ EquipmentManager.Add()
        └→ OnBackpackChanged
            └→ BackpackUI.Refresh()

Click Backpack Slot
    └→ EquipmentManager.Equip()
        └→ OnSlotChanged
        └→ OnBackpackChanged
            └→ EquipmentSlotUI updates
            └→ WeaponHandler spawns weapon

Click Equipment Slot
    └→ EquipmentManager.Unequip()
        └→ OnSlotChanged
        └→ OnBackpackChanged
            └→ Item returns to backpack
```

---

## File Summary

| Category | Files |
|----------|-------|
| Core | `Player/Inventory.cs`, `Player/EquipmentManager.cs` |
| Data | `Item/ItemData.cs`, `Item/EquipmentData.cs`, `Item/ArmorData.cs`, `Item/EquipmentInstance.cs`, `Item/EquipmentEnums.cs` |
| Pickups | `Item/Item.cs`, `Item/EquipmentPickup.cs` |
| UI | `UI/EquipmentUI.cs`, `UI/EquipmentSlotUI.cs`, `UI/BackpackUI.cs`, `UI/BackpackSlotUI.cs` |
| Weapons | `Weapons/WeaponEquipmentData.cs`, `Player/WeaponHandler.cs` |
| Interfaces | `Common/IItemStorage.cs`, `Common/IInteractable.cs` |
