using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Jenga {
    public static class MiscExtensions {

        public static bool TryGetComponentInParent<T>(this Component c, out T t)
        where T : Component
            => (t = c.GetComponentInParent<T>()) != null;
        public static bool TryGetComponentInParent<T>(this GameObject go, out T t)
        where T : Component
            => (t = go.GetComponentInParent<T>()) != null;

        public static bool TryGetComponentInChildren<T>(this Component c, out T t) 
        where T : Component 
            => (t = c.GetComponentInChildren<T>()) != null;
        public static bool TryGetComponentInChildren<T>(this GameObject go, out T t) 
        where T : Component 
            => (t = go.GetComponentInChildren<T>()) != null;

        public static IEnumerable<T> 
        GetComponentsInChildrenDirect<T>(this Component c) 
        where T : Component {
            var stack = GenericPool<Stack<Transform>>.Get();
            // var stack = new Stack<T>();
            stack.Clear();
            stack.Push(c.transform);

            while (stack.Count > 0) {
                var item = stack.Pop();

                if (item != c.transform && item.TryGetComponent<T>(out var t)) {
                    yield return t;
                    continue;
                }

                for (int i = 0; i < item.childCount; ++i) 
                    stack.Push(item.GetChild(i));                
            }

            GenericPool<Stack<Transform>>.Release(stack);
        }

        public static IEnumerable<T> 
        GetComponentsInChildrenDirect<T>(this GameObject go) 
        where T : Component 
            => go.transform.GetComponentsInChildrenDirect<T>();


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


        public static bool Contains<T>(this T[] list, T x)
            => System.Array.IndexOf(list, x) != -1;


        public static IEnumerable<T> Reverse<T>(this IEnumerable<T> col) {
            var stack = new Stack<T>();

            foreach (var item in col)
                stack.Push(item);

            while (stack.TryPop(out var item))
                yield return item;
        }

        public static HashSet<T> Copy<T>(this HashSet<T> set)
            => new(set);
        public static T[] Copy<T>(this T[] array) {
            var dest = new T[array.Length];
            array.CopyTo(dest, 0);
            return dest;
        }

        public static T[] ConvertToArray<T>(this IEnumerable<T> collection) {
            var list = new List<T>();
            foreach (var item in collection)
                list.Add(item);
            return list.ToArray();
        }


    }
}
