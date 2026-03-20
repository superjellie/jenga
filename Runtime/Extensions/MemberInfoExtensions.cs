using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Jenga {
    public static class MemberInfoExtensions {

        public static T GetAttribute<T>(this MemberInfo memb) 
        where T : System.Attribute {
            var attrs = memb.GetCustomAttributes(typeof(T), true);

            if (attrs.Length > 0) return (T)attrs[0];
            return null;
        }

        public static bool HasAttribute<T>(this MemberInfo memb) 
        where T : System.Attribute {
            var attrs = memb.GetCustomAttributes(typeof(T), true);
            return attrs.Length > 0;
        }

        public static bool 
        TryGetAttribute<T>(this MemberInfo memb, out T attr) 
        where T : System.Attribute 
            => (attr = memb.GetAttribute<T>()) != null;

    }
}
