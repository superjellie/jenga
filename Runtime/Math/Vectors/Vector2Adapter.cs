using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Jenga {

    public sealed class Vector2Adapter : VectorAdapterF<Vector2> {
        [RequiredMember] 
        public override Vector2 Zero() => Vector2.zero;
        [RequiredMember] 
        public override int Dim(Vector2 x) => 2;
        [RequiredMember] 
        public override float Get(Vector2 x, int index) => x[index];
        [RequiredMember] 
        public override void Set(ref Vector2 x, int index, float value)  
            => x[index] = value;
    }

}
