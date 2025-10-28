using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {


    public static partial class Mechanics {

    //     public static ODE<HamiltonianState<T>> 
    //     ToHamiltonEqs<T>(this Hamiltonian<T> H)
    //     => (t, s) => (
    //         Derf.Gradient(p => H(t, s.q, p), s.p),                              // q' = Hp 
    //         VectorAdapter<T>.main.Negate(Derf.Gradient(q => H(t, q, s.p), s.q)) // p' = -Hq
    //     );

    //     public static ODE<HamiltonianState<T>> 
    //     ToHamiltonEqs<T>(this ODE<T> f)
    //     => (t, s) => (
    //         s.p, 
    //         VectorAdapter<T>.main.Add(
    //             Derf.Tangent<T>(t1 => f(t1, s.q), t),
    //             Derf.JacobiMatrix<T>(q1 => f(t, q1), s.q)
    //                 * f(t, s.q)
    //         )
    //     );

    }

}
