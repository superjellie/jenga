using UnityEngine;
using Jenga;

[System.Serializable]
public struct CurveWithDuration {
    public AnimationCurve curve;
    public float duration;

    public CurveWithDuration(float duration) {
        curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        this.duration = duration;
    }

    public Coroutine Tween(
        MonoBehaviour master, System.Action<float> action
    ) => Jenga.Tween.AlongCurve(master, action, curve, duration);
}
