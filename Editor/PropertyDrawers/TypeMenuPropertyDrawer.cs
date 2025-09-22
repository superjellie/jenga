using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    [CustomPropertyDrawer(typeof(TypeMenuAttribute))]
    public class TypeMenuPropertyDrawer : PropertyDrawer {

        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label
        ) {
            // Debug.Log("Het");

            if (!JengaEditorGUI.ShouldShowChildren()) {
                JengaEditorGUI.PropertyReferencePlug(position, property, label);
                return;
            }

            var rect = position.RightCut(100f).LineCut();
            JengaEditorGUI.TypeMenu(rect, property);
            EditorGUI.PropertyField(
                position, property, label, 
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
