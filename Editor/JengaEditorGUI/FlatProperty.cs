using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {
        public static void 
        FlatProperty(Rect rect, SerializedProperty property) {
            if (property == null) return;

            var labelRect = rect.LineCut(out rect);
            var typeMenuRect = labelRect.RightCut(150f, out labelRect);
            var linkRect = labelRect.LeftCut(20f, out labelRect);
            LinkCablePlug(linkRect, property);
            EditorGUI.LabelField(labelRect, property.GetDisplayName());
            JengaEditorGUI.TypeMenu(typeMenuRect, property);


            EditorGUI.indentLevel++;
            foreach (var child in property.Children()) {

                if (child.isArray) {
                    JengaEditorGUI.BeginForceCollapsedGroup();
                    rect = JengaEditorGUI.ListField(rect, child, 250f);
                    JengaEditorGUI.EndForceCollapsedGroup();
                } else if (child.propertyType
                        == SerializedPropertyType.ManagedReference) {
                    var childRect = rect.LineCut(out rect);
                    var name = child.GetDisplayName();
                    var type = child.GetCurrentType()?.Name;
                    EditorGUI.LabelField(childRect, name, type);
                } else {
                    var height = EditorGUI.GetPropertyHeight(child, true);
                    var childRect = rect.TopCut(height, out rect);
                    EditorGUI.PropertyField(childRect, child, true);
                }
            } 
            EditorGUI.indentLevel--;

        }
    }
}
