using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // Parametric Vector is a vector with named (by values of T) axes
    public class ParametricVectorF<T> 
    : IEnumerable<KeyValuePair<T, float>>, 
    ISerializationCallbackReceiver,
    ParametricVector.IAxesBag {
        Dictionary<T, float> values = new();

        public float this[T axis] {
            get => values.TryGetValue(axis, out var x) ? x : 0f;
            set => values[axis] = value;
        }


        // Constructors
        public ParametricVectorF() { }

        public ParametricVectorF(params (T, float)[] pairs) {
            foreach (var (axis, value) in pairs)
                this[axis] = value;
        }

        // Iterating
        public IEnumerable<T> Axes() 
            => values.Keys;
        public IEnumerator<KeyValuePair<T, float>> GetEnumerator() 
            => values.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() 
            => values.GetEnumerator();

        // Parametric vectors are not usable with LinAlgf by default,
        // you need to select axes you want to use with ToVector* functions
        public Vector2 ToVector2(T axis1, T axis2)
            => new(this[axis1], this[axis2]);
        public Vector3 ToVector3(T axis1, T axis2, T axis3)
            => new(this[axis1], this[axis2], this[axis3]);
        public Vector4 ToVector4(T axis1, T axis2, T axis3, T axis4)
            => new(this[axis1], this[axis2], this[axis3], this[axis4]);

        // ISerializationCallbackReceiver
        [SerializeField, HideInInspector]
        List<float> serializedValues = new();
        [SerializeField, HideInInspector]
        List<T> serializedKeys = new();

        void ISerializationCallbackReceiver.OnBeforeSerialize() {
            serializedKeys.Clear(); serializedValues.Clear();

            foreach (var (axis, value) in values) {
                serializedKeys.Add(axis);
                serializedValues.Add(value);
            }
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            values.Clear();
            var n = Mathx.Min(serializedKeys.Count, serializedValues.Count);
            for (int i = 0; i < n; ++i)
                values.Add(serializedKeys[i], serializedValues[i]);
        }

        //
        IEnumerable<Q> ParametricVector.IAxesBag.GetAxes<Q>() {
            foreach (var t in Axes())
                if (t is Q q) 
                    yield return q;
        }
    }

    // TODO
    // Use these attributes to control editor layouting
    public static class ParametricVector {

        public class CustomAxes : PropertyAttribute { }
        
        public class NumAxes : PropertyAttribute { 
            public int minAxes; public int maxAxes;
            public NumAxes(int numAxes) 
                => minAxes = (maxAxes = numAxes);
            public NumAxes(int minAxes, int maxAxes) 
                { this.minAxes = minAxes; this.maxAxes = maxAxes; } 
        }

        // Property or field at path should implement IAxesBag interface
        // Only works inside "SerializeReference" values and Unity Objects
        public class AxesFromProperty : PropertyAttribute {
            public string path;
            public AxesFromProperty(string path) => this.path = path;
        }

        // Context must be provided by AxesFromProperty/AxesFromContext
        // attribute on owner
        // Context taken in layouting time so it will create different contexts
        // if same property is linked at several propertyPaths
        public class AxesFromContext : PropertyAttribute {
            public string key;
        }


        public interface IAxesBag {
            IEnumerable<T> GetAxes<T>();
        }

        public class SimpleAxesBag<T> : IAxesBag {
            public List<T> axes = new();

            IEnumerable<Q> IAxesBag.GetAxes<Q>() {
                foreach (var t in axes)
                    if (t is Q q) 
                        yield return q;
            }
        }
    }
}
