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
            var attr = attribute as TypeMenuAttribute;
            var owner = fieldInfo.DeclaringType;
            var path = attr.path;
            if (owner.IsConstructedGenericType) {
                var arg = owner.GenericTypeArguments[0];
                path += $"/{arg.ToString()}"; 
            }
                // = attr.path != null && attr.subtype != null
                //     ? $"{attr.path}}"
                // : attr.path != null ? attr.path
                // : null;

            if (!JengaEditorGUI.ShouldShowChildren()) {
                JengaEditorGUI.PropertyReferencePlug(position, property, label);
                return;
            }

            var rect = position.RightCut(100f).LineCut();
            JengaEditorGUI.TypeMenu(rect, property, path);
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
