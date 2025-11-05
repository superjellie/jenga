using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    [AddTypeMenu("Jenga.MonoCondition/AND")]
    [System.Serializable]
    public class AndCondition : MonoCondition, ISerializationCallbackReceiver {

        [SerializeReference, TypeMenu, Wrapper]
        public MonoCondition[] conditions = { };

        public MonoConditionReference[] items = { };
 
        public override bool Check() {
            foreach (var condition in conditions) 
                if (!condition.Check()) return false;
            return true;
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() {
            conditions = new MonoCondition[items.Length];
            for (int i = 0; i < items.Length; ++i)
                conditions[i] = items[i].value;
        }
    }

    [AddTypeMenu("Jenga.MonoCondition/OR")]
    [System.Serializable]
    public class OrCondition : MonoCondition, ISerializationCallbackReceiver {

        [SerializeReference, TypeMenu, Wrapper]
        public MonoCondition[] conditions = { };

        public MonoConditionReference[] items = { };

        public override bool Check() {
            foreach (var condition in conditions) 
                if (condition.Check()) return true;
            return false;
        }

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { 
            conditions = new MonoCondition[items.Length];
            for (int i = 0; i < items.Length; ++i)
                conditions[i] = items[i].value;
        }
    }


    [AddTypeMenu("Jenga.MonoCondition/NOT")]
    [System.Serializable]
    public class NotCondition : MonoCondition, ISerializationCallbackReceiver {

        [SerializeReference, TypeMenu, Wrapper]
        public MonoCondition condition = new ConstCondition();

        [FormerlySerializedAs("condition")]
        public MonoConditionReference item = new ConstCondition();

        public override bool Check() => !condition.Check();

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { 
            condition = item.value;
        }
    }


    [AddTypeMenu("MonoCondition/Const")]
    [System.Serializable]
    public class ConstCondition : MonoCondition {
        public enum BoolEnum { True = 1, False = 0 } 

        [HideLabel]
        public BoolEnum value = BoolEnum.True;

        public override bool Check() => value == BoolEnum.True;
    }

}