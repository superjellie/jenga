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
        public static string ussClassName 
            = "jenga-field-container";

        // Style classes (apply to root)
        public static string ussNoLabelClassName 
            = "jenga-field-container--no-label";
        public static string ussNoToggleClassName 
            = "jenga-field-container--no-toggle";
        public static string ussNoMarginClassName 
            = "jenga-field-container--no-margin";
        public static string ussInlineClassName 
            = "jenga-field-container--inline";
        public static string ussNoHeaderClassName 
            = "jenga-field-container--no-header";

        // Parts
        public Foldout foldout = new();
        public VisualElement header => foldout.hierarchy[0][0];
        public VisualElement content => foldout.hierarchy[1];
        // public Label labelElement;


        //
        public string label {
            get => foldout.text;
            set => foldout.text = value;
        }

        public bool inline {
            get => ClassListContains(ussInlineClassName);
            set {
                EnableInClassList(ussInlineClassName, value);
                foldout.value = true;
            }
        }

        public bool hideToggle {
            get => ClassListContains(ussNoToggleClassName);
            set {
                EnableInClassList(ussNoToggleClassName, value);
                foldout.value = true;
            }
        }

        public bool hideHeader {
            get => ClassListContains(ussNoHeaderClassName);
            set {
                EnableInClassList(ussNoHeaderClassName, value);
                foldout.value = true;
            }
        }

        public bool removeContentMargins {
            get => ClassListContains(ussNoMarginClassName);
            set => EnableInClassList(ussNoMarginClassName, value);
        }

        public FieldContainer() {
            EnableInClassList(ussClassName, true);
            Add(foldout);
            styleSheets.Add(SerializedPropertyUtility.jengaStyle);

            
            foldout.RegisterCallback<ChangeEvent<bool>>((evt) => {
                if ((hideToggle || inline || hideHeader) && !evt.newValue)
                    foldout.value = true;
            });

            RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
        }


        // Code from unity sources, BaseField.cs
        VisualElement cachedInspectorElement;
        VisualElement cachedContextWidthElement;

        void OnAttachToPanel(AttachToPanelEvent e) {
            if (e.destinationPanel == null) return;
            if (e.destinationPanel.contextType == ContextType.Player) return;

            cachedInspectorElement = null;
            cachedContextWidthElement = null;

            for (var x = parent; x != null; x = x.parent) {
                if (x.ClassListContains("unity-inspector-element"))
                    cachedInspectorElement = x;
                if (x.ClassListContains("unity-inspector-main-container")) {
                    cachedContextWidthElement = x;
                    break;
                }
            }
            
            if (content.childCount == 0)
                hideToggle = true;

            RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
            // labelElement = header.Q<Label>(className: "unity-foldout__text");
        }
    
    void OnGeometryChanged(GeometryChangedEvent evt) {
            if (!inline || cachedInspectorElement == null) return;

            var m_LabelWidthRatio = 0.45f;
            var m_LabelExtraPadding = 37.0f;
            var m_LabelBaseMinWidth = 123.0f;

            var totalPadding = m_LabelExtraPadding;
            var spacing = worldBound.x 
                - cachedInspectorElement.worldBound.x 
                - cachedInspectorElement.resolvedStyle.paddingLeft;

            totalPadding += spacing;
            totalPadding += resolvedStyle.paddingLeft;

            var minWidth = m_LabelBaseMinWidth 
                - spacing - resolvedStyle.paddingLeft;

            var contextWidthElement = cachedContextWidthElement 
                ?? cachedInspectorElement;

            var toggle = foldout.hierarchy[0];

            toggle.style.minWidth = Mathf.Max(minWidth, 0);

            // Formula to follow IMGUI label width settings
            var newWidth 
                = Mathf.Ceil(contextWidthElement.resolvedStyle.width 
                        * m_LabelWidthRatio) - totalPadding;

            if (Mathf.Abs(toggle.resolvedStyle.width - newWidth) > .001f)
                toggle.style.width = Mathf.Max(0f, newWidth);
        }

    }
}
#endif