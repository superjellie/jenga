using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    // Integration algorithms
    public static class Intf {

        public delegate float Funcf(float x);
        public delegate float ODEf(float x, float y);
        public delegate Vector2 ODEv2(float x, Vector2 y);
        public enum IntError { None = 0, WontConverge = 1, NotEnoughDepth = 2 }

        // ***************************** Integrals *****************************

        // https://en.wikipedia.org/wiki/Adaptive_Simpson%27s_method#C
        // NOT TESTED
        static float SimpsonInternal(
            Funcf f, float a, float b, float eps,
            float whole, float fa, float fb, float fm, 
            int maxDepth, ref IntError error
        ) {
            var m  = .5f * (a + b);  var h   = .5f * (b - a);
            var lm = .5f * (a + m);  var rm  = .5f * (m + b);

            // serious numerical trouble: it won't converge
            if (.5f * eps == eps || a == lm) { 
                error = IntError.WontConverge; 
                return whole; 
            }

            var flm = f(lm); var frm = f(rm);
            var left  = (h / 6f) * (fa + 4f * flm + fm);
            var right = (h / 6f) * (fm + 4f * frm + fb);
            var delta = left + right - whole;

            // depth limit too shallow
            if (maxDepth <= 0 && error != IntError.WontConverge) 
                error = IntError.NotEnoughDepth;

            // Lyness 1969 + Richardson extrapolation; see article
            if (maxDepth <= 0 || Mathf.Abs(delta) <= 15f * eps)
                return left + right + delta / 15f;

            return SimpsonInternal(
                f, a, m, .5f * eps, left, fa, fm, flm, maxDepth - 1, ref error) 
            + SimpsonInternal(
                f, m, b, .5f * eps, right, fm, fb, frm, maxDepth - 1, ref error);
        }

        public static float Simpson(
            Funcf f, float a, float b, float eps, 
            out IntError error, int maxDepth = 8
        ) {
            error = IntError.None;

            var h = b - a;
            if (h == 0f) return 0f;

            var fa = f(a); var fb = f(b); var fm = f(.5f * (a + b));
            var whole = (h / 6f) * (fa + 4f * fm + fb);
            return SimpsonInternal(
                f, a, b, eps, whole, fa, fb, fm, maxDepth, ref error
            );
        }

        public static float Simpson(
            Funcf f, float a, float b, float eps, int maxDepth = 8
        ) => Simpson(f, a, b, eps, out var _, maxDepth: maxDepth);

        // ************************* Equations *********************************
        public static float ApproximateDerivative(Funcf f, float x)
            => (f(x + .01f) - f(x - .01f)) / .01f;

        // Solves f(x) = 0 with given initial guess and iterations
        public static float Newton(Funcf f, float x0, int iterations) {
            var x = x0;
            for (int i = 0; i < iterations; ++i) {
                var df = ApproximateDerivative(f, x);
                x = x - f(x) / df;
            }
            return x;
        } 

        // ****************************** ODEs *********************************

        // https://en.wikipedia.org/wiki/Bogacki%E2%80%93Shampine_method
        // should work :)
        static void BogShInternal(
            ODEf f, float x0, float y0, float z0, float dx,
            out float y1, out float z1, ref IntError error,
            float eps, int maxDepth
        ) {

            if (eps == .5f * eps)
                error = IntError.WontConverge;

            var k1 = dx * f(x0, y0);
            var k2 = dx * f(x0 +  .5f * dx, y0 +  .5f * k1);
            var k3 = dx * f(x0 + .75f * dx, y0 + .75f * k2);

            // third order approx
            y1 = y0 + 2f/9f * k1 + 1f/3f * k2 + 4f/9f * k3;

            var k4 = dx * f(x0 + dx, y1);

            // second order approx
            z1 = z0 + 7f/24f * k1 + .25f * k2 + 1f/3f * k3 + .125f * k4;

            var err = Mathf.Abs(y1 - z1);

            if (err < eps)
                return;

            if (maxDepth > 0) {
                BogShInternal(
                    f, x0, y0, z0, .5f * dx, out var ym, out var zm, ref error,
                    .5f * eps, maxDepth - 1);
                BogShInternal(
                    f, x0 + .5f * dx, ym, zm, .5f * dx, out y1, out z1, ref error,
                    .5f * eps, maxDepth - 1);
            } else if (error != IntError.WontConverge)
                error = IntError.NotEnoughDepth;
        }

        public static float BogSh(
            ODEf f, float x0, float y0, float dx,
            out IntError error,
            float eps, int maxDepth = 8
        ) {
            error = IntError.None;
            BogShInternal(
                f, x0, y0, y0, dx, 
                out var y1, out var _, ref error, eps, maxDepth
            );
            return y1;
        }

        public static float BogSh(
            ODEf f, float x0, float y0, float dx,
            float eps, int maxDepth = 8
        ) => BogSh(f, x0, y0, dx, out var _, eps, maxDepth);

        static void BogSh2Internal(
            ODEv2 f, float x0, Vector2 y0, Vector2 z0, float dx,
            out Vector2 y1, out Vector2 z1, ref IntError error,
            float eps, int maxDepth
        ) {

            if (eps == .5f * eps)
                error = IntError.WontConverge;

            var k1 = dx * f(x0, y0);
            var k2 = dx * f(x0 +  .5f * dx, y0 +  .5f * k1 * dx);
            var k3 = dx * f(x0 + .75f * dx, y0 + .75f * k2 * dx);
            var k4 = dx * f(x0 + dx, y0 + k3);

            // third order approx
            y1 = y0 + (2f/9f * k1 + 1f/3f * k2 + 4f/9f * k3) * dx;

            // second order approx
            z1 = z0 + (7f/24f * k1 + .25f * k2 + 1f/3f * k3 + .125f * k4) * dx;

            var err = (y1 - z1).magnitude;
            // Debug.Log($"err: {err}, eps: {eps}, depth: {8 - maxDepth}");

            if (err < eps)
                return;

            if (maxDepth > 0) {
                BogSh2Internal(
                    f, x0, y0, z0, .5f * dx, out var ym, out var zm, ref error,
                    .5f * eps, maxDepth - 1);
                BogSh2Internal(
                    f, x0 + .5f * dx, ym, zm, .5f * dx, out y1, out z1, ref error,
                    .5f * eps, maxDepth - 1);
            } else if (error != IntError.WontConverge)
                error = IntError.NotEnoughDepth;
        }

        public static Vector2 BogSh2(
            ODEv2 f, float x0, Vector2 y0, float dx,
            out IntError error,
            float eps, int maxDepth = 8
        ) {
            error = IntError.None;
            BogSh2Internal(
                f, x0, y0, y0, dx, 
                out var y1, out var _, ref error, eps, maxDepth
            );
            return y1;
        }

        public static Vector2 BogSh2(
            ODEv2 f, float x0, Vector2 y0, float dx,
            float eps, int maxDepth = 8
        ) => BogSh2(f, x0, y0, dx, out var _, eps, maxDepth);

    }
}