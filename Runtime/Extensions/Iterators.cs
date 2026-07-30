using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class Iterators {

        public static IEnumerable<T> Repeat<T>(int times, T value) {
            for (int i = 0; i < times; ++i)
                yield return value;
        }

        public static IEnumerable<T> 
        Generate<T>(int times, System.Func<T> generator) {
            for (int i = 0; i < times; ++i)
                yield return generator();
        }

        public static IEnumerable<T> Empty<T>() { yield break; }
    }
}
