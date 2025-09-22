using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    [CustomPropertyDrawer(typeof(HideLabelAttribute))]
    public class HideLabelPropertyDrawer : PropertyDrawer {

        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label
        ) {
            EditorGUI.PropertyField(
                position, property, GUIContent.none, 
                JengaEditorGUI.ShouldShowChildren()
            );
        }

        public override float GetPropertyHeight(
            SerializedProperty property, GUIContent label
        ) => EditorGUI.GetPropertyHeight(
            property, label, JengaEditorGUI.ShouldShowChildren()
        );

    }

}
