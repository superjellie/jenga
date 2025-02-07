using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jenga;

namespace Jenga {

#if USE_UNITY_UI
    [RequireComponent(typeof(VisualInterface))]
    [RequireComponent(typeof(CanvasGroup))]
    public class VisualInterfaceFade : MonoBehaviour {

        public VisualInterface vi => GetComponent<VisualInterface>();
        public CanvasGroup canvasGroup => GetComponent<CanvasGroup>();

        public VisualStateData<float> stateAlpha = new() {
            matchers = new VisualStateData<float>.StateData[] 
                { new() { mask = 2, data = 1f } }
        };
        public VisualTransitionData<CurveWithDuration> curves = new() {
            fallback = new CurveWithDuration(.2f)
        };

        void Awake() {
            vi.onStateChange += OnStateChange;
        }

        void Start() {
            OnStateChange(0, vi.state, true);
        }

        Coroutine crtn = null;

        void OnStateChange(int oldState, int newState, bool immediate) {

            // Debug.Log($"{name}: {oldState} => {newState}, imm. = {immediate}");

            if (crtn != null) StopCoroutine(crtn);

            var start = canvasGroup.alpha;
            var end = stateAlpha.Get(newState);

            if (immediate)
                canvasGroup.alpha = end;
            else
                crtn = curves.Get(oldState, newState).Tween(
                    this, (t) => canvasGroup.alpha = Mathx.Lerp(start, end, t)
                );
        }

    }
#endif
}