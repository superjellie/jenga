using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    // Serializable generic condition
    // Check out WhenEnemy.cs for tutorial on writing custom conditions
    [System.Serializable, ALay.HideHeader]
    public class MonoCondition : ALay.ILayoutMe {

        // GO is object witch initiated the Check
        // Ignore it if you want global condition
        public virtual bool Check(GameObject go) => true;
    }

    // Use this class to use MonoCondition as field 
    // (Hides [SerializeReference] and some ui drawing)
    [System.Serializable]
    [ALay.TypeSelector(typeof(MonoCondition), path = "value")]
    public struct MonoConditionReference : ALay.ILayoutMe {

        [SerializeReference]
        [FormerlySerializedAs("serializedValue")]
        public MonoCondition value;

        public bool Check(GameObject go) => value?.Check(go) ?? false;

        public static implicit operator
        MonoConditionReference(MonoCondition condition) 
            => new MonoConditionReference() { value = condition };
    }
}