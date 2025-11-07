using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    // Integration algorithms
    public static class Intf {

        // public delegate float ODEf(float x, float y);

        public enum IntError { 
            None = 0, WontConverge = 1, NotEnoughDepth = 2,
            VectorAdapterNotImplemented = 3
        }

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

        // Solves f(x) = 0 with given initial guess and iterations
        public static float Newton(Funcf f, float x0, int iterations) {
            var x = x0;
            for (int i = 0; i < iterations; ++i) {
                var df = Derf.Derivative(f, x);
                x = x - f(x) / df;
            }
            return x;
        } 

        // ****************************** ODEs *********************************

        // https://en.wikipedia.org/wiki/Bogacki%E2%80%93Shampine_method
        static void BogShInternal<T>(
            ODE<T> f, float x0, T y0, T z0, float dx,
            out T y1, out T z1, ref IntError error,
            float eps, int maxDepth
        ) {

            if (eps == .5f * eps)
                error = IntError.WontConverge;

            var k1 = LinAlgf.Scale(f(x0, y0), dx);

            var x2 = x0 +  .5f * dx; 
            var y2 = LinAlgf.AXPY(.5f, k1, y0);
            var k2 = LinAlgf.Scale(f(x2, y2), dx);

            var x3 = x0 + .75f * dx; 
            var y3 = LinAlgf.AXPY(.75f, k2, y0);
            var k3 = LinAlgf.Scale(f(x3, y3), dx);

            // third order approx
            y1 = LinAlgf.Add(
                y0,
                LinAlgf.Scale(k1, 2f/9f), 
                LinAlgf.Scale(k2, 1f/3f), 
                LinAlgf.Scale(k3, 4f/9f)
            );

            var x4 = x0 + dx; 
            var k4 = LinAlgf.Scale(f(x4, y1), dx);

            // second order approx
            z1 = LinAlgf.Add(
                z0,
                LinAlgf.Scale(k1, 7f/24f), 
                LinAlgf.Scale(k2, .25f), 
                LinAlgf.Scale(k3, 1f/3f),
                LinAlgf.Scale(k4, .125f)
            );

            var err = LinAlgf.Distance(y1, z1);
            // Debug.Log($"err: {err}, eps: {eps}, depth: {8 - maxDepth}");

            if (err < eps)
                return;

            if (maxDepth > 0) {
                BogShInternal(
                    f, x0, y0, z0, .5f * dx, 
                    out var ym, out var zm, ref error, .5f * eps, maxDepth - 1
                );
                BogShInternal(
                    f, x0 + .5f * dx, ym, zm, .5f * dx, 
                    out y1, out z1, ref error, .5f * eps, maxDepth - 1
                );
            } else if (error != IntError.WontConverge)
                error = IntError.NotEnoughDepth;
        }

        public static T BogSh<T>(
            ODE<T> f, float x0, T y0, float dx,
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

        public static T BogSh<T>(
            ODE<T> f, float x0, T y0, float dx,
            float eps, int maxDepth = 8
        ) => BogSh(f, x0, y0, dx, out var _, eps, maxDepth);

    }
}