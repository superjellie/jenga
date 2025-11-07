using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    [AddTypeMenu("Jenga.MonoCondition/AND")]
    [System.Serializable]
    public class AndCondition : MonoCondition {

        [SerializeReference, TypeMenu, Wrapper]
        public MonoCondition[] conditions = { };

 
        public override bool Check() {
            foreach (var condition in conditions) 
                if (!condition.Check()) return false;
            return true;
        }

    }

    [AddTypeMenu("Jenga.MonoCondition/OR")]
    [System.Serializable]
    public class OrCondition : MonoCondition {

        [SerializeReference, TypeMenu, Wrapper]
        public MonoCondition[] conditions = { };


        public override bool Check() {
            foreach (var condition in conditions) 
                if (condition.Check()) return true;
            return false;
        }

    }


    [AddTypeMenu("Jenga.MonoCondition/NOT")]
    [System.Serializable]
    public class NotCondition : MonoCondition {

        [SerializeReference, TypeMenu, Wrapper]
        public MonoCondition condition = new ConstCondition();

        public override bool Check() => !condition.Check();

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