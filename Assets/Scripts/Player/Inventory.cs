using System.Collections.Generic;
using UnityEngine;
using LooterShooter.Item;

namespace LooterShooter.Player
{
    public class Inventory : MonoBehaviour
    {
        public static Inventory Instance { get; private set; }

        private Dictionary<ItemType, int> _items = new Dictionary<ItemType, int>();

        public event System.Action OnInventoryChanged;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void AddItem(ItemData item)
        {
            if (_items.ContainsKey(item.itemType))
            {
                _items[item.itemType] += item.quantity;
            }
            else
            {
                _items[item.itemType] = item.quantity;
            }

            Debug.Log($"Added {item.quantity}x {item.itemName} to inventory. Total {item.itemType}: {_items[item.itemType]}");
            OnInventoryChanged?.Invoke();
        }

        public int GetItemCount(ItemType type)
        {
            return _items.TryGetValue(type, out int count) ? count : 0;
        }

        public bool RemoveItem(ItemType type, int amount = 1)
        {
            if (!_items.ContainsKey(type) || _items[type] < amount)
            {
                return false;
            }

            _items[type] -= amount;
            if (_items[type] <= 0)
            {
                _items.Remove(type);
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        public Dictionary<ItemType, int> GetAllItems()
        {
            return new Dictionary<ItemType, int>(_items);
        }

        public int TotalItemCount()
        {
            int total = 0;
            foreach (var count in _items.Values)
            {
                total += count;
            }
            return total;
        }
    }
}
