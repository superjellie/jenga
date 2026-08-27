using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Jenga;

namespace Jenga {

#if USE_UNITY_UI
    [RequireComponent(typeof(VisualInterface))]
    public class VisualInterfaceChangeColorImage : MonoBehaviour {

        public VisualInterface vi => GetComponent<VisualInterface>();
        public Image image;

        public VisualStateData<Color> stateColor = new();
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

        void OnStateChange(int oldState, int newState, bool immediate) {

            // Debug.Log($"{name}: {oldState} => {newState}, imm. = {immediate}");

            if (crtn != null) StopCoroutine(crtn);

            var start = image.color;
            var end = stateColor.Get(newState);

            if (immediate)
                image.color = end;
            else
                crtn = curves.Get(oldState, newState).Tween(
                    this, (t) => image.color = Color.Lerp(start, end, t)
                );
        }

    }
#endif
}