using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Jenga {

    public sealed class FloatVectorAdapter : VectorAdapterF<float> {
        [RequiredMember] 
        public override float Zero() => 0f;
        [RequiredMember] 
        public override int Dim(float x) => 1;
        [RequiredMember] 
        public override float Get(float x, int index) => x;
        [RequiredMember] 
        public override void Set(ref float x, int index, float value)  
            => x = value;
    }

}
