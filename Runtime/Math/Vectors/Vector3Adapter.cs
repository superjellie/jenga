using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Jenga {

    public sealed class Vector3Adapter : VectorAdapterF<Vector3> {
        [RequiredMember] 
        public override Vector3 Zero() => Vector2.zero;
        [RequiredMember] 
        public override int Dim(Vector3 x) => 3;
        [RequiredMember] 
        public override float Get(Vector3 x, int index) => x[index];
        [RequiredMember] 
        public override void Set(ref Vector3 x, int index, float value)  
            => x[index] = value;
    }

}
