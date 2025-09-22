#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace Jenga {
    [CustomPropertyDrawer(typeof(VisualStateData<>), true)]
    [CustomPropertyDrawer(typeof(VisualTransitionData<>), true)]
    public class VisualStateDataPropertyDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI(SerializedProperty prop) {
            var propMatchers = prop.FindPropertyRelative("matchers");
            var propFallback = prop.FindPropertyRelative("fallback");

            var listView = new ListView() {
                makeItem = () => new PropertyField() { label = "" },
                bindingPath = propMatchers.propertyPath,
                reorderable = true,
                virtualizationMethod 
                    = CollectionVirtualizationMethod.DynamicHeight,
                showFoldoutHeader = false,
                showAddRemoveFooter = true,
                showBoundCollectionSize = false,
                selectionType = SelectionType.None
            };

            listView.itemIndexChanged += (i, j) => listView.Rebuild();
            listView.itemsAdded += (ind) => listView.Rebuild();

            var fallback = new PropertyField() { 
                label = "Fallback",
                bindingPath = propFallback.propertyPath
            };

            var root = new Foldout() { text = preferredLabel, value = true };
            root.Add(fallback);
            root.Add(listView);

            return root;
        }
    }

    [CustomPropertyDrawer(typeof(VisualStateData<>.StateData), true)]
    public class VisualStateMatcherPropertyDrawer : PropertyDrawer {

        public static void UpdateOptions(
            List<string> options, List<int> masks, int myMask, MonoBehaviour comp
        ) {
            options.Clear();
            masks.Clear();

            var vi = comp.GetComponent<VisualInterface>();
            if (vi == null) return;

            options.Add("Disabled");
            masks.Add(1);

            foreach (var desc in vi.stateDescriptions) {
                options.Add(desc.name);
                masks.Add(1 << desc.id);

                if (myMask != ~0) 
                    myMask &= ~(1 << desc.id);
            }

            if (myMask != 0 && myMask != ~0) {
                for (int i = 0; i < 31; ++i) {
                    if (((1 << i) & myMask) == 0) continue;
                    options.Add($"[ID: {i}]");
                    masks.Add(1 << i);
                }
            }

        }

        public override VisualElement CreatePropertyGUI(SerializedProperty prop) {

            var propMask = prop.FindPropertyRelative("mask");
            var propData = prop.FindPropertyRelative("data");

            var pathMask = propMask.propertyPath;

            var comp = prop.serializedObject.targetObject as MonoBehaviour;
            if (comp == null) return new Label("Used only in MonoBehaviours");


            var root = new VisualElement();

            var options = new List<string>();
            var masks = new List<int>();

            var maskField = new MaskField() {
                bindingPath = propMask.propertyPath
            };

            maskField.schedule.Execute(() => {
                var so = new SerializedObject(comp);
                var pMask = so.FindProperty(pathMask);

                var mask = pMask != null ? pMask.intValue :0;
                UpdateOptions(options, masks, mask, comp);
                maskField.choices = options;
                maskField.choicesMasks = masks;
            }).Every(1000).StartingIn(0);

            root.Add(maskField);

            root.Add(new PropertyField() {
                bindingPath = propData.propertyPath,
                label = ""
            });

            return root;
        }
    }

    [CustomPropertyDrawer(typeof(VisualTransitionData<>.TransitionData), true)]
    public class VisualTransitionMatcherPropertyDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI(SerializedProperty prop) {

            var propFromMask = prop.FindPropertyRelative("fromMask");
            var propToMask = prop.FindPropertyRelative("toMask");
            var propData = prop.FindPropertyRelative("data");

            var pathFromMask = propFromMask.propertyPath;
            var pathToMask = propToMask.propertyPath;

            var comp = prop.serializedObject.targetObject as MonoBehaviour;
            if (comp == null) return new Label("Used only in MonoBehaviours");

            var root = new VisualElement();

            var options = new List<string>();
            var masks = new List<int>();

            var maskFromField = new MaskField() {
                bindingPath = propFromMask.propertyPath,
                style = { flexGrow = 1f }
            };
            
            var maskToField = new MaskField() {
                bindingPath = propToMask.propertyPath,
                style = { flexGrow = 1f }
            };
     
            maskFromField.schedule.Execute(() => {
                var so = new SerializedObject(comp);
                var pFrom = so.FindProperty(pathFromMask);
                var pTo = so.FindProperty(pathToMask);

                var mask = (pFrom != null ? pFrom.intValue : 0)
                    & (pTo != null ? pTo.intValue : 0);

                VisualStateMatcherPropertyDrawer.UpdateOptions(
                    options, masks, mask, comp
                );

                maskFromField.choices = options;
                maskFromField.choicesMasks = masks;
                maskToField.choices = options;
                maskToField.choicesMasks = masks;
            }).Every(1000).StartingIn(0);

            var header = new VisualElement() { 
                style = { flexDirection = FlexDirection.Row }
            };

            header.Add(maskFromField);
            header.Add(maskToField);

            root.Add(header);
            root.Add(new PropertyField() {
                bindingPath = propData.propertyPath,
                label = ""
            });

            return root;
        }
    }
}
#endif