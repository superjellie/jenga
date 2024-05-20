using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class Anim {

        public static Coroutine MoveTo(
            MonoBehaviour actor, Vector3 position, float speed
        ) {
            IEnumerator MoveToCoroutine() {
                var start = actor.transform.position; 
                var end   = position; 
                var distance = (end - start).magnitude;
                var startT = Time.time;
                var totalT = distance / speed;
                for (var t = 0f; t < 1f; t = (Time.time - startT) / totalT) {
                    actor.transform.position = Mathj.Lerp(start, end, t);
                    yield return new WaitForFixedUpdate();
                }
            } 

            return actor.StartCoroutine(MoveToCoroutine());
        }

    }
}
