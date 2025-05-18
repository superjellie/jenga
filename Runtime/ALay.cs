using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Jenga {

    // Attribute Inspector UI Layouting system
    // Works only with UI Toolkit
    public static class ALay {

        // Couple classes to reduce code duplication
        [System.AttributeUsage(
            System.AttributeTargets.Field
            | System.AttributeTargets.Class
            | System.AttributeTargets.Struct
        )]
        public class FieldAttribute : System.Attribute { }

        [System.AttributeUsage(
            System.AttributeTargets.Class
            | System.AttributeTargets.Struct
        )]
        public class ClassAttribute : System.Attribute { }

        [System.AttributeUsage(System.AttributeTargets.Method)]
        public class MethodAttribute : System.Attribute { }

        // Inherit from this interface to enable ALay layouting
        public interface ILayoutMe { }

        // Add to hide property label
        public class HideLabelAttribute : FieldAttribute { }
        public class HideHeaderAttribute : FieldAttribute { }

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

            // Callbacks must be in #if UNITY_EDITOR ... #endif region

            public string afterAddItemCallback    = null;
            // static void AfterAddItem(SerializedProperty list, int index);

            public string beforeRemoveItemCallback = null;
            // static void BeforeRemoveItem(SerializedProperty list, int index);

            public string beforeMoveItemCallback   = null;
            // static void BeforeMoveItem
            //      (SerializedProperty list, int from, int to);
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

        // Place in current property header property at path inside class
        public class PlaceInHeaderAttribute : ClassAttribute {
            public string path = null; // path is relative to parent
            public int pos = 0; 

            public PlaceInHeaderAttribute(string path, int pos) { 
                this.path = path;
                this.pos = pos;
            }
        }

        // Inline current class 
        public class InlineAttribute : ClassAttribute { }
        
        // Places reference field into header and links together
        // all objects with same non-empty refName
        public class MatchReferencesAttribute : ClassAttribute { 
            public string pathToSerializedReference = "value";

            // uses string property to match references inside serialized ref
            public string pathToRefName = "refName";   
        }

        // Make callback that triggered when field is changed
        // Method should be of signature:
        // static void OnItemChange(SerializedProperty property) { ... }
        public class OnChangeCallbackAttribute : FieldAttribute {
            public string methodName;
            public bool inClass;
            public OnChangeCallbackAttribute(string name) { methodName = name; }
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
        public class StyleAttribute : FieldAttribute { 
            public float flexGrow       = float.NaN;
            public float flexShrink     = float.NaN;
            public float minWidth       = float.NaN;
            public float minHeight      = float.NaN;
            public float maxWidth       = float.NaN;
            public float maxHeight      = float.NaN;
            public float width          = float.NaN;
            public float height         = float.NaN;
            public float marginLeft     = float.NaN;
            public float marginRight    = float.NaN;
            public float marginTop      = float.NaN;
            public float marginBottom   = float.NaN;

            public bool applyToContent  = false;
            public bool hideCheckmark   = false;
        }

        // Makes dropdown with generic options
        public class OptionsAttribute : FieldAttribute {
            
            // Must be static void function name taking
            // SerializedProperty and OptionsAttribute.Map attributes
            // This function should populate options (name, value) list 
            public string provider = null;

            public OptionsAttribute(string provider) 
                => this.provider = provider; 

            public class Map {
                public List<(string, object)> options = new();
                public List<string> updateWith = new();
                public string error = null;

                // Set error if you cant populate correctly
                public void SetError(string error) => this.error = error; 
                public void UpdateWith(string path) => updateWith.Add(path);
                public void Add<T>(string name, T value) 
                    => options.Add((name, value));
            }

        }

        // Adds toggable preview functionality for property
        // You should add [ScenePreviewRoot] to root field
        public class ScenePreviewAttribute : ClassAttribute { }

        public class ScenePreviewRootAttribute : FieldAttribute { }

        // State Manipulators
    }
}
