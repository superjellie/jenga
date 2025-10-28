using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace Jenga {

    public sealed class FloatVectorAdapter : VectorAdapterf<float> {
        public override float Zero() => 0f;
        public override int Dim(float x) => 1;
        public override float Get(float x, int index) => x;
        public override void Set(ref float x, int index, float value)  
            => x = value;
    }

}
