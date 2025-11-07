using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Scripting;
using Unity.Properties;
using BeardPhantom.RuntimeTypeCache;

namespace Jenga {

    [System.Serializable]
    public struct GameObjectProperty<TOwner, TValue> 
    where TOwner : class {
        [SerializeField] string hierarchyPath;
        [SerializeField] string componentTypeName;
        [SerializeField] string componentPath;
        [SerializeField] string externalPropertyID;

        public delegate void   SetterFunc(TOwner target, TValue value);
        public delegate TValue GetterFunc(TOwner target);

        public GameObjectProperty(
            string hierarchyPath, string componentTypeName,
            string componentPath, string externalPropertyID
        ) {
            this.hierarchyPath = hierarchyPath;
            this.componentTypeName = componentTypeName;
            this.componentPath = componentPath;
            this.externalPropertyID = externalPropertyID;
        }

        public bool TryGetValue(GameObject go, out TValue value) {
            value = default(TValue);
            // var t = go.transform.Find(hierarchyPath);
            // if (t == null) return false;

            // var comp = t.GetComponent(componentTypeName);
            // if (comp == null) return false;

            // if (string.IsNullOrEmpty(externalPropertyID)) {
            //     if (PropertyContainer
            //             .TryGetValue(ref comp, componentPath, out value))  
            //         return true;
            // } else if (PropertyContainer.TryGetValue(
            //     ref comp, componentPath, out TOwner x
            // )) {
            //     if (TryGetGetter(externalPropertyID, out var getter)) {
            //         value = (T)getter(ref x);
            //         return true;
            //     }
            // }

            return false;
        }

        public bool TrySetValue(GameObject go, TValue value) {
            value = default(TValue);
            // var t = go.transform.Find(hierarchyPath);
            // if (t == null) return false;

            // var comp = t.GetComponent(componentTypeName);
            // if (comp == null) return false;

            // if (string.IsNullOrEmpty(externalPropertyID)) {
            //     if (PropertyContainer
            //             .TrySetValue(ref comp, componentPath, value)) 
            //         return true;
            // } else if (PropertyContainer.TryGetValue(
            //     ref comp, componentPath, out TOwner x
            // )) {
            //     if (TryGetSetter(externalPropertyID, out var setter)) {
            //         setter(x, value);
            //         return true;
            //     }
            // }

            return false;
        }

        public override string ToString() 
            => $"{hierarchyPath}:{componentTypeName}:{componentPath}"
            + $":{externalPropertyID}";


    //
        void Update() {
            // go.transform.Find(hierarchyPath);
        }   

        static bool TryGetSetter(string id, out SetterFunc setter) {
            setter = null;
            var mId = new GameObjectProperty.MethodID() 
                { tOwner = typeof(TOwner), tValue = typeof(TValue), id = id };

            if (!GameObjectProperty.externalSetters
                    .TryGetValue(mId, out var methodInfo))
                return false;
            
            setter = (SetterFunc)methodInfo.CreateDelegate(typeof(SetterFunc));
            return setter != null;
        }

        static bool TryGetGetter<T>(string id, out GetterFunc getter) { 
            getter = null;
            var mId = new GameObjectProperty.MethodID() 
                { tOwner = typeof(TOwner), tValue = typeof(TValue), id = id };

            if (!GameObjectProperty.externalGetters
                    .TryGetValue(mId, out var methodInfo))
                return false;
            
            getter = (GetterFunc)methodInfo.CreateDelegate(typeof(GetterFunc));
            return getter != null;
        }
    }

    public static class GameObjectProperty {

        [System.AttributeUsage(System.AttributeTargets.Method)]
        [RequireAttributeUsages]
        public class SetterAttribute : System.Attribute {
            public string externalPropertyID;
            public SetterAttribute(string id) => externalPropertyID = id;
        }

        [System.AttributeUsage(System.AttributeTargets.Method)]
        [RequireAttributeUsages]
        public class GetterAttribute : System.Attribute {
            public string externalPropertyID;
            public GetterAttribute(string id) => externalPropertyID = id;
        }
        

    // Private

        public struct MethodID {
            public System.Type tOwner;
            public System.Type tValue;
            public string      id;

            public override bool Equals(object other)
                => other is MethodID otherID
                && otherID.tOwner == tOwner
                && otherID.tValue == tValue
                && otherID.id == id;

            public override int GetHashCode()
                => (tOwner, tValue, id).GetHashCode();
        }

        public static Dictionary<MethodID, MethodInfo>
            externalSetters = new();
        public static Dictionary<MethodID, MethodInfo>
            externalGetters = new();

    
        static GameObjectProperty() {
            var getters 
                = GlobalTypeCache.GetMethodsWithAttribute<GetterAttribute>();
            var setters 
                = GlobalTypeCache.GetMethodsWithAttribute<SetterAttribute>();
            
            foreach (var getter in getters) {
                var attr = getter.GetCustomAttribute<GetterAttribute>();
                var tValue = getter.ReturnType; 
                var prms = getter.GetParameters();
                if (prms.Length != 1) continue;
                var tOwner = prms[0].ParameterType;
                var id = new MethodID() { 
                    tOwner = tOwner, tValue = tValue, 
                    id = attr.externalPropertyID 
                };

                externalGetters.Add(id, getter);
            }

            foreach (var setter in setters) {
                var attr = setter.GetCustomAttribute<GetterAttribute>();
                var prms = setter.GetParameters();
                if (prms.Length != 2) continue;
                var tOwner = prms[0].ParameterType;
                var tValue = prms[1].ParameterType;
                var id = new MethodID() { 
                    tOwner = tOwner, tValue = tValue, 
                    id = attr.externalPropertyID 
                };

                externalSetters.Add(id, setter);
            }
        }
    }


}
