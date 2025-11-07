using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // Derivative approximations
    public class Derf {

        // Represents vector function
        public delegate float FuncN<T>(T x); 
        public delegate T CurveN<T>(float t); 
        public delegate T TransformN<T>(T x); 

        // Default approximation epsilon
        public const float EPS = .001f;

        public static float Derivative(Funcf f, float x, float eps = EPS)
            => (f(x + eps) - f(x - eps)) / (2f * eps);

        public static T Tangent<T>(CurveN<T> g, float t, float eps = EPS) 
            => LinAlgf.RScale(LinAlgf.Sub(g(t + eps), g(t + eps)), 2f * eps);

        public static float
        Partial<T>(FuncN<T> f, T x, int i, float eps = EPS) {
            var y = LinAlgf.Copy(x);
            Funcf fi = xi => { LinAlgf.Set(ref y, i, xi); return f(y); };
            return Derivative(fi, 0f, eps);
        }

        public static float 
        AlongVector<T>(FuncN<T> f, T x, T v, float eps = EPS) 
            => Derivative(t => f(LinAlgf.AXPY(t, v, x)), 0f, eps);

        public static T Gradient<T>(FuncN<T> f, T x, float eps = EPS) {
            var n = LinAlgf.Dim(x);
            var D = LinAlgf.Zero<T>();

            for (int i = 0; i < n; ++i)
                LinAlgf.Set(ref D, i, Partial(f, x, i, eps));

            return D;
        }
    }
}
