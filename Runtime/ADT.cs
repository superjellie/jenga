using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class ADT {

        [System.Serializable]
        public class Map<T, Q> : ISerializationCallbackReceiver {

            [SerializeField] private List<T> keys = new List<T>(); 
            [SerializeField] private List<Q> values = new List<Q>();

            private Dictionary<T, Q> dict = new Dictionary<T, Q>();

            public void OnBeforeSerialize() {
                this.keys.Clear();
                this.values.Clear();
                foreach (var (key, value) in this.dict) {
                    this.keys.Add(key);
                    this.values.Add(value);
                    // Debug.Log($"Added {key}, {value}");
                }
            }

            public void OnAfterDeserialize() {
                this.dict.Clear();
                for (int i = 0; i < this.keys.Count; ++i)
                    this.dict[this.keys[i]] = 
                        i < this.values.Count ? this.values[i] : default(Q);
            }

            public Q this[T x] {
                get => dict.ContainsKey(x) ? dict[x] : default(Q); 
                set => dict[x] = value;
            }

            public bool ContainsKey(T key) => dict.ContainsKey(key);
            public bool ContainsValue(Q value) => dict.ContainsValue(value);
            public void Clear() => dict.Clear();

            public int Count() => dict.Count;
        }

    }
}
