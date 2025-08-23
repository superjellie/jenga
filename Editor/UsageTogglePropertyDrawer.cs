using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    [CustomPropertyDrawer(typeof(UsageToggleAttribute))]
    public class UsageTogglePropertyDrawer : PropertyDrawer {

        public override void OnGUI(
            Rect pos, SerializedProperty property, GUIContent label
        ) {
            var attr = attribute as UsageToggleAttribute;
            var usageProperty = property.FindPropertyOnParent(attr.path);

            EditorGUI.BeginProperty(pos, label, property);

            //
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            var toggleRect = new Rect(pos.x, pos.y, 20f, 20f);
            var fieldRect = new Rect(pos.x + 20f, pos.y, pos.width - 30f, pos.height);

            usageProperty.boolValue 
                = EditorGUI.Toggle(toggleRect, usageProperty.boolValue);

            EditorGUI.BeginDisabledGroup(!usageProperty.boolValue);
            fieldRect = EditorGUI.PrefixLabel(
                fieldRect, GUIUtility.GetControlID(FocusType.Passive), label
            );
            EditorGUI.PropertyField(fieldRect, property, GUIContent.none, true);
            EditorGUI.EndDisabledGroup();

            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }
    }
}
