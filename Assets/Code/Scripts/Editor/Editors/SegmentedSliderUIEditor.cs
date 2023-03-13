using Code.Scripts.UI;
using UnityEditor;

namespace Code.Scripts.Editor.Editors
{
    [CustomEditor(typeof(SegmentedSliderUI))]
    public class SegmentedSliderUIEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }
    }
}