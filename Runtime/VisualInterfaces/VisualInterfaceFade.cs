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


        void OnEnable() {
            vi.onStateChange += OnStateChange;
            OnStateChange(0, vi.state, true);
        }

        void OnDisable() {
            vi.onStateChange -= OnStateChange;
        }

        Coroutine crtn = null;
        float canBeDisabledAt;

        void Update() {
            if (Time.time < canBeDisabledAt) {
                canBeDisabledAt = Mathf.Infinity;
                vi.canBeDisabled = true;
            }
        }

        void OnStateChange(int oldState, int newState, bool immediate) {

            canBeDisabledAt = Mathf.Infinity;
            if (crtn != null) CoroutineMaster.main.StopCoroutine(crtn);

            var start = canvasGroup.alpha;
            var end = stateAlpha.Get(newState);
            canvasGroup.blocksRaycasts = newState > 0;
            canvasGroup.interactable = newState > 0;

            if (immediate)
                canvasGroup.alpha = end;
            else {
                var crv = curves.Get(oldState, newState);
                crtn = crv.Tween(
                    CoroutineMaster.main, 
                    (t) => {
                        if (this != null && canvasGroup != null)
                            canvasGroup.alpha = Mathx.Lerp(start, end, t);
                    }
                );

                vi.canBeDisabled = false;
                canBeDisabledAt = Time.time + crv.duration;
            }

        }

    }
#endif
}