using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Jenga {
    public static class TypeExtensions {

        public static T 
        GetAttribute<T>(this System.Type type, bool inherit = false) 
        where T : System.Attribute {
            if (type == null) return null;
            var attrs = type.GetCustomAttributes(typeof(T), inherit);
            return attrs.Length > 0 ? attrs[0] as T : null; 
        }
        public static bool 
        TryGetAttribute<T>(this System.Type type, out T t, bool inherit = false)
        where T : System.Attribute 
            => (t = type.GetAttribute<T>(inherit)) != null;

        public static T 
        GetAttribute<T>(this MemberInfo mb, bool inherit = false) 
        where T : System.Attribute {
            if (mb == null) return null;
            var attrs = mb.GetCustomAttributes(typeof(T), inherit);
            return attrs.Length > 0 ? attrs[0] as T : null; 
        }
        public static bool 
        TryGetAttribute<T>(this MemberInfo mb, out T t, bool inherit = false)
        where T : System.Attribute 
            => (t = mb.GetAttribute<T>(inherit)) != null;

#if UNITY_EDITOR
        public static bool TryGetFieldWithAttribute<T>(
            this System.Type type, out FieldInfo field
        ) where T : System.Attribute {
            foreach (var fi in type.GetFields())
                if ((fi as MemberInfo).TryGetAttribute<T>(out var _)) 
                    { field = fi; return true; }   

            field = null;
            return false; 
        }
#endif

    }
}
