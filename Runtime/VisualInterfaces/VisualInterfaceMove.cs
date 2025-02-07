using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jenga {
    [RequireComponent(typeof(VisualInterface))]
    public class VisualInterfaceMove : MonoBehaviour {

        public VisualInterface vi => GetComponent<VisualInterface>();

        public VisualStateData<TransformSnapshot> statePositions = new();
        public VisualTransitionData<CurveWithDuration> curves = new() {
            fallback = new CurveWithDuration()
        };

        void Awake() {
            vi.onStateChange += OnStateChange;
        }

        void Start() {
            OnStateChange(0, vi.state, true);
        }

        Coroutine crtn = null;

        void OnStateChange(int oldState, int newState, bool immediate) {
            if (crtn != null) StopCoroutine(crtn);

            var start = TransformSnapshot.FromTransform(transform);
            var end = statePositions.Get(newState);

            if (immediate)
                end.SetTransform(transform);
            else
                crtn = curves.Get(oldState, newState).Tween(this, 
                    (t) => TransformSnapshot
                        .Lerp(start, end, t).SetTransform(transform)
                );
        }

    #if UNITY_EDITOR

        void DrawLabelFor(string name, TransformSnapshot snapshot) {
            var content = new GUIContent($"{name}");
            var style = new GUIStyle(GUI.skin.button) {
                alignment = TextAnchor.MiddleCenter,

            };

            // var size = style.CalcSize(content);

            var color = GUI.backgroundColor;
            GUI.backgroundColor = new Color(.2f, .4f, 0f, 1f);
            Handles.Label(snapshot.localPosition, content, style);
            GUI.backgroundColor = color;
        }

        void OnDrawGizmosSelected() {
            var matrix = Handles.matrix;
            Handles.matrix = transform.parent != null 
                ? transform.parent.localToWorldMatrix : Matrix4x4.identity;

            foreach (var matcher in statePositions.matchers)
                DrawLabelFor($"[Mask: {matcher.mask}]", matcher.data);

            DrawLabelFor($"[Fallback]", statePositions.fallback);


            Handles.matrix = matrix;
        }
    #endif
    }
}