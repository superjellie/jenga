using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    /** @brief Base class of AudioPlayers.
     * 
     * To create new AudioPlayer inherit from this one and override 
     * PlayUsing method. 
     * 
     * @see @ref manual-audio-player
     */
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer")]
    [AddTypeMenu("Jenga.AudioPlayer/Nothing")]
    public class AudioPlayer {

        /** Play AudioPlayer on given audio source 
         * @param source given source 
         */
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

        /** Create new AudioSource to use later. 
         * @param position where to place AudioSource
         * @param parent optional parent for new source
         */
        public static AudioSource
        GetNewSource(Vector3 position, Transform parent = null) {
            var go = new GameObject("AudioPlayer");
            var src = go.AddComponent<AudioSource>();

            go.transform.parent = parent;
            go.transform.localPosition = position;

            return src;
        }

        /** 
         * Create AudioSource at given position, play AudioPlayer and destroy
         * source after.
         * @param position where to place AudioSource
         * @param parent optional parent for new source
         */
        public AudioSource PlayAt(Vector3 position, Transform parent = null) {
            var src = GetNewSource(position, parent);
            var master = CoroutineMaster.GetOnObject(src.gameObject);
            var crtn = PlayUsing(src);
            master.PlayCoroutineAndDestroy(crtn);
            return src;
        }

        /** 
         * Play AudioClip at given position. Will play metadata if there is any. 
         * @param clip AudioClip to play
         * @param position where to place AudioSource
         * @param parent optional parent for new source
         */
        public static AudioSource PlayClipAt(
            AudioClip clip, Vector3 position, Transform parent = null
        ) {
            var player = new ClipAudioPlayer() { clip = clip };
            return player.PlayAt(position, parent);
        }

    }
}
