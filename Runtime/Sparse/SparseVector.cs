using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga.Sparse {

    // Sparse vector 
    [System.Serializable]
    public partial class SparseVector<T> {
        public List<int> indices = new();
        public List<T> values = new();
    }

    public partial class SparseVector<T> 
    : IRandomReadableVector<T>, IRandomWritableVector<T> {
        public T this[int i] {
            get {
                var id = indices.BinarySearch(i);
                if (id < 0) return default(T);
                return values[id];
            } set {
                var id = indices.BinarySearch(i);
                if (id >= 0 && object.Equals(value, default(T)))
                    { indices.RemoveAt(id); values.RemoveAt(id); } 
                else if (object.Equals(value, default(T))) 
                    return;
                else if (id >= 0)
                    { indices[id] = i; values[id] = value; }
                else if (~id < indices.Count)
                    { indices.Insert(~id, i); values.Insert(~id, value); }
                else 
                    { indices.Add(i); values.Add(value); }      
            }
        }
    }

    public partial class SparseVector<T> : IIterableVector<T> {
        public IEnumerable<IndexedValue<T>> GetIndexedValues() {
            for (int i = 0; i < indices.Count; ++i)
                yield return (indices[i], values[i]);
        }
    }

    // Index vector only contains indices of values
    [System.Serializable]
    public partial class IndexVector {
        public List<int> indices = new();
    }

    public partial class IndexVector 
    : IRandomReadableVector<bool>, IRandomWritableVector<bool> {
        public bool this[int i] {
            get => indices.BinarySearch(i) >= 0;
            set {
                var index = indices.BinarySearch(i);
                if (index >= 0 && value)
                    indices.RemoveAt(index); 
                else if (!value) 
                    indices.Insert(~index, i);         
            }
        }
    }

    public partial class IndexVector : IIterableIndexVector {
        public IEnumerable<int> GetIndices() => indices;
        public IEnumerator GetEnumerator() => GetIndices().GetEnumerator();
    }

    public partial class IndexVector : IIterableVector<bool> {
        public IEnumerable<IndexedValue<bool>> GetIndexedValues() {
            foreach (var index in indices)
                yield return (index, true);
        }
    }
}
