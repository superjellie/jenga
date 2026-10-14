using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // If you want, you can carry custom data on AudioSource GameObject
    // This example uses additional MonoBehaviour to pass volumeFactor inside
    // ClipAudioPlayer
    public class CommonAudioModifiers : MonoBehaviour {
        
        public float volumeFactor = 1f;
        // public float pitchFactor = 1f;

        public static CommonAudioModifiers Get(AudioSource source) {
            if (source.TryGetComponent<CommonAudioModifiers>(out var cam))
                return cam;
            return source.gameObject.AddComponent<CommonAudioModifiers>();
        }

        public static bool 
        TryGet(AudioSource source, out CommonAudioModifiers cam) 
            => source.TryGetComponent<CommonAudioModifiers>(out cam);

    }

    // These to attributes required to Serialization System and TypeMenu's 
    // system to see your audio player. Name (or path) "AudioModificator" 
    // in AddTypeMenu attribute will correspond to name in TypeMenu.
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/AudioModificator")]
    /** Changes volume of sound.
     * 
     * This class helps you modify volume of your sound.
     * It will also probably have some common modificators later.
     */
    public class AudioModificatorPlayer : AudioPlayer {
        
        // Inside this class you can add properties just like in 
        // any MonoBehaviour
        
        public float volumeFactor = 1f; 
            /**< Volume factor by witch to change volume. */

        // Composition hierarchy is made using AudioPlayer values 
        // in AudioPlayers. Wrapper Attribute adds Wrap functionality.  
        [SerializeReference, TypeMenu, Wrapper]
        public AudioPlayer player;

        // This coroutine will play every time somebody plays AudioPlayer
        // AudioSource param is where our sound is played
        public override IEnumerator PlayUsing(AudioSource source) {

            // This example adds it's volumeFactor to pass down hierarchy
            var cam = CommonAudioModifiers.Get(source);
            var oldFactor = cam.volumeFactor;
            cam.volumeFactor *= volumeFactor;

            // Usually when you modify other players you will wantt to play them 
            // at some point. Use Their PlayUsing method for this.
            yield return player.PlayUsing(source);

            // It's a good practice to reset all our non-permanent changes
            // after we played everything. This way if we will play something
            // with AudioModificator later we will not interfere.
            cam.volumeFactor = oldFactor;
        }

    }
}
