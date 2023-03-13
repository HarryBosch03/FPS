using UnityEditor;
using UnityEngine;

namespace Code.Scripts.Player.Inventory.Editor
{
    [CustomEditor(typeof(PlayerInventory))]
    public class PlayerInventoryEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (!Application.isPlaying) return;

            GUILayout.Space(30);

            using var layout = new EditorGUILayout.VerticalScope(EditorStyles.helpBox);

            GUILayout.Label("Inventory Contents");
            var indent = EditorGUI.indentLevel++;

            if (target is not PlayerInventory inventory) return;

            foreach (var item in inventory)
            {
                using (item)
                {
                    using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
                    {
                        GUILayout.Label($"{item.Type.name}: {item.Amount}/{item.Type.maxStackSize}");

                        if (GUILayout.Button("Drop"))
                        {
                            ItemPickup.Spawn(item, inventory.transform.position);
                        }
                    }
                }
            }

            EditorGUI.indentLevel = indent;
            GUILayout.Label(inventory.SlotsLeft == 0 ? "Inventory Full" : $"{inventory.SlotsLeft} Slots Left");
        }
    }
}