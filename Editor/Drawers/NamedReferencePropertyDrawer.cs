#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace Jenga {
    public class NamedReferencePropertyDrawer<T, UsageStrategy> : PropertyDrawer
        where UsageStrategy : INamedReferenceUsageStrategy<T> {

        public override VisualElement CreatePropertyGUI(
            SerializedProperty prop
        ) {
            var propID = prop.FindPropertyRelative("id");
            var propRegistry = prop.FindPropertyRelative("registry");

            var registry = (NamedReferenceRegistry<T, UsageStrategy>)
                propRegistry.objectReferenceValue;

            var root = new VisualElement();
            var header = new VisualElement() { 
                style = { 
                    flexDirection = FlexDirection.Row
                }
            };

            // var label = new Label() { text = preferredLabel };
            var box = new HelpBox() { style = { display = DisplayStyle.None } };

            var registryField = new PropertyField() { 
                bindingPath = propRegistry.propertyPath,
                label = preferredLabel,
                style = { flexGrow = 1f }
            };

            var dropdown = new PopupField<int>() {
                choices = registry?.references.pairs
                    .Select(x => x.key).ToList() ?? new List<int>(),
                index = registry?.references.pairs
                    .FindIndex(x => x.key == propID.intValue) ?? -1,
                formatListItemCallback 
                    = (id) => registry?.GetName(id) ?? "[No Registry]",
                formatSelectedValueCallback 
                    = (id) => registry?.GetName(id) ?? "[No Registry]",
                style = { minWidth = 150f }
            };

            dropdown.RegisterValueChangedCallback((evt) => {
                propID.intValue = evt.newValue;
                propID.serializedObject.ApplyModifiedProperties();
            });

            box.schedule.Execute(() => {
                var newRegistry = (NamedReferenceRegistry<T, UsageStrategy>)
                    propRegistry.objectReferenceValue;

                if (registry != newRegistry) {
                    registry = newRegistry;
                    dropdown.choices = registry?.references.pairs
                        .Select(x => x.key).ToList() ?? new List<int>();
                    dropdown.index = registry?.references.pairs
                        .FindIndex(x => x.key == propID.intValue) ?? -1;
                }

                var id = propID.intValue;
                var rf = registry != null ? registry.Get(id) : default(T);

                box.style.display 
                    = rf == null ? DisplayStyle.Flex : DisplayStyle.None;
                box.text = 
                    registry == null ? "Select registry"
                    : id <= 0 ? "Select reference" 
                    : "Reference is missing in Reference Master";
                box.messageType = HelpBoxMessageType.Error; 

            }).Every(1000).StartingIn(0);


            header.Add(registryField);
            header.Add(dropdown);

            root.Add(header);
            root.Add(box);

            return root;
        }
    }

    [CustomPropertyDrawer(typeof(NamedReference<RNGAsset>))]
    public class NamedRNGAssetReference 
        : NamedReferencePropertyDrawer<RNGAsset, NoUsageStartegy<RNGAsset>> { }
}

#endif