#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace Jenga {
    [CustomPropertyDrawer(typeof(TransformWorldSnapshot))]
    public class TransformWorldSnapshotPropertyDrawer 
    : PropertyDrawer {

        public static SerializedReferenceLink selected 
            = SerializedReferenceLink.Null;
        public static bool isInited;

        void Init() {
            isInited = true;
            SceneView.duringSceneGui += OnSceneGUI;
        }

        public override void 
        OnGUI(Rect position, SerializedProperty property, GUIContent label) {
            
            if (!isInited) Init();

            EditorGUI.BeginProperty(position, label, property);
            var link = property.GetLink();
            var isSelected = selected == link;

            var propPosition = property.FindPropertyRelative("position");
            var propAngles = property.FindPropertyRelative("eulerAngles");

            // Draw label

            var firstLine = position.LineCut(out position);

            firstLine = EditorGUI.PrefixLabel(
                firstLine, GUIUtility.GetControlID(FocusType.Passive), label
            );

            var rectButton = firstLine.RightCut(100f, out firstLine);
            var rectPosition = position.LineCut(out position);
            var rectAngles = position.LineCut(out position);

            var oldColor = GUI.color;
            EditorGUI.indentLevel++;
            if (isSelected)
                GUI.color = Color.green;

            if (GUI.Button(rectButton, "Edit")) {
                if (isSelected) selected = SerializedReferenceLink.Null;
                else selected = link;
            }

            GUI.color = oldColor;
                
            if (isSelected) {
                EditorGUI.PropertyField(rectPosition, propPosition);
                EditorGUI.PropertyField(rectAngles, propAngles);
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        public override float
        GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var link = property.GetLink();

            if (link == selected) 
                return EditorGUIUtility.singleLineHeight * 3f; 

            return EditorGUIUtility.singleLineHeight;
        }

        static void OnSceneGUI(SceneView sv) {
            // if (selectedKey != null)
            if (selected == null) return;
            var prop = selected.GetProperty();
            if (prop == null) return;

            var propPosition = prop.FindPropertyRelative("position");
            var propEulerAngles = prop.FindPropertyRelative("eulerAngles");

            var pos = propPosition.vector3Value;
            var rot = Quaternion.Euler(propEulerAngles.vector3Value);

            EditorGUI.BeginChangeCheck();
            Handles.TransformHandle(ref pos, ref rot);
        
            if (EditorGUI.EndChangeCheck()) {
                propPosition.vector3Value = pos;
                propEulerAngles.vector3Value = rot.eulerAngles;
                prop.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif