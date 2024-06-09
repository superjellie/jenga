using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Jenga {
    
    public class EnumStyleAttribute : PropertyAttribute {
        // public bool includeAssets = false;
        public bool allowNone = true;
    }

    public class ReadOnlyAttribute : PropertyAttribute { }

    [AttributeUsage(AttributeTargets.Method)]
    public class MessageAttribute : Attribute { }

}
