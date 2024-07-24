using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Jenga {

    // AQRY functions DO NOT make copies if they can help it
    //      that means that they usually change array they work on
    // Intended to replace LINQ cause it is inconvinient and baggy sometimes
    // In future AQRY will also be optimised better i hope 
    public static class AQRY {

        public delegate bool Filter<T>(T x, int i); 
        public delegate float Measure<T>(T x, int i); 
        public delegate Q Transformer<T, Q>(T x, int i); 

        // Min, Max, Sum
        public static T MinBy<T>(ArrayView<T> array, Measure<T> by) {
            if (array.length <= 0) return default(T);
            T minT = array[0];
            float min = by(minT, 0);
            for (int i = 1; i < array.length; ++i) {
                var item = array[i];
                var value = by(item, i);
                if (value < min) { minT = item; min = value; }
            }
            return minT;
        }

        public static T MaxBy<T>(ArrayView<T> array, Measure<T> by) {
            if (array.length <= 0) return default(T);
            T maxT = array[0];
            float max = by(maxT, 0);
            for (int i = 1; i < array.length; ++i) {
                var item = array[i];
                var value = by(item, i);
                if (value > max) { maxT = item; max = value; }
            }
            return maxT;
        }


        public static float Min(ArrayView<float> array) 
            => MinBy(array, (x, i) => x);

        public static float Max(ArrayView<float> array) 
            => MaxBy(array, (x, i) => x);

        public static float Sum(ArrayView<float> fs) {
            var sum = 0f;
            for (int i = 0; i < fs.length; ++i) 
                sum += fs[i];
            return sum;
        }

        public static bool Contains<T>(ArrayView<T> array, Filter<T> filter) {
            for (int i = 0; i < array.length; ++i) 
                if (filter(array[i], i)) return true;
            return false;  
        }

        public static (T item, int index) Search<T>(
            ArrayView<T> array, Filter<T> filter
        ) {
            for (int i = 0; i < array.length; ++i) 
                if (filter(array[i], i)) return (array[i], i);
            return (default(T), -1);  
        }

        public static ArrayView<T> Where<T>(
            ArrayView<T> array, Filter<T> filter
        ) {
            var firstBad = 0;
            for (int i = 0; i < array.length; ++i) 
                if (filter(array[i], i)) {
                    (array[firstBad], array[i]) = (array[i], array[firstBad]);
                    firstBad++;
                }
            return ArrayView.Slice(array, 0, firstBad);
        }

        public static Q[] Transform<T, Q>(
            ArrayView<T> array, Transformer<T, Q> transformer
        ) {
            var qArray = new Q[array.length];
            for (int i = 0; i < array.length; ++i) 
                qArray[i] = transformer(array[i], i);
            
            return qArray;
        }

        public static ArrayView<T> Select<T>(
            ArrayView<T> source, ArrayView<T> destination, Filter<T> filter
        ) {
            int j = 0;
            for (int i = 0; i < source.length && j < destination.length; ++i)
                if (filter(source[i], i)) destination[j++] = source[i];
            return ArrayView.Slice(destination, 0, j);
        }

        public static T TakeRandom<T>(RNGi rng, ArrayView<T> array) {
            if (array.length <= 0) return default(T);
            return array[Mathx.Mod(rng(), array.length)];
        }

        public static ArrayView<T> Shuffle<T>(RNGi rng, ArrayView<T> array) {
            for (int i = 0; i < array.length - 2; ++i) {
                int j = i + Mathx.Mod(rng(), array.length - i);
                (array[i], array[j]) = (array[j], array[i]);
            }
            return array;
        } 

        public static ArrayView<T> SortBy<T>(
            ArrayView<T> array, Measure<T> by
        ) {
            for (int i = 0; i < array.length; ++i) {
                var minIndex = i;
                var minValue = by(array[i], i);
                for (int j = i + 1; j < array.length; ++j) {
                    var value = by(array[j], j);
                    if (value < minValue) {
                        minValue = value;
                        minIndex = j;
                    }
                }

                array.Swap(i, minIndex);
            }
            return array;
        }
    }
}
