#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
namespace Jenga {
    [CustomEditor(typeof(VisualInterface))]
    [CanEditMultipleObjects]
    public class VisualInterfaceEditor : Editor {

        public override VisualElement CreateInspectorGUI() {
            // 
            var propStates = serializedObject.FindProperty("stateDescriptions");

            //
            var root = new VisualElement();
            var stateList = new ListView() {
                bindingPath = "stateDescriptions",
                virtualizationMethod 
                    = CollectionVirtualizationMethod.DynamicHeight,

                showFoldoutHeader = true,
                headerTitle = "State Descriptions",
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                showAddRemoveFooter = true,
                showBoundCollectionSize = false,
                selectionType = SelectionType.Single,

                makeItem = () => new PropertyField() { label = "" },
            };


            stateList.itemIndexChanged += (i, j) => stateList.Rebuild();

            stateList.itemsAdded += (indices) => {
                int maxID = 0;
                for (var i = 0; i < propStates.arraySize; ++i) {
                    var propID = propStates.GetArrayElementAtIndex(i)
                        .FindPropertyRelative("id");

                    maxID = propID.intValue > maxID ? propID.intValue : maxID;
                }

                foreach (var index in indices) {

                    var propState = propStates.GetArrayElementAtIndex(index);
                    var propID = propState.FindPropertyRelative("id");
                    var propName = propState.FindPropertyRelative("name");
                    var propCnd = propState.FindPropertyRelative("condition")
                        .FindPropertyRelative("serializedValue");

                    propID.intValue = maxID + 1;
                    propName.stringValue = maxID == 0 ? "Enabled" : "New State";
                    propCnd.managedReferenceValue = null;
                    // propCnd.boxedValue = new MonoConditionReference() 
                    //     { serializedValue = new ConstCondition() };

                    maxID++;
                }

                serializedObject.ApplyModifiedProperties();
                stateList.Rebuild();
            };

            var propDelay = serializedObject.FindProperty("delayBeforeStart");
            root.Add(new PropertyField(propDelay));
            root.Add(stateList);
            root.Add(new HelpBox() {
                text = "Earlier state in list takes priority. \n"
                    +  "0 is always 'Disabled' state. \n"
                    +  "Only states between 1 and 31 allowed",
                messageType = HelpBoxMessageType.Info
            });

            var debug = new IntegerField() {
                label = "Current State", bindingPath = "state"
            };

            debug.SetEnabled(false);

            root.Add(debug);

            return root;
        }
    }

    [CustomPropertyDrawer(typeof(VisualInterface.StateDescription))]
    public class VisualInterfaceDescriptionEditor : PropertyDrawer {

        public override VisualElement CreatePropertyGUI(SerializedProperty prop) {

            var root = new VisualElement();

            var header = new VisualElement() {
                style = { flexDirection = FlexDirection.Row }
            };

            header.Add(new IntegerField() { 
                bindingPath = "id", style = { minWidth = 30f }

            });
            header.Add(new TextField() { 
                bindingPath = "name", style = { flexGrow = 1f }
            });

            root.Add(header);
            root.Add(new PropertyField() { bindingPath = "condition" });

            return root;
        }
    }
}

#endif