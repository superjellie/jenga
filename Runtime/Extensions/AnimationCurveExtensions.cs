using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class AnimationCurveExtensions {

        public static float Integrate(
            this AnimationCurve curve, float from, float to, 
            float eps = .001f
        ) => Intf.Simpson(curve.Evaluate, from, to, eps, out var error);

    }
}
