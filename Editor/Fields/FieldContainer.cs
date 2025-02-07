#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;


namespace Jenga {
    public class FieldContainer : VisualElement {

        // Element classes
        public static string ussClassName = "custom-field-container";
        public static string ussHeaderClassName 
            = "custom-field-container__header";
        public static string ussLabelClassName 
            = "custom-field-container__label";
        public static string ussContentClassName 
            = "custom-field-container__content";

        // Style classes (apply to root)
        public static string ussNoLabelClassName 
            = "custom-field-container--no-label";
        public static string ussNoMarginClassName 
            = "custom-field-container--no-margin";

        // Parts
        public Foldout foldout;
        public VisualElement header => foldout.hierarchy[0];
        public VisualElement label  => foldout.hierarchy[0][0];
        public VisualElement content => foldout.hierarchy[1];

        //
        public string labelText = null;
    
        public FieldContainer() {
            foldout = new Foldout();

            EnableInClassList(ussClassName, true);
            header.EnableInClassList(ussHeaderClassName, true);
            label.EnableInClassList(ussLabelClassName, true);
            content.EnableInClassList(ussContentClassName, true);

            Add(foldout);
            styleSheets.Add(SerializedPropertyUtility.customStyle);

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }

        void OnAttachToPanel(AttachToPanelEvent evt) {
            EnableInClassList(
                ussNoLabelClassName, string.IsNullOrEmpty(labelText)
            );
            EnableInClassList(
                ussNoMarginClassName, 
                string.IsNullOrEmpty(labelText) 
                && header.childCount <= 1
            );

            foldout.text = labelText;

            if (string.IsNullOrEmpty(labelText)) {
                foldout.value = true;
            } 
        }
    }
}
#endif