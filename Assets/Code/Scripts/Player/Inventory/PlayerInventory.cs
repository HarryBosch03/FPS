using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private int inventorySize;

    public List<ItemStack> items = new();

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
}
