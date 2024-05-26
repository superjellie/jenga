using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    [CustomPropertyDrawer(typeof(EnumStyleAttribute))]
    public class EnumStyleDrawer : PropertyDrawer {
         // Draw the property inside the given rect
        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label
        ) {
            property.serializedObject.Update();
            // First get the attribute since it contains the range for the slider
            EnumStyleAttribute self = attribute as EnumStyleAttribute;
            var propType = property.propertyType;
            var type = fieldInfo.FieldType;
            if (propType != SerializedPropertyType.ObjectReference) {
                EditorGUILayout.PropertyField(property);
            } else {
                var value = property.objectReferenceValue;

                var objs = Object.FindObjectsByType(
                    type, FindObjectsSortMode.None
                ).ToList();

                bool missing = value == null || !objs.Contains(value);
                if (missing) objs.Add(value);
                int index = objs.FindIndex(x => x == value);
                var names = objs.Select(x => x != null ? x.name : "None").
                    ToArray();
                if (missing && value != null) 
                    names[index] = $"{names[index]} (Missing)";

                int newIndex = EditorGUILayout.Popup(
                    property.displayName, index, names
                );
                if (newIndex != index)
                    property.objectReferenceValue = objs[newIndex];

            }

            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
