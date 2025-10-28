using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BeardPhantom.RuntimeTypeCache;

namespace Jenga {

    public abstract class VectorAdapterf<T> {

        // Default Adapter implementation for T
        // Searches automaticly for non-generic child class
        public static VectorAdapterf<T> main = null;

        // Overload to implement operations
        public abstract T     Zero();
        public abstract int   Dim(T x);
        public abstract float Get(T x, int index);
        public abstract void  Set(ref T x, int index, float value);

        // Private
        static VectorAdapterf() {
            var cch = GlobalTypeCache.GetTypesDerivedFrom<VectorAdapterf<T>>();
            
            foreach (var type in cch) {
                if (!type.IsAbstract) {
                    main = System.Activator.CreateInstance(type)    
                        as VectorAdapterf<T>;
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
