using System.Collections.Generic;
using Code.Scripts.Utility;
using UnityEngine;

namespace Code.Scripts.Player.Inventory
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Inventory/Item Asset Register")]
    public class ItemAssetRegister : ScriptableObject
    {
        private const string Fallback = "fallback";

        private Sprite[] icons;
        
        private static Dictionary<string, Sprite> lookup;

        public static Sprite Lookup(string reference)
        {
            if (lookup == null)
            {
                lookup = new Dictionary<string, Sprite>();
                var registers = FindObjectsOfType<ItemAssetRegister>();
                foreach (var register in registers)
                {
                    foreach (var entry in register.icons)
                    {
                        lookup.Add(Util.SimplifyName(entry.name), entry);
                    }
                }
            }

            if (lookup.ContainsKey(reference)) return lookup[reference];
            if (lookup.ContainsKey(Fallback)) return lookup[Fallback];
            return null;
        }
    }
}