using System.Collections.Generic;
using Code.Scripts.Utility;
using UnityEngine;

[CreateAssetMenu(menuName = "Scriptable Objects/Inventory/Item Asset Register")]
public class ItemAssetRegister : ScriptableObject
{
    public Sprite[] icons;

    private static Dictionary<string, Sprite> lookup;

    public static Dictionary<string, Sprite> Lookup
    {
        get
        {
            if (lookup != null) return lookup;

            lookup = new Dictionary<string, Sprite>();
            var registers = FindObjectsOfType<ItemAssetRegister>();
            foreach (var register in registers)
            {
                foreach (var entry in register.icons)
                {
                    lookup.Add(Util.SimplifyName(entry.name), entry);
                }
            }
            
            return lookup;
        }
    }
}
