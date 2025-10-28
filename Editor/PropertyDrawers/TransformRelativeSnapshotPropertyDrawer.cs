#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace Jenga {
    [CustomPropertyDrawer(typeof(TransformRelativeSnapshot))]
    public class TransformRelativeSnapshotPropertyDrawer 
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

            var propPosition = property.FindPropertyRelative("localPosition");
            var propAngles = property.FindPropertyRelative("localEuler");
            var propScale = property.FindPropertyRelative("localScale");
            var propParent = property.FindPropertyRelative("parent");

            // Draw label

            var firstLine = position.LineCut(out position);

            firstLine = EditorGUI.PrefixLabel(
                firstLine, GUIUtility.GetControlID(FocusType.Passive), label
            );

            var rectButton = firstLine.RightCut(100f, out firstLine);
            var rectParent  = position.LineCut(out position);
            var rectPosition = position.LineCut(out position);
            var rectAngles  = position.LineCut(out position);
            var rectScale   = position.LineCut(out position);

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
                EditorGUI.PropertyField(rectParent, propParent);
                EditorGUI.PropertyField(rectScale, propScale);
            }

            EditorGUI.indentLevel--;
            EditorGUI.EndProperty();
        }

        public override float
        GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var link = property.GetLink();

            if (link == selected) 
                return EditorGUIUtility.singleLineHeight * 5f; 

            return EditorGUIUtility.singleLineHeight;
        }

        static void OnSceneGUI(SceneView sv) {
            // if (selectedKey != null)
            if (selected == null) return;
            var prop = selected.GetProperty();
            if (prop == null) return;

            var propPosition = prop.FindPropertyRelative("localPosition");
            var propAngles = prop.FindPropertyRelative("localEuler");
            var propScale = prop.FindPropertyRelative("localScale");
            var propParent = prop.FindPropertyRelative("parent");
            var parent = propParent.objectReferenceValue as Transform;

            var pos = parent != null 
                ? parent.TransformPoint(propPosition.vector3Value)
                : propPosition.vector3Value;

            var rot = parent != null 
                ? parent.rotation * Quaternion.Euler(propAngles.vector3Value)
                : Quaternion.Euler(propAngles.vector3Value);

            var scale = propScale.vector3Value;

            EditorGUI.BeginChangeCheck();
            Handles.TransformHandle(ref pos, ref rot, ref scale);
        
            if (EditorGUI.EndChangeCheck()) {
                propAngles.vector3Value = parent != null 
                    ? (Quaternion.Inverse(parent.rotation) * rot).eulerAngles
                    : rot.eulerAngles;

                propPosition.vector3Value = parent != null
                    ? parent.InverseTransformPoint(pos)
                    : pos;

                propScale.vector3Value = scale;
                prop.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
#endif