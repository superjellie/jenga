using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    [CustomPropertyDrawer(typeof(IInlineMe), true)]
    public class IInlineMePropertyDrawer : PropertyDrawer {

        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label
        ) {
            label = EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(position, label);
            
            foreach (var child in property.DirectChildren()) {
                var propChild = child as SerializedProperty;
                var childField = fieldInfo.FieldType.GetField(propChild.name); 

                var rect = position;

                if (childField.TryGetAttribute<StyleAttribute>(out var style)) {

                    if (!float.IsNaN(style.width))
                        rect = position.LeftCut(style.width, out position);
                    if (!float.IsNaN(style.marginLeft))
                        rect = rect.LeftCut(style.marginLeft);
                    if (!float.IsNaN(style.marginRight))
                        rect = rect.RightCut(style.marginRight);
                }

                rect = rect.RightCut(5f);
                EditorGUI.PropertyField(rect, propChild, GUIContent.none, true);
            } 

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(
            SerializedProperty property, GUIContent label
        ) {
            var height = 0f;
            foreach (var propChild in property) 
                height = Mathf.Max(
                    height, EditorGUI.GetPropertyHeight(property, label, true)
                );
            

            return height;
        } 
    }

}
