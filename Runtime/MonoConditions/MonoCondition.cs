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

    [System.Serializable, System.Obsolete]
    public struct MonoConditionReference : ALay.ILayoutMe {

        [SerializeReference]
        [FormerlySerializedAs("serializedValue")]
        public MonoCondition value;

        public bool Check() => value?.Check() ?? false;

        public static implicit operator
        MonoConditionReference(MonoCondition condition) 
            => new MonoConditionReference() { value = condition };
    }
}