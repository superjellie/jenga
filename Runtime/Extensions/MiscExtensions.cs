using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class MiscExtensions {
        
        public static bool TryGetComponentInParent<T>(this Component c, out T t) 
        where T : Component 
            => (t = c.GetComponentInParent<T>()) != null;
        public static bool TryGetComponentInParent<T>(this GameObject go, out T t) 
        where T : Component 
            => (t = go.GetComponentInParent<T>()) != null;


        public static float SlopeAt(this AnimationCurve curve, float t) {
            var dt = .0001f;

            var tMin = curve.keys[0].time;
            var tMax = curve.keys[^1].time;

            var t1 = Mathf.Max(tMin, t - dt);
            var t2 = Mathf.Min(tMax, t + dt);

            if (t1 == t2) return 0f;

            var y1 = curve.Evaluate(t1);
            var y2 = curve.Evaluate(t2);

            return (y2 - y1) / (t2 - t1);
        }

        public static void OrderBy<T>(this List<T> list, Measuref<T> by) 
            => list.Sort((x, y) => by(x).CompareTo(by(y)));


        public static IEnumerable<T> Reverse<T>(this IEnumerable<T> col) {
            var stack = new Stack<T>();
            
            foreach (var item in col) 
                stack.Push(item);
            
            while (stack.TryPop(out var item))
                yield return item;
        }

    }
}
