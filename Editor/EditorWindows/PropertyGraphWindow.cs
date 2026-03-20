using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public class PropertyGraphWindow : EditorWindow {
        public PersistentProperty property;

        // [MenuItem("Window/Jenga/Property Graph")]
        public static void ShowGraph() {
            var wnd = GetWindow<PropertyGraphWindow>();
            wnd.titleContent = new GUIContent("Property Graph");
        }

        public static void Edit(SerializedProperty property) {
            var wnd = GetWindow<PropertyGraphWindow>();
            wnd.titleContent = new GUIContent("Property Graph");
            wnd.property = new(property);
        }

        void OnGUI() {
            // var sp = property.GetProperty();
            // if (sp == null) return;

            // var so = sp.serializedObject;
            // if (so == null) return;
            
            // so.Update();
            // SerializedReferenceUtility.UpdateCachedLinks(so);

            var rect = new Rect(0f, 0f, position.width, position.height);

            JengaEditorGUI.ResetDataGroup();
            JengaEditorGUI.BeginDataGroup($"Jenga.PropertyGraph+{property}");
            JengaEditorGUI.SplitView(rect, "split", out var r1, out var r2);
            
            JengaEditorGUI.BeginDraggableViewGroup(
                r2, JengaAssets.texBckgGrid, "view"
            )
            ;
            JengaEditorGUI.EndDraggableViewGroup();

            JengaEditorGUI.EndDataGroup();

            // so.ApplyModifiedProperties();
        }

    }
}
