using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace Jenga {

    public sealed class Vector4Adapter : VectorAdapterf<Vector4> {
        public override Vector4 Zero() => Vector2.zero;
        public override int Dim(Vector4 x) => 4;
        public override float Get(Vector4 x, int index) => x[index];
        public override void Set(ref Vector4 x, int index, float value)  
            => x[index] = value;
    }

}
