#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Jenga {
    public class ManagedReferenceField : VisualElement {

        //
        public const string ussManagedReferenceClassName 
            = "custom-managed-reference-field";
        public const string ussPropertyFieldClassName 
            = "custom-managed-reference-field__property-field";
        public const string ussTypeSelectorClassName 
            = "custom-managed-reference-field__type-selector";

        public const string ussInlinedClassName
            = "custom-managed-reference-field__inlined";

        //
        public System.Type typeFamily;
        public SerializedProperty property;
        public string labelText;

        //
        public FieldContainer container;
        public TypeSelectorField typeSelector;
        public PropertyField propertyField;

        public ManagedReferenceField() {
            container = new FieldContainer();    
            Add(container);        

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            styleSheets.Add(SerializedPropertyUtility.jengaStyle);
            
            EnableInClassList(ussManagedReferenceClassName, true);

        }

        System.Type currentType 
            => SerializedPropertyUtility.GetManagedType(property);

        void OnAttachToPanel(AttachToPanelEvent evt) {
            container.label = labelText;
            RebuildHierarchy();
        }

        void RebuildHierarchy() {
            if (propertyField != null)
                propertyField.RemoveFromHierarchy();
            if (typeSelector != null)
                typeSelector.RemoveFromHierarchy();

            this.Unbind();

            // if ()
            propertyField = new PropertyField() 
                { bindingPath = property.propertyPath, label = "" };
            propertyField.EnableInClassList(ussPropertyFieldClassName, true);
            // propertyField.EnableInClassList(
            //     SerializedPropertyUtility.ussNoLabelPropertyClassName, true
            // );
            container.content.Add(propertyField);

            typeSelector = new TypeSelectorField() {
                currentType 
                    = SerializedPropertyUtility.GetManagedType(property),
                typeFamily = typeFamily,
                onSelect = (type) => {
                    SerializedPropertyUtility
                        .SetManagedReference(property, type);
                    property.serializedObject.ApplyModifiedProperties();
                    property.serializedObject.Update();
                    
                    RebuildHierarchy();
                },
                style = { 
                    flexGrow = 1f, marginLeft = 2f
                }
            };
            typeSelector.EnableInClassList(ussTypeSelectorClassName, true);

            container.header.Add(typeSelector);

            var doInline = currentType != null &&
                currentType.GetCustomAttributes(
                    typeof(InlinePropertyEditorAttribute), false
                ).Length > 0;

            EnableInClassList(ussInlinedClassName, doInline);

            // Keep hidden foldout open
            // propertyField.schedule.Execute(() => {
            //     var propertyFoldout = propertyField.Query<Foldout>()
            //         .Where(x => x.parent == propertyField).First();

            //     if (propertyFoldout != null) {
            //         propertyFoldout.value = true;
            //     }

            //     if (doInline)
            //         this.value = true;
                    
            // }).StartingIn(100).Every(1000);

            this.Bind(property.serializedObject);
        }
    }
}

#endif