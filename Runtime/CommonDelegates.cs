using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public delegate bool  Predicate<T>(T x);
    public delegate float Measuref<T>(T x);
    public delegate float Funcf(float x);
    public delegate float Hamiltonian<T>(float t, T q, T p);
    public delegate T ODE<T>(float x, T y);
}
