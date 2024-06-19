using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class Tween {
        public static Coroutine AlongCurve(
            MonoBehaviour master, System.Action<float> tweener,
            AnimationCurve curve, float duration
        ) {
            IEnumerator Play() {
                var start = Time.time;
                var end = start + duration;

                for (float t = start; t < end; t = Time.time) {
                    float g = curve.Evaluate((t - start) / duration);
                    tweener(g);
                    yield return new WaitForFixedUpdate();
                }

                tweener(curve.Evaluate(1f));
            }

            return master.StartCoroutine(Play());
        }
    }
}
