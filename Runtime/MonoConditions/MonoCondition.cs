using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    // Serializable generic condition
    // Check out WhenEnemy.cs for tutorial on writing custom conditions
    [System.Serializable]
    public class MonoCondition : ALay.ILayoutMe {

        // GO is object witch initiated the Check
        // Ignore it if you want global condition
        public virtual bool Check(GameObject go) => true;
    }

    // Use this class to use MonoCondition as field 
    // (Hides [SerializeReference] and some ui drawing)
    [System.Serializable]
    [ALay.TypeSelector(typeof(MonoCondition), path = "serializedValue")]
    public struct MonoConditionReference : ALay.ILayoutMe {

        [SerializeReference]
        [ALay.HideHeader]
        public MonoCondition serializedValue;

        public bool Check(GameObject go) => serializedValue?.Check(go) ?? false;

        public static implicit operator
        MonoConditionReference(MonoCondition condition) 
            => new MonoConditionReference() { serializedValue = condition };
    }
}