using Code.Scripts.Utility;
using UnityEngine;

namespace Code.Scripts.Player.Inventory
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Inventory/Item Type")]
    public class ItemType : ScriptableObject
    {
        public int maxStackSize = 100;
        public Mesh mesh;
        public Material[] materials;
        
        public string ReferenceName { get; private set; }
        public Sprite Icon { get; private set; }
        
        public void OnEnable()
        {
            ReferenceName = Util.SimplifyName(name);
            Icon = ItemAssetRegister.Lookup(ReferenceName);
        }
    }
}
