using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Jenga {

    public sealed class Vector4Adapter : VectorAdapterF<Vector4> {
        [RequiredMember] 
        public override Vector4 Zero() => Vector2.zero;
        [RequiredMember] 
        public override int Dim(Vector4 x) => 4;
        [RequiredMember] 
        public override float Get(Vector4 x, int index) => x[index];
        [RequiredMember] 
        public override void Set(ref Vector4 x, int index, float value)  
            => x[index] = value;
    }

}
