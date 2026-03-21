using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    
    // TODO: Decide what to do with it
    [System.Serializable]
    public struct ActionName {

        public TypeName type;
        public string name;

        public ActionName(System.Type type, string name) {
            this.type = type;
            this.name = name;
        }

        public bool IsValid() {
            var tp = (System.Type)type;
            if (tp == null) return false;

            var info = tp.GetMethod(name);
            if (info == null) return false;

            return true;
        }

        public void Invoke(object master) {
            if (!IsValid()) return;

            var tp = (System.Type)type;
            var info = tp.GetMethod(name);
            info.Invoke(master, new object[0]);
        }
    }
}
