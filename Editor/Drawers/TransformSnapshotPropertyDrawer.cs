#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace Jenga {
    [CustomPropertyDrawer(typeof(TransformSnapshot))]
    public class TransformSnapshotPropertyDrawer : PropertyDrawer {

        static Button editButton = null;

        static void StartEditing(Button button, SerializedProperty prop) {
            StopEditing();

            var comp = prop.serializedObject.targetObject as MonoBehaviour;
            if (comp == null) return;
            var transform = comp.GetComponent<Transform>();
            if (transform == null) return;

            button.text = "Stop Edit";
            button.style.backgroundColor = new Color(0f, .4f, 0f, 1f);

            var path = prop.propertyPath;

            {
                var propPos   = prop.FindPropertyRelative("localPosition");
                var propScale = prop.FindPropertyRelative("localScale");
                var propRot   = prop.FindPropertyRelative("localEuler");

                Undo.RecordObject(transform, "Snap transform to snapshot");

                transform.localPosition = propPos.vector3Value;
                transform.localScale = propScale.vector3Value;
                transform.localEulerAngles = propRot.vector3Value;
            }

            button.schedule.Execute(() => {
                if (editButton != button) return;

                var so = new SerializedObject(comp);
                var newProp = so.FindProperty(path);
                var propPos   = newProp.FindPropertyRelative("localPosition");
                var propScale = newProp.FindPropertyRelative("localScale");
                var propRot   = newProp.FindPropertyRelative("localEuler");

                propPos.vector3Value = transform.localPosition;
                propScale.vector3Value = transform.localScale;
                propRot.vector3Value = transform.localEulerAngles;

                so.ApplyModifiedProperties();

                // Debug.Log($"{path}, {propPos.vector3Value}");
            }).Until(() => editButton != button).Every(200).StartingIn(200);

            editButton = button;
        }

        static void StopEditing() {
            if (editButton != null) {
                editButton.text = "Edit";
                editButton.style.backgroundColor = Color.gray;
            }

            editButton = null;
        }

        static void ToggleEditing(Button button, SerializedProperty prop) {
            if (editButton == button) {
                StopEditing();
            } else {
                StartEditing(button, prop);
            }
        }

        public override VisualElement CreatePropertyGUI(
            SerializedProperty prop
        ) {

            var root = new VisualElement() { 
                style = { flexDirection = FlexDirection.Row }
            };

            var foldout = new Label() { 
                text = preferredLabel, 
                style = { 
                    minWidth 
                        = preferredLabel != "" ? EditorGUIUtility.labelWidth : 0f 
                }
            };
            var button = new Button() { 
                text = "Edit", style = { flexGrow = 1f } 
            };

            button.style.backgroundColor = Color.gray;
            button.clicked += () => ToggleEditing(button, prop);

            // foldout.Add(new Vector3Field() { bindingPath = "localPosition" });
            // foldout.Add(new Vector3Field() { bindingPath = "localScale" });
            // foldout.Add(new Vector3Field() { bindingPath = "localEuler" });

            root.Add(foldout);
            root.Add(button);
            return root;
        }
    }
}
#endif