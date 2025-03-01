using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // Attribute Inspector UI Layouting system
    // Works only with UI Toolkit
    public static class ALay {

        // Couple classes to reduce code duplication
        // Dont use them
        [System.AttributeUsage(
            System.AttributeTargets.Field
            | System.AttributeTargets.Class
            | System.AttributeTargets.Struct
        )]
        public class FieldAttribute : System.Attribute { }

        [System.AttributeUsage(System.AttributeTargets.Method)]
        public class MethodAttribute : System.Attribute { }

        // Inherit from this interface to enable ALay layouting
        public interface ILayoutMe { }

        // Add to enable ALay for single item
        public class LayoutMeAttribute : PropertyAttribute { }

        // Add to hide property label
        public class HideLabelAttribute : FieldAttribute { }
        public class HideHeaderAttribute : FieldAttribute { }
        public class UseRootLabelAttribute : FieldAttribute { }

        // Add to replace property label
        public class LabelAttribute : FieldAttribute { 
            public string value;
            public LabelAttribute(string value) => this.value = value; 
        }

        // Add to lists and arrays to generate custom listView   
        public class ListViewAttribute : FieldAttribute { 
            public bool reorderable = true;
            public bool showFoldoutHeader = true;
            public bool showAddRemoveFooter = true;
            public bool showBoundCollectionSize = false;

            // Callbacks must be static methods taking SerializedProperty arg
            // Callbacks must be in #if UNITY_EDITOR ... #endif region
            public string addItemCallback = null;
        }

        // Add to method to create button
        // Method should be in #if UNITY_EDTIOR ... #endif region
        public class ButtonAttribute : MethodAttribute {
            public string label = null;
        }

        // Add type selector button in header
        // Property should be managedReference
        public class TypeSelectorAttribute : FieldAttribute {
            public System.Type typeFamily;
            public string path = null;
            public TypeSelectorAttribute(System.Type typeFamily)
                => this.typeFamily = typeFamily;
        }
        
        // Begin row group starting from item
        public class BeginRowGroupAttribute : FieldAttribute { }

        // Places group in parent's header, or in header at path
        public class PlaceInHeaderAttribute : FieldAttribute {
            public string path = null; // path is relative to parent
            public int pos = 0; 
        }

        // End group on item
        public class EndGroupAttribute : FieldAttribute { }

        public class InlineAttribute : FieldAttribute { }


        // Make callback that triggered when field is changed
        // Method should be of signature:
        // static void OnItemChange(SerializedProperty property) { ... }
        public class OnChangeCallbackAttribute : FieldAttribute {
            public string methodName;
            public bool inClass;
            public OnChangeCallbackAttribute(string name) {
                methodName = name;
            }
        }

        // Make method that emits custom field
        // Method should be of signature:
        // static FieldAttribute[] EmitItem() { ... }
        public class EmitFieldAttribute : MethodAttribute { }
        public class DelayAttributeAttribute : FieldAttribute { 
            public string name;
            public bool inClass = false;
            public DelayAttributeAttribute(string name) => this.name = name;
        }

        // Add enable/disable toggle to property
        public class UsageToggleAttribute : FieldAttribute {
            // Path to property for toggle relative to declaring class
            public string path;
            public UsageToggleAttribute(string path) => this.path = path;
        }

        // Add flexGrow to item
        public class FlexGrowAttribute : FieldAttribute { 
            public float value;
            public FlexGrowAttribute(float value) => this.value = value; 
        }

        // Add flexShrink to item
        public class FlexShrinkAttribute : FieldAttribute { 
            public float value;
            public FlexShrinkAttribute(float value) => this.value = value; 
        }

        // Add minWidth to item
        public class MinWidthAttribute : FieldAttribute { 
            public float value;
            public MinWidthAttribute(float value) => this.value = value; 
        }

        // Add maxWidth to item
        public class MaxWidthAttribute : FieldAttribute { 
            public float value;
            public MaxWidthAttribute(float value) => this.value = value; 
        }


    }
}
