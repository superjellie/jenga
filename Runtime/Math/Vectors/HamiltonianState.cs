using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Jenga {
    
    [System.Serializable]
    public struct HamiltonianState<T> {
        public T q; public T p;

        public void Deconstruct(out T q, out T p)
            { q = this.q; p = this.p; }
        public static implicit operator HamiltonianState<T>((T, T) s)
            => new() { q = s.Item1, p = s.Item2 };        
    }

    public class HamiltonianStateAdapter<T> 
    : VectorAdapterF<HamiltonianState<T>> {
        [RequiredMember] 
        public override HamiltonianState<T> Zero() 
            => (LinAlgf.Zero<T>(), LinAlgf.Zero<T>());
        [RequiredMember] 
        public override int Dim(HamiltonianState<T> x) 
            => LinAlgf.Dim(x.q) + LinAlgf.Dim(x.p);
        [RequiredMember] 
        public override float Get(HamiltonianState<T> x, int index) 
            => index < LinAlgf.Dim(x.q) 
                ? LinAlgf.Get(x.q, index) 
                : LinAlgf.Get(x.p, index - LinAlgf.Dim(x.q));
        [RequiredMember] 
        public override void 
        Set(ref HamiltonianState<T> x, int index, float value) { 
            if (index < LinAlgf.Dim(x.q))
                LinAlgf.Set(ref x.q, index, value);
            else 
                LinAlgf.Set(ref x.p, index - LinAlgf.Dim(x.q), value);
        }
    }

    public class HamiltonianState2DAdapter : HamiltonianStateAdapter<Vector2> 
        { }

    public class HamiltonianState3DAdapter : HamiltonianStateAdapter<Vector3> 
        { }

}
