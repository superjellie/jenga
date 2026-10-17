using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/Silence")]
    /** Plays nothing for given number of seconds.
     * @see @ref manual-audio-player
     */
    public class SilenceAudioPlayer : AudioPlayer {
        public float seconds = 1f;

        public override IEnumerator PlayUsing(AudioSource source) {
            yield return new WaitForSeconds(seconds);
        }
    }
}