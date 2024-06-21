using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Jenga {
    public static class AQRY  {

        public static T[] Array<T>(params T[] array) => array;
        public static ArraySegment<T> Segment<T>(params T[] array) => array;
        // public static List<T> List(params T[] array) => array.ToList();

        // Min, Max, Sum
        public static T MinBy<T>(
            ArraySegment<T> array, System.Func<T, float> by
        ) {
            if (array.Count == 0) return default(T);
            T minT = array[0];
            float min = by(minT);
            for (int i = 1; i < array.Count; ++i) {
                var item = array[i];
                var value = by(item);
                if (value < min) { minT = item; min = value; }
            }
            return minT;
        }

        public static T MaxBy<T>(
            ArraySegment<T> array, System.Func<T, float> by
        ) {
            if (array.Count == 0) return default(T);
            T maxT = array[0];
            float max = by(maxT);
            for (int i = 1; i < array.Count; ++i) {
                var item = array[i];
                var value = by(item);
                if (value > max) { maxT = item; max = value; }
            }
            return maxT;
        }


        public static float Min(ArraySegment<float> fs) {
            if (fs.Count == 0) return 0f;
            var min = fs[0];
            foreach (var f in fs) if (f < min) min = f;
            return min;
        }

        public static float Max(ArraySegment<float> fs) {
            if (fs.Count == 0) return 0f;
            var max = fs[0];
            foreach (var f in fs) if (f > max) max = f;
            return max;
        }

        public static float Sum(ArraySegment<float> fs) {
            var sum = 0f;
            foreach (var f in fs) sum += f;
            return sum;
        }

        public static bool Contains<T>(ArraySegment<T> array, T x) {
            foreach (var y in array) if (y.Equals(x)) return true;
            return false;  
        }

        public static bool Contains<T>(
            ArraySegment<T> array, System.Func<T, int, bool> condition
        ) {
            for (int i = 0; i < array.Count; ++i) 
                if (condition(array[i], i)) return true;
            return false;  
        }

        public static (T item, int index) Search<T>(
            ArraySegment<T> array, System.Func<T, int, bool> condition
        ) {
            for (int i = 0; i < array.Count; ++i) 
                if (condition(array[i], i)) return (array[i], i);
            return (default(T), -1);  
        }

        public static T[] Copy<T>(ArraySegment<T> array) => array.ToArray();

        public static ArraySegment<T> Where<T>(
            ArraySegment<T> array, System.Func<T, bool> condition
        ) {
            var firstBad = 0;
            for (int i = 0; i < array.Count; ++i) 
                if (condition(array[i])) {
                    (array[firstBad], array[i]) = (array[i], array[firstBad]);
                    firstBad++;
                }
            return new ArraySegment<T>(array.Array, array.Offset, firstBad);
        }

        public static T TakeRandom<T>(RNGi rng, ArraySegment<T> array) {
            if (array.Count == 0) return default(T);
            return array[Mathx.Mod(rng(), array.Count)];
        }

        public static ArraySegment<T> Shuffle<T>(
            RNGi rng, ArraySegment<T> array
        ) {
            for (int i = 0; i < array.Count - 2; ++i) {
                int j = i + Mathx.Mod(rng(), array.Count - i);
                (array[i], array[j]) = (array[j], array[i]);
            }
            return array;
        } 

        public static ArraySegment<T> SortBy<T>(
            ArraySegment<T> array, System.Func<T, float> by
        ) {
            var arr = array.ToArray();
            System.Array.Sort(arr, (x, y) => by(x).CompareTo(by(y)));
            return arr;
        }

        public static ArraySegment<T> Where<T>(
            ArraySegment<T> array, System.Func<T, int, bool> condition
        ) {
            var firstBad = 0;
            for (int i = 0; i < array.Count; ++i) {
                if (condition(array[i], i)) {
                    (array[firstBad], array[i]) = (array[i], array[firstBad]);
                    firstBad++;
                }
            }
            return new ArraySegment<T>(array.Array, array.Offset, firstBad);
        }
    }
}
