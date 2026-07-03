using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer")]
    [AddTypeMenu("Jenga.AudioPlayer/Nothing")]
    public class AudioPlayer {


        public virtual IEnumerator PlayUsing(AudioSource source) => null;

        [System.Obsolete("Use CoroutineMaster explicitly")]
        public IEnumerator PlayUsingMaster(AudioSource source)
            => CoroutineMaster
                .StartOnObject(source.gameObject, PlayUsing(source));

        [System.Obsolete("Use CoroutineMaster explicitly")]
        public Coroutine PlayUsingNewSource(Vector3 pos)
            => CoroutineMaster.main.StartCoroutine(PlayOnNewSourceCrtn(pos));

        IEnumerator PlayOnNewSourceCrtn(Vector3 pos) {
            var go = new GameObject();
            var src = go.AddComponent<AudioSource>();
            go.transform.position = pos;

            yield return PlayUsing(src);

            if (go != null)
                GameObject.Destroy(go);
        }

        public static AudioSource
        GetNewSource(Vector3 position, Transform parent = null) {
            var go = new GameObject("AudioPlayer");
            var src = go.AddComponent<AudioSource>();

            go.transform.parent = parent;
            go.transform.localPosition = position;

            return src;
        }


        public AudioSource PlayAt(Vector3 position, Transform parent = null) {
            var src = GetNewSource(position, parent);
            var master = CoroutineMaster.GetOnObject(src.gameObject);
            var crtn = PlayUsing(src);
            master.PlayCoroutineAndDestroy(crtn);
            return src;
        }

        public static AudioSource PlayClipAt(
            AudioClip clip, Vector3 position, Transform parent = null
        ) {
            var player = new ClipAudioPlayer() { clip = clip };
            return player.PlayAt(position, parent);
        }

    }
}
