using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class MonoGeneratorAsset<T> : ScriptableObject {
        public MonoGeneratorReference<T> generator;
    }
}
