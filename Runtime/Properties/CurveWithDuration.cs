using UnityEngine;

namespace Jenga {

    [System.Serializable, ALay.Inline]
    public class CurveWithDuration : ALay.ILayoutMe {

        [ALay.Style(flexGrow = 1f)] 
        public AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        
        [ALay.Style(width = 50f)]
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