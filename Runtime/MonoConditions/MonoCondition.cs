using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// #if USE_VISUAL_SCRIPTING
// using Unity.VisualScripting;
// #endif
using UnityEngine.Serialization;

namespace Jenga {

    [System.Serializable]
    [AddTypeMenu("Jenga.MonoCondition")]
    public class MonoCondition {
        public virtual bool Check() => true;
    }
}