using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    [AddTypeMenu(typeof(MonoCondition), "General/AND", 2)]
    [System.Serializable]
    public class AndCondition : MonoCondition {

        [ALay.ListView(showFoldoutHeader = false)]
        public MonoConditionReference[] items = { };
 
        public override bool Check(GameObject go) {
            foreach (var condition in items) 
                if (!condition.Check(go)) return false;
            return true;
        }
    }

    [AddTypeMenu(typeof(MonoCondition), "General/OR", 3)]
    [System.Serializable]
    public class OrCondition : MonoCondition {
        [ALay.ListView(showFoldoutHeader = false)]
        public MonoConditionReference[] items = { };

        public override bool Check(GameObject go) {
            foreach (var condition in items) 
                if (condition.Check(go)) return true;
            return false;
        }
    }


    [AddTypeMenu(typeof(MonoCondition), "General/NOT", 1)]
    [System.Serializable]
    [InlinePropertyEditor]
    public class NotCondition : MonoCondition {

        public MonoConditionReference condition = new ConstCondition();

        public override bool Check(GameObject go) => !condition.Check(go);
    }


    [AddTypeMenu(typeof(MonoCondition), "General/Const", 0)]
    [InlinePropertyEditor]
    [System.Serializable]
    public class ConstCondition : MonoCondition {
        public enum BoolEnum { True = 1, False = 0 } 

        [ALay.HideLabel]
        public BoolEnum value = BoolEnum.True;

        public override bool Check(GameObject go) => value == BoolEnum.True;
    }

}