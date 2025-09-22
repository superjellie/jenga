using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class CoroutineMaster : MonoBehaviour {

        static CoroutineMaster main_; 
        public static CoroutineMaster main => main_ != null 
            ? main_
            : main_ = SpawnMaster();

        public static CoroutineMaster SpawnMaster() {
            var go = new GameObject(
                "CoroutineMaster", 
                typeof(CoroutineMaster)
            );
            DontDestroyOnLoad(go);
            return go.GetComponent<CoroutineMaster>();
        }


        public static CoroutineMaster GetOnObject(GameObject go) {
            var master = go.GetComponent<CoroutineMaster>();

            if (master != null)
                return master;
                
            return go.AddComponent<CoroutineMaster>();
        }
    }
}
