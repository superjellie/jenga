using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class AQRY  {

        // Min, Max, Sum
        public static T MinBy<T>(T[] array, System.Func<T, float> by) {
            if (array.Length == 0) return default(T);
            T minT = array[0];
            float min = by(minT);
            for (int i = 1; i < array.Length; ++i) {
                var item = array[i];
                var value = by(item);
                if (value < min) { minT = item; min = value; }
            }
            return minT;
        }

        public static T MaxBy<T>(T[] array, System.Func<T, float> by) {
            if (array.Length == 0) return default(T);
            T maxT = array[0];
            float max = by(maxT);
            for (int i = 1; i < array.Length; ++i) {
                var item = array[i];
                var value = by(item);
                if (value > max) { maxT = item; max = value; }
            }
            return maxT;
        }


        public static float Min(params float[] fs) {
            if (fs.Length == 0) return 0f;
            var min = fs[0];
            foreach (var f in fs) if (f < min) min = f;
            return min;
        }

        public static float Max(params float[] fs) {
            if (fs.Length == 0) return 0f;
            var max = fs[0];
            foreach (var f in fs) if (f > max) max = f;
            return max;
        }

        public static float Sum(params float[] fs) {
            var sum = 0f;
            foreach (var f in fs) sum += f;
            return sum;
        }

        public static bool Contains<T>(T[] array, T x) 
            => System.Array.Exists(array, y => System.Object.Equals(x, y));
    }
}
