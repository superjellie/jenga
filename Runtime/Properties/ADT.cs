using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class ADT {

        [System.Serializable]
        public class Map<T, Q> 
            : ISerializationCallbackReceiver, ALay.ILayoutMe {

            [System.Serializable]
            public class Pair : ALay.ILayoutMe {
                public T key;
                public Q value;
            } 

            [ALay.ListView(showFoldoutHeader = false)]
            public List<Pair> pairs = new(); 
            
            public Dictionary<T, Q> dict = new();

            public void OnBeforeSerialize() {

                var keysSet = new HashSet<T>(dict.Keys.ToList());
                var toRemove = new List<Pair>();

                foreach (var pair in pairs)
                    if (dict.ContainsKey(pair.key)) {
                        pair.value = dict[pair.key];
                        keysSet.Remove(pair.key);
                    } else toRemove.Add(pair);

                foreach (var pair in toRemove)
                    pairs.Remove(pair);

                foreach (var key in keysSet)
                    pairs.Add(new() { key = key, value = dict[key] });
            }

            public void OnAfterDeserialize() {
                dict.Clear();
                for (int i = 0; i < pairs.Count; ++i)
                    dict[pairs[i].key] = pairs[i].value;
            }

            public Q this[T x] {
                get => dict.ContainsKey(x) ? dict[x] : default(Q); 
                set => dict[x] = value;
            }

            public bool ContainsKey(T key) => dict.ContainsKey(key);
            public bool ContainsValue(Q value) => dict.ContainsValue(value);
            public void Clear() => dict.Clear();

            public int Count => dict.Count;
            public IEnumerator<KeyValuePair<T, Q>> GetEnumerator()
                => dict.GetEnumerator();
        }

    }
}
