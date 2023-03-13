using Code.Scripts.Editor_Tools;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Code.Scripts.Editor
{
    [CustomPropertyDrawer(typeof(CannotBeNullAttribute))]
    public class CannotBeNullDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var color = new Color(1.0f, 0.0f, 0.0f, 0.3f);

            var root = new VisualElement();

            root.Add(new PropertyField(property));
            
            return root;
        }
    }
}
