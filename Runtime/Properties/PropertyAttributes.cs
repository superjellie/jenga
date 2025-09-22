using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    public class UsageToggleAttribute : PropertyAttribute { 
        public string path;
        public UsageToggleAttribute(string path) => this.path = path;
    }

    // Inlines all properties inside given property
    // Pair with style attributes on children properties
    public interface IInlineMe { }

    public class StyleAttribute : PropertyAttribute { 
        public float flexGrow       = float.NaN;    // NOT IMPLEMENTED
        public float flexShrink     = float.NaN;    // NOT IMPLEMENTED
        public float minWidth       = float.NaN;    // NOT IMPLEMENTED
        public float minHeight      = float.NaN;    // NOT IMPLEMENTED
        public float maxWidth       = float.NaN;    // NOT IMPLEMENTED
        public float maxHeight      = float.NaN;    // NOT IMPLEMENTED
        public float width          = float.NaN;    // NOT IMPLEMENTED
        public float height         = float.NaN;    // NOT IMPLEMENTED
        public float marginLeft     = float.NaN;    // NOT IMPLEMENTED
        public float marginRight    = float.NaN;    // NOT IMPLEMENTED
        public float marginTop      = float.NaN;    // NOT IMPLEMENTED
        public float marginBottom   = float.NaN;    // NOT IMPLEMENTED
    }

    public class HideLabelAttribute : PropertyAttribute { }

    public class TypeMenuAttribute : PropertyAttribute { }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class WrapperAttribute : System.Attribute { }
}
