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


        public static Vector3 GetRootPosition(this Transform t)
            => t.root.InverseTransformPoint(t.position);

        public static Vector3 SetRootPosition(this Transform t, Vector3 pos)
            => t.position = t.root.TransformPoint(pos);

    }
}
