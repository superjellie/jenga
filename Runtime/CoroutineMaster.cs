using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class CoroutineMaster : MonoBehaviour {

        public static CoroutineMaster mainDoNotInstantiate; 
        public static CoroutineMaster main => mainDoNotInstantiate != null 
            ? mainDoNotInstantiate
            : mainDoNotInstantiate = SpawnMaster();

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
