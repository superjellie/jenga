using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class LinAlgf {


        // Lifting from adapter
        public static T Zero<T>() 
            => VectorAdapterF<T>.main.Zero();
        public static int Dim<T>(T x) 
            => VectorAdapterF<T>.main.Dim(x);
        public static float Get<T>(T x, int i) 
            => VectorAdapterF<T>.main.Get(x, i);
        public static void Set<T>(ref T x, int i, float value) 
            => VectorAdapterF<T>.main.Set(ref x, i, value);

        // w = a * x + b * y
        // Functions with W prefix are in-place
        public static void WAXPBY<T>(ref T w, float a, T x, float b, T y) {
            var n = Mathx.Min(Dim(x), Dim(y));

            for (int i = 0; i < n; ++i)
                Set(ref w, i, a * Get(x, i) + b * Get(y, i));
        } 

        // Scalar product
        public static float Dot<T>(T x, T y) {
            var n = Mathx.Min(Dim(x), Dim(y));
            var dot = 0f;

            for (int i = 0; i < n; ++i)
                dot += Get(x, i) * Get(y, i);

            return dot;
        }

        // SqrMagnitude(x) = x[1]^2 + x[2]^2 + ... + x[n]^2
        public static float SqrMagnitude<T>(T x) {
            var s = 0f; var n = Dim(x);
            for (int i = 0; i < n; ++i) {
                var xi = Get(x, i);
                s += xi * xi;
            }
            return s;
        }
        public static float Magnitude<T>(T x) 
            => Mathf.Sqrt(SqrMagnitude(x));
        public static float Distance<T>(T x, T y) 
            => Magnitude(Sub(x, y));


        // w = arg[0] + arg[1] + ... + arg[k] 
        public static void WAdd<T>(ref T w, params T[] args)
            { foreach (var arg in args) WAXPBY(ref w, 1f, w, 1f, arg); }


        // Generating versions
        public static T AXPBY<T>(float a, T x, float b, T y) 
            { var w = Zero<T>(); WAXPBY(ref w, a, x, b, y); return w; }
        public static T Add<T>(params T[] args)
            { var w = Zero<T>(); WAdd(ref w, args); return w; } 

        // In-Place shorthands
        public static void WAXPY<T>(ref T w, float a, T x, T y) 
            => WAXPBY(ref w, a, x, 1f, y);
        public static void WScale<T>(ref T w, float a) 
            => WAXPBY(ref w, a, w, 1f, Zero<T>());
        public static void WRScale<T>(ref T w, float a) 
            => WAXPBY(ref w, 1f / a, w, 1f, Zero<T>());
        public static void WSub<T>(ref T w, T x, T y)
            => WAXPBY(ref w, 1f, x , -1f, y);
        public static void WCopy<T>(ref T w, T x)
            => WAXPBY(ref w, 1f, x , 0f, Zero<T>());

        // Generating shorthands
        public static T AXPY<T>(float a, T x, T y) 
            => AXPBY(a, x, 1f, y);
        public static T Scale<T>(T x, float a) 
            => AXPBY(a, x, 1f, Zero<T>());
        public static T RScale<T>(T x, float a) 
            => AXPBY(1f / a, x, 1f, Zero<T>());
        public static T Sub<T>(T x, T y) 
            => AXPBY(1f, x, -1f, y);
        public static T Copy<T>(T x)
            => AXPBY(1f, x , 0f, Zero<T>());

    }
}
