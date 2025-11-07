using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Jenga {
    public static class MemberInfoExtensions {

        public static T GetCustomAttribute<T>(this MemberInfo memb) 
        where T : System.Attribute {
            var attrs = memb.GetCustomAttributes(typeof(T), false);

            if (attrs.Length > 0) return (T)attrs[0];
            return null;
        }

        public static bool 
        TryGetCustomAttribute<T>(this MemberInfo memb, out T attr) 
        where T : System.Attribute 
            => (attr = memb.GetCustomAttribute<T>()) != null;

    }
}
