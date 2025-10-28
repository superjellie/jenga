using UnityEngine;
namespace Jenga {

    [System.Serializable]
    public struct TransformSnapshot {
        public Vector3 localPosition;
        public Vector3 localEuler;
        public Vector3 localScale;

        public TransformSnapshot(bool hey = false) {
            localScale = Vector3.one;
            localPosition = Vector3.zero;
            localEuler = Vector3.zero;
        }

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
            localEuler = Quaternion.Slerp(
                Quaternion.Euler(from.localEuler), 
                Quaternion.Euler(to.localEuler), 
                t
            ).eulerAngles,
            localScale = Vector3.Lerp(from.localScale, to.localScale, t)
        };
    }

    [System.Serializable]
    public struct TransformRelativeSnapshot {
        public Transform parent;
        public Vector3 localPosition;
        public Vector3 localEuler;
        public Vector3 localScale;

        public TransformRelativeSnapshot(bool hey = false) {
            localScale = Vector3.one;
            parent = null;
            localPosition = Vector3.zero;
            localEuler = Vector3.zero;
        }

        public void SetTransform(Transform t) {
            t.parent = parent;
            t.localPosition = localPosition;
            t.localEulerAngles = localEuler;
            t.localScale = localScale;
        }

        public static TransformRelativeSnapshot FromTransform(Transform t) 
            => new TransformRelativeSnapshot() {
                localPosition = t.localPosition,
                localEuler = t.localEulerAngles,
                localScale = t.localScale,
                parent = t.parent
            };

        public TransformWorldSnapshot ToWorld()
            => new() { 
                position = parent == null 
                    ? localPosition 
                    : parent.TransformPoint(localPosition),
                eulerAngles = parent == null
                    ? localEuler
                    : (parent.rotation * Quaternion.Euler(localEuler))
                        .eulerAngles
            };
    }

    [System.Serializable]
    public struct TransformWorldSnapshot {
        public Vector3 position;
        public Vector3 eulerAngles;

        public void SetTransform(Transform t) {
            t.position = position;
            t.eulerAngles = eulerAngles;
        }

        public static TransformWorldSnapshot FromTransform(Transform t) 
            => new TransformWorldSnapshot() {
                position = t.position,
                eulerAngles = t.eulerAngles,
            };

        public static TransformWorldSnapshot Lerp(
            TransformWorldSnapshot from, TransformWorldSnapshot to, float t
        ) => new() {
            position = Vector3.Lerp(from.position, to.position, t),
            eulerAngles = Quaternion.Slerp(
                Quaternion.Euler(from.eulerAngles), 
                Quaternion.Euler(to.eulerAngles), 
                t
            ).eulerAngles,
        };
    }
    
}