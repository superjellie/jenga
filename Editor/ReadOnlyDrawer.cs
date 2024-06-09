using UnityEngine;
using UnityEditor;

namespace Jenga {

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer {
        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label
        ) {
            var wasEnabled = GUI.enabled;
            GUI.enabled = false;
            EditorGUILayout.PropertyField(property, label);
            GUI.enabled = wasEnabled;
        }

        public override float GetPropertyHeight(
            SerializedProperty property, GUIContent label
        ) => 0f;
    }

}
