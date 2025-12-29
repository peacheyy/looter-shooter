using System;

namespace LooterShooter
{
    /// <summary>
    /// Unified storage interface for both consumables and equipment.
    /// Enables consistent Add/Remove/IsFull logic across backpack, consumable inventory,
    /// and future systems (Vault, Postmaster).
    /// </summary>
    public interface IItemStorage<T>
    {
        bool Add(T item);
        bool Remove(T item);
        bool IsFull { get; }
        int Count { get; }
        int Capacity { get; }
        event Action OnChanged;
    }
}
