using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class CoroutineExtensions {

        public static Coroutine 
        AndThen(this Coroutine crtn, System.Action toDo) {
            IEnumerator Play() {
                yield return crtn;
                toDo();
            }

            return CoroutineMaster.main.StartCoroutine(Play());
        }
    
    }
}
