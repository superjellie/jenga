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
        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label
        ) {
            var propID = property.FindPropertyRelative("id");
            var propRegistry = property.FindPropertyRelative("registry");

            var registry = (NamedReferenceRegistry<T, UsageStrategy>)
                propRegistry.objectReferenceValue;

            bool hasRegistry = registry != null;
            bool hasCorrectId = hasRegistry && registry.HasID(propID.intValue);

            // var lw = EditorGUIUtility.labelWidth;
            var rectLine = position.LineCut(out position);
            var rectDropdown = rectLine.RightCut(150f, out rectLine)
                .LeftExtend(25f); 
            var rectRegistry = rectLine;
            var rectHelp     = position.LineCut(out position);

            // EditorGUI.LabelField(rectName, label);
            propRegistry.objectReferenceValue = EditorGUI.ObjectField(
                rectRegistry, label, registry, 
                typeof(NamedReferenceRegistry<T, UsageStrategy>), 
                false
            );

            if (hasRegistry) {
                int i = hasCorrectId ? 0 : 1;
                var options = new string[registry.CountItems() + i]; 
                var ids = new int[registry.CountItems() + i];
                
                if (!hasCorrectId) {
                    ids[0] = propID.intValue;
                    options[0] = registry.GetName(ids[0]);
                }

                foreach (var (key, value) in registry.references) {
                    ids[i] = key;
                    options[i++] = value.name;
                }

                var index = System.Array.IndexOf(ids, propID.intValue);
                index = EditorGUI.Popup(rectDropdown, index, options);

                propID.intValue = ids[index];
            }

            if (!hasRegistry || !hasCorrectId)
                EditorGUI.HelpBox(
                    rectHelp, 
                    !hasRegistry ? "Select registry"
                    : propID.intValue <= 0 ? "Select reference" 
                    : "Reference is missing in Reference Master",
                    MessageType.Error
                );            
        } 

        public override float 
        GetPropertyHeight(SerializedProperty property, GUIContent label) {
            var propID = property.FindPropertyRelative("id");
            var propRegistry = property.FindPropertyRelative("registry");
            var registry = (NamedReferenceRegistry<T, UsageStrategy>)
                propRegistry.objectReferenceValue;
            bool hasRegistry = registry != null;
            bool hasCorrectId = hasRegistry && registry.HasID(propID.intValue);

            var position = new Rect(0f, 0f, 0f, 0f);
            var startY = position.yMin;
            var rectLine = position.LineCut(out position);
            var rectHelp = position.LineCut(out position);
            var endY = hasRegistry && hasCorrectId 
                ? rectLine.yMax : rectHelp.yMax;
            return endY - startY;
        } 

    }

    [CustomPropertyDrawer(typeof(NamedReference<RNGAsset>))]
    public class NamedRNGAssetReference 
        : NamedReferencePropertyDrawer<RNGAsset, NoUsageStartegy<RNGAsset>> { }
}

#endif