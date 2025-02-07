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
        )]
        public class FieldAttribute : System.Attribute { }

        [System.AttributeUsage(System.AttributeTargets.Method)]
        public class MethodAttribute : System.Attribute { }

        // Inherit from this interface to enable ALay layouting
        public interface ILayoutMe { }

        // Add to enable ALay for single field
        public class LayoutFieldAttribute : PropertyAttribute { }

        // Add to hide property label
        public class HideLabelAttribute : FieldAttribute { }

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
        
        // Start inlining starting from item
        public class StartLineAttribute : FieldAttribute { }

        // End inlining on item
        public class EndLineAttribute : FieldAttribute { }

        // Add flexGrow to item
        public class FlexGrowAttribute : FieldAttribute { 
            public float value;
            public FlexGrowAttribute(float value) => this.value = value; 
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

        // Add enable/disable toggle to property
        public class UsageToggleAttribute : FieldAttribute {
            // Path to property for toggle relative to declaring class
            public string path;
            public UsageToggleAttribute(string path) => this.path = path;
        }

        // Skip item but still serialize
        // public class SkipAttribute : FieldAttribute { }

        // Add to method to create button
        // Method should be in #if UNITY_EDTIOR ... #endif region
        public class ButtonAttribute : MethodAttribute {
            public string label = null;
        }

    }
}
