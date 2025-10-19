using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class TransformExtensions {

        public static TransformSnapshot ToSnapshot(this Transform t)
            => TransformSnapshot.FromTransform(t);
        public static void 
        FromSnapshot(this Transform t, TransformSnapshot snapshot)
            => snapshot.SetTransform(t);

    }
}
