using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class Mathj {
        
        public static float Lerp(float a, float b, float t) 
            => a * (1f - t) + b * t;
        public static Vector2 Lerp(Vector2 a, Vector2 b, float t) 
            => a * (1f - t) + b * t;
        public static Vector3 Lerp(Vector3 a, Vector3 b, float t) 
            => a * (1f - t) + b * t;
        public static Quaternion Slerp(Quaternion a, Quaternion b, float t) 
            => Quaternion.Slerp(a, b, t);

    }
}
