using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class Vector3Extensions {

        public static Vector2 SwizzleXZ(this Vector3 v) => new(v.x, v.z);
        public static Vector2 SwizzleXY(this Vector3 v) => new(v.x, v.y);
        public static Vector3 SwizzleX0Y(this Vector2 v) => new(v.x, 0f, v.y);
        public static Vector3 SwizzleXY0(this Vector2 v) => new(v.x, v.y, 0f);

    }
}
