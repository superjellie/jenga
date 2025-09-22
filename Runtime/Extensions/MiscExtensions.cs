using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class MiscExtensions {

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
