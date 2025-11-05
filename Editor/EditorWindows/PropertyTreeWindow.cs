using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public class PropertyTreeWindow : EditorWindow {

        public Object[] objectReferences;
        public string propertyPath;
        public SerializedObject serializedObject;
        public SerializedProperty serializedProperty;

        [MenuItem("Window/Jenga/Property Tree")]
        public static void ShowExample() {
            var wnd = GetWindow<PropertyTreeWindow>();
            wnd.titleContent = new GUIContent("Property Tree");
        }

        public static void Edit(SerializedProperty property) {
            var wnd = GetWindow<PropertyTreeWindow>();
            wnd.titleContent = new GUIContent("Property Tree");
            wnd.objectReferences = property.serializedObject.targetObjects;
            wnd.propertyPath = property.propertyPath;
            wnd.UpdateProperty();
        }

        void OnEnable() => UpdateProperty();

        void UpdateProperty() {
            try {
                serializedObject = new(objectReferences);
                serializedProperty = serializedObject.FindProperty(propertyPath);
            } finally { }
        }

        void OnGUI() {
            if (serializedObject != null) {
                serializedObject.Update();
                SerializedReferenceUtility.UpdateCachedLinks(serializedObject);
            }

            var rect = new Rect(0f, 0f, position.width, position.height);

            JengaEditorGUI.ResetDataGroup();
            JengaEditorGUI.BeginCablePlugsGroup();
            JengaEditorGUI.BeginDataGroup("Jenga.PropertyTree");
            JengaEditorGUI.SplitView(rect, "split", out var r1, out var r2);
            var selected = JengaEditorGUI
                .PropertyTree(r1.MoveDown(-5f), serializedProperty, "tree");

            JengaEditorGUI.layoutLeftPlugs = true;
            JengaEditorGUI.FlatProperty(r2.Shrink(5f, 5f), selected);
            JengaEditorGUI.layoutLeftPlugs = false;

            JengaEditorGUI.EndCablePlugsGroup();
            JengaEditorGUI.EndDataGroup();

            if (serializedObject != null)
                serializedObject.ApplyModifiedProperties();
        }
    }
}
