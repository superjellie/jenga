using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardPhantom.RuntimeTypeCache;
using UnityEngine.Scripting;

namespace Jenga {
    
    [RequireDerived]
    public abstract class VectorAdapterF<T> {

        // Default Adapter implementation for T
        // Searches automaticly for non-generic child class
        public static VectorAdapterF<T> main = null;

        // Overload to implement operations
        public abstract T     Zero();
        public abstract int   Dim(T x);
        public abstract float Get(T x, int index);
        public abstract void  Set(ref T x, int index, float value);

        // Private
        static VectorAdapterF() {
            var cch = GlobalTypeCache.GetTypesDerivedFrom<VectorAdapterF<T>>();
            
            foreach (var type in cch) {
                if (!type.IsAbstract) {
                    main = System.Activator.CreateInstance(type)    
                        as VectorAdapterF<T>;
                    return;
                }
            }

            throw new System.NotSupportedException(
                $"Type {typeof(T).FullName} has no VectorAdapter.",
                null
            );
        }
    }

}
