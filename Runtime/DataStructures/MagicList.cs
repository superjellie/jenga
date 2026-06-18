using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // Magic container that: 
    // - has stable ids
    // - has O(1) access by id
    // - has O(1) remove by id
    // - has O(1*) add (amortized by need to reallocate)
    // - has same iteration behaviour as List<T>
    // - is serializable
    // This container does not ever shrink and has considerable overhead
    //
    // TODO:
    // - Testing
    // - Small-size optimization?
    [System.Serializable]
    public class MagicList<T> : IEnumerable<T> {
        [SerializeField] List<int> index2id = new();
        [SerializeField] List<int> id2index = new();
        [SerializeField] List<T> items = new();

        public int Count()
            => items.Count;

        public int Capacity()
            => items.Capacity;

        // public int EnsureCapacity(int capacity) 
        //     => Mathx.Min(
        //         index2id.EnsureCapacity(capacity), 
        //         id2index.EnsureCapacity(capacity), 
        //         items.EnsureCapacity(capacity)
        //     );

        public bool HasItemAt(int id) 
            => id < id2index.Count && id >= 0 
            && id2index[id] < items.Count;

        // Ensure id is present
        public T this[int id] {
            get => items[id2index[id]];
            set => items[id2index[id]] = value;
        }

        // Add item and returns assigned id
        public int Add(T item) {
            // index of new item
            var index = items.Count;

            // Add item
            items.Add(item);

            // Has free ids
            if (index < index2id.Count) {
                return index2id[index];
            }

            // Need allocate new id
            index2id.Add(index);
            id2index.Add(index);
            return index;
        }

        // Removes item by its ID and frees this ID
        public void RemoveAt(int id) {
            if (!HasItemAt(id)) return;

            var lastIndex = items.Count - 1;
            var index = id2index[id];
            var lastId = index2id[lastIndex];

            // Swap index and lastIndex 
            items.Swap(index, lastIndex);

            // Update id table
            id2index[id] = lastIndex;
            id2index[lastId] = index;
            index2id[index] = lastId;
            index2id[lastIndex] = id;

            // Now we can remove item in O(1)
            items.RemoveAt(lastIndex);
        }

        // Iterates over all items
        public IEnumerator<T> GetEnumerator() => items.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => items.GetEnumerator();
        public IEnumerable<(int id, T item)> GetItemIDPairs() {
            for (var index = 0; index < items.Count; ++index)
                yield return (index2id[index], items[index]);
        }

        // 
        public bool TryGetValue(int id, out T value) {
            if (HasItemAt(id)) { value = this[id]; return true; }
            value = default(T); return false;
        }

    }
}
