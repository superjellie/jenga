using UnityEngine;
namespace Jenga {

    [System.Serializable]
    public class TransformSnapshot {
        public Vector3 localPosition;
        public Vector3 localEuler;
        public Vector3 localScale = Vector3.one;

        public void SetTransform(Transform t) {
            t.localPosition = localPosition;
            t.localEulerAngles = localEuler;
            t.localScale = localScale;
        }

        public static TransformSnapshot FromTransform(Transform t) 
            => new TransformSnapshot() {
                localPosition = t.localPosition,
                localEuler = t.localEulerAngles,
                localScale = t.localScale
            };

        public static TransformSnapshot Lerp(
            TransformSnapshot from, TransformSnapshot to, float t
        ) => new TransformSnapshot() {
            localPosition = Vector3.Lerp(from.localPosition, to.localPosition, t),
            localEuler = Vector3.Slerp(from.localEuler, to.localEuler, t),
            localScale = Vector3.Lerp(from.localScale, to.localScale, t)
        };
    }
    
}