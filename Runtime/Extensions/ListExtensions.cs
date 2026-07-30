using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class ListExtensions {
        public static void Swap<T>(this IList<T> list, int indexA, int indexB) {
            var item = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = item;
        }

        public static void EnsureCapacity<T>(this List<T> list, int capacity) {
            list.Capacity = Mathx.Max(list.Capacity, capacity);
        }

        public static void EnsureCount<T>(
            this List<T> list, int count, T defaultValue = default(T)
        ) {
            var itemsToAdd = count - list.Count;
            if (itemsToAdd < 0) return;
            list.EnsureCapacity(count);
            list.AddRange(Iterators.Repeat(itemsToAdd, defaultValue));
        }

        public static void EnsureCount<T>(
            this List<T> list, int count, System.Func<T> generator
        ) {
            var itemsToAdd = count - list.Count;
            if (itemsToAdd < 0) return;
            list.EnsureCapacity(count);
            list.AddRange(Iterators.Generate(itemsToAdd, generator));
        }
    }
}
