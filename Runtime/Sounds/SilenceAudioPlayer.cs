using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/Silence")]
    public class SilenceAudioPlayer : AudioPlayer {

        // Usage
        // [ALay.Skip] public bool useVolume;
        // [ALay.Skip] public bool useMixerGroup;
        // [ALay.UsageToggle("useVolume"), Range(0f, 1f)] public float volume = 1f;
        // [ALay.UsageToggle("useMixerGroup")]   
        // public AudioMixerGroup outputAudioMixerGroup;
        public float seconds = 1f;

        public override IEnumerator PlayUsing(AudioSource source) {
            yield return new WaitForSeconds(seconds);
        }
    }
}