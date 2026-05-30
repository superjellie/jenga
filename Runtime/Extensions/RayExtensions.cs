using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class RayExtensions {

        public static Vector3 ClosestPoint(this Ray ray, Vector3 other) {
            var y = Vector3.Project(other - ray.origin, ray.direction);
            return ray.origin + y;
        }

        public static float DistanceTo(this Ray ray, Vector3 other) {
            return (ray.ClosestPoint(other) - other).magnitude;
        }

        // public static float 
        // DistanceToSegment(this Ray ray, Vector3 v0, Vector3 v1) {
        //     var v10 = v1 - v0;

        //     //
        //     var d = ray.direction
        // }
    }
}
