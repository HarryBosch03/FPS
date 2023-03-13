using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

namespace Code.Scripts.Player.Inventory
{
    public class PlayerInventory : MonoBehaviour, IEnumerable<PlayerInventory.ItemStackBorrow>
    {
        [SerializeField] private int inventorySize;

        private readonly List<ItemStack> items = new();
        public int SlotsLeft => inventorySize - items.Count;
        public int Size => Mathf.Max(items.Count, inventorySize);
        
        public bool AddItemToInventory(ItemStack item)
        {
            if (!item) return true;
        
            foreach (var other in items)
            {
                if (other.TryCombine(item)) return true;
            }

            if (items.Count >= inventorySize) return false;
        
            items.Add(item);
            return true;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<ItemStackBorrow> GetEnumerator()
        {
            var list = new List<ItemStackBorrow>();
            foreach (var item in items)
            {
                list.Add(new ItemStackBorrow(item, this));
            }

            return list.GetEnumerator();
        }

        public class ItemStackBorrow : IDisposable
        {
            public readonly ItemStack stack;
            public readonly PlayerInventory inventory;

            public ItemType Type => stack.Type;
            public int Amount { get => stack.Amount; set => stack.Amount = value; }

            public ItemStackBorrow(ItemStack stack, PlayerInventory inventory)
            {
                this.stack = stack;
                this.inventory = inventory;
            }

            public static implicit operator ItemStack(ItemStackBorrow borrow) => borrow.stack;
            
            public void Dispose()
            {
                if (!stack) inventory.items.Remove(stack);
            }
        }

        public int GetItemCount(ItemType type)
        {
            var c = 0;
            foreach (var item in items)
            {
                if (item.Type != type) continue;
                c += item.Amount;
            }

            return c;
        }

        public int TryTakeItemCount(ItemType type, int amount)
        {
            var left = amount;
            foreach (var item in items)
            {
                if (item.Type != type) continue;
                var fromThis = Mathf.Min(left, item.Amount);
                item.Amount -= fromThis;
                left -= fromThis;

                if (left == 0) break;
            }
            
            return amount - left;
        }
    }
}
