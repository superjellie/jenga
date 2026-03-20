using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer")]
    [AddTypeMenu("Jenga.AudioPlayer/Nothing")]
    public class AudioPlayer : ALay.ILayoutMe {


        protected virtual IEnumerator PlayUsing(AudioSource source) => null;

        public Coroutine PlayUsingMaster(AudioSource source) 
            => CoroutineMaster.GetOnObject(source.gameObject)
                .StartCoroutine(PlayUsing(source));

        IEnumerator PlayOnNewSourceCrtn(Vector3 pos) {
            var go = new GameObject();
            var src = go.AddComponent<AudioSource>();
            go.transform.position = pos;

            yield return PlayUsingMaster(src);

            if (go != null)
                GameObject.Destroy(go);
        }

        public Coroutine PlayUsingNewSource(Vector3 pos) 
            => CoroutineMaster.main.StartCoroutine(PlayOnNewSourceCrtn(pos));

        public void PlayAt(Vector3 position, Transform parent = null) {
            var go = new GameObject("AudioPlayer");
            var src = go.AddComponent<AudioSource>();

            go.transform.parent = parent;
            go.transform.localPosition = position;

            var master = CoroutineMaster.GetOnObject(go);
            master.PlayCoroutineAndDestroy(PlayUsing(src));
        }

        public static void PlayClipAt(
            AudioClip clip, Vector3 position, Transform parent = null
        ) {
            var player = new ClipAudioPlayer() { clip = clip };

            var go = new GameObject("AudioPlayer");
            var src = go.AddComponent<AudioSource>();

            go.transform.parent = parent;
            go.transform.localPosition = position;

            var master = CoroutineMaster.GetOnObject(go);
            master.PlayCoroutineAndDestroy(player.PlayUsing(src));
        }

    }

    [System.Serializable, System.Obsolete]
    public class AudioPlayerReference : ALay.ILayoutMe {
        // [SerializeReference] public AudioPlayer value = new();

        // public virtual Coroutine PlayUsing(AudioSource source) 
        //     => CoroutineMaster.GetOnObject(source.gameObject)
        //         .StartCoroutine(value?.PlayUsing(source));
        
        // public static implicit operator AudioPlayerReference(AudioPlayer player)
        //     => new() { value = player };
    }
}