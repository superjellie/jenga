using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu(typeof(AudioPlayer), "Play/Clip", 1)]
    public class ClipAudioPlayer : AudioPlayer {

        // Usage
        // [ALay.Skip] public bool useVolume;
        // [ALay.Skip] public bool useMixerGroup;
        // [ALay.UsageToggle("useVolume"), Range(0f, 1f)] public float volume = 1f;
        // [ALay.UsageToggle("useMixerGroup")]   
        // public AudioMixerGroup outputAudioMixerGroup;
        public AudioClip clip;
        public bool ignoreMetadata;

        public override IEnumerator PlayUsing(AudioSource source) {
        // Debug.Log("HJet");

            // var oldVolume = source.volume; 
            // var oldMixerGroup = source.outputAudioMixerGroup; 

            // if (useVolume) 
            //     source.volume = volume;
            // if (useMixerGroup) 
            //     source.outputAudioMixerGroup = outputAudioMixerGroup;

            AudioPlayerAsset asset = null;
            if (!ignoreMetadata && MetadataMasterAsset.main != null) {
                asset = MetadataMasterAsset.main
                    .GetMetadata<AudioPlayerAsset>(clip);
            }

            if (asset != null) {
                yield return asset.player.PlayUsing(source);
            } else {
                source.clip = clip;
                source.Play();
                // Debug.Log(clip);
                yield return null;
                yield return new WaitWhile(() => source != null && source.isPlaying);
            }

            // if (useVolume) 
            //     source.volume = oldVolume;
            // if (useMixerGroup) 
            //     source.outputAudioMixerGroup = oldMixerGroup;
        }
    }
}