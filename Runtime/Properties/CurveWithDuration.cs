using UnityEngine;

namespace Jenga {

    [System.Serializable]
    public class CurveWithDuration {

        public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        public float duration = .2f;

        public CurveWithDuration(float duration) {
            curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            this.duration = duration;
        }

        public CurveWithDuration() {
            curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            this.duration = .2f;
        }

        public Coroutine Tween(
            MonoBehaviour master, System.Action<float> action
        ) => Jenga.Tween.AlongCurve(master, action, curve, duration);
    }

}