using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    // Serializable generic condition
    // Check out WhenEnemy.cs for tutorial on writing custom conditions
    [System.Serializable, ALay.HideHeader]
    public class MonoCondition : ALay.ILayoutMe {
        
        // Used to match together references
        [HideInInspector] public string refName;

        public virtual bool Check() => true;
    }

    // Use this class to use MonoCondition as field 
    // (Hides [SerializeReference] and some ui drawing)
    [System.Serializable]
    [ALay.TypeSelector(typeof(MonoCondition), path = "value")]
    [ALay.MatchReferences]
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