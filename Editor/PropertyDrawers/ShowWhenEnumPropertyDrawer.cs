using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    [CustomPropertyDrawer(typeof(ShowWhenEnumAttribute))]
    public class ShowWhenEnumPropertyDrawer : PropertyDrawer {

        public override void OnGUI(
            Rect pos, SerializedProperty property, GUIContent label
        ) {
            var attr = attribute as ShowWhenEnumAttribute;
            var enumProperty = property.FindPropertyOnParent(attr.path);
            var value = enumProperty?.enumValueIndex ?? 0;

            if (attr.values.Contains(value))  
                EditorGUI.PropertyField(pos, property, label, true);
        }

        public override float GetPropertyHeight(
            SerializedProperty property, GUIContent label
        ) {
            var attr = attribute as ShowWhenEnumAttribute;
            var enumProperty = property.FindPropertyOnParent(attr.path);
            var value = enumProperty?.enumValueIndex ?? 0;

            if (attr.values.Contains(value))  
                return EditorGUI.GetPropertyHeight(property, label, true);

            return 0f;
        }
    }
}
