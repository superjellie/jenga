using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace Jenga {

    public sealed class Vector3Adapter : VectorAdapterf<Vector3> {
        public override Vector3 Zero() => Vector2.zero;
        public override int Dim(Vector3 x) => 3;
        public override float Get(Vector3 x, int index) => x[index];
        public override void Set(ref Vector3 x, int index, float value)  
            => x[index] = value;
    }

}
