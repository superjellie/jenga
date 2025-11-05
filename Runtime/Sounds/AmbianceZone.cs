using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    public class AmbianceZone : MonoBehaviour, ISerializationCallbackReceiver {

        [Range(0f, 100f)] public float rolloffDistance = 5f;
        [Range(0f, 100f)] public float maxDistance = 10f;
        public AnimationCurve rolloffCurve 
            = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);

        [FormerlySerializedAs("player")]
        public AudioPlayerReference playerOLD;

        [SerializeReference, TypeMenu]
        public AudioPlayer player;

        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { 
            player = playerOLD.value;
        }


        AudioListener listener_;
        public AudioListener listener => listener_ != null 
            ? listener_ 
            : listener_ = FindAnyObjectByType<AudioListener>();

        public Collider myCollider => GetComponent<Collider>();


        AudioSource source;
        Coroutine crtn;
        void OnEnable() => crtn = StartCoroutine(Play());

        void OnDisable() {
            Destroy(source.gameObject);
            StopCoroutine(crtn);
        }
    
        IEnumerator Play() {
            var go = new GameObject("AmbianceZone Source");
            source = go.AddComponent<AudioSource>();
            go.transform.SetParent(transform, false);
            source.spatialize = true;
            source.maxDistance = maxDistance;

            while (true)
                yield return player.PlayUsing(source);
        }

        void Update() {
            if (listener == null) return;
            if (myCollider == null) return;

            source.transform.position 
                = myCollider.ClosestPoint(listener.transform.position);

            var distance = Vector3.Distance(
                source.transform.position,
                listener.transform.position
            );

            var value = rolloffCurve.Evaluate(
                Mathf.Clamp(1f - distance / rolloffDistance, 0f, 1f)
            );

            source.spatialBlend = value;
        }

    }
}
