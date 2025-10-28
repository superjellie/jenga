using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;

namespace Jenga {

    public sealed class Vector2Adapter : VectorAdapterf<Vector2> {
        public override Vector2 Zero() => Vector2.zero;
        public override int Dim(Vector2 x) => 2;
        public override float Get(Vector2 x, int index) => x[index];
        public override void Set(ref Vector2 x, int index, float value)  
            => x[index] = value;
    }

}
