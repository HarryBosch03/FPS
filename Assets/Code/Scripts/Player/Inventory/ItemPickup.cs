using Code.Scripts.Utility;
using UnityEngine;

namespace Code.Scripts.Player.Inventory
{
    public class ItemPickup : MonoBehaviour
    {
        private const float ItemPickupRange = 3.0f;
        private const float ItemPickupDelay = 3.0f;
    
        [SerializeField] private ItemType type;
        [SerializeField] private int count;

        private MeshFilter filter;
        private new MeshRenderer renderer;
        private new BoxCollider collider;
        private new Rigidbody rigidbody;

        private float spawnTime = 1.0f;

        private void Awake()
        {
            Rebuild();
            spawnTime = Time.time;
        }

        [ContextMenu("Rebuild")]
        private void Rebuild()
        {
            rigidbody = gameObject.GetOrAddComponent<Rigidbody>();

            var visuals = transform.childCount == 0
                ? new GameObject("Visuals")
                : transform.GetChild(0).gameObject;

            visuals.transform.SetParent(transform);
            visuals.transform.localPosition = Vector3.zero;
            visuals.transform.localRotation = Quaternion.identity;
            visuals.transform.localScale = Vector3.one;
            
            filter = visuals.GetOrAddComponent<MeshFilter>();
            renderer = visuals.GetOrAddComponent<MeshRenderer>();
            collider = visuals.GetOrAddComponent<BoxCollider>();
            
            RefreshVisuals();
        }

        private void Start()
        {
            RefreshVisuals();
        }

        private void FixedUpdate()
        {
            if (Time.time - spawnTime < ItemPickupDelay) return;
            
            var queries = Physics.OverlapSphere(transform.position, ItemPickupRange);
            foreach (var query in queries)
            {
                var inventory = query.GetComponentInParent<PlayerInventory>();
                if (!inventory) continue;
                if (!inventory.AddItemToInventory(new ItemStack(type, count))) continue;
            
                Destroy(gameObject);
                break;
            }
        }

        public void RefreshVisuals()
        {
            if (!type) return;
            
            filter.sharedMesh = type.mesh;
            renderer.sharedMaterials = type.materials;
            
            collider.center = type.mesh.bounds.center;
            collider.size = type.mesh.bounds.size;

            rigidbody.mass = collider.size.x * collider.size.y * collider.size.z * 6.0f;
        }
        
        public static ItemPickup Spawn(ItemStack stack, Vector3 position, Vector3 force = default)
        {
            var instance = new GameObject($"x{stack.Amount} {stack.Type.name} Pickup").AddComponent<ItemPickup>();
            instance.transform.position = position;
            
            instance.type = stack.Type;
            instance.count = stack.Amount;
            instance.RefreshVisuals();
            
            instance.rigidbody.AddForce(force, ForceMode.Impulse);
            
            stack.Amount = 0;

            return instance;
        }
    }
}
