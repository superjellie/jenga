using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyAttributePropertyDrawer : PropertyDrawer {
        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label
        ) {
            GUI.enabled = false; 
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true; 
        }

        public override float 
        GetPropertyHeight(SerializedProperty property, GUIContent label)
            => EditorGUI.GetPropertyHeight(property, label, true);
            
    }
}
