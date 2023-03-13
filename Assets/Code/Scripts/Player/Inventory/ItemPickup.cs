using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private const float itemPickupRange = 3.0f;
    
    [SerializeField] private ItemType type;
    [SerializeField] private int count;

    private void FixedUpdate()
    {
        var queries = Physics.OverlapSphere(transform.position, itemPickupRange);
        foreach (var query in queries)
        {
            var inventory = query.GetComponentInParent<PlayerInventory>();
            if (!inventory) continue;
            if (!inventory.AddItemToInventory(new ItemStack(type, count))) continue;
            
            Destroy(gameObject);
            break;
        }
    }
}
