using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {

    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/Asset")]
    public class AssetAudioPlayer : AudioPlayer {

        [HideInInspector] public bool useVolume;
        [UsageToggle("useVolume"), Range(0f, 1f)] public float volume = 1f;
        
        public AudioPlayerAsset asset;

        AudioPlayerAsset instance;
        public override IEnumerator PlayUsing(AudioSource source) {

            var oldVolume = source.volume; 

            if (useVolume) source.volume = volume;

            if (instance == null)
                instance = Object.Instantiate(asset);
            yield return instance.player.PlayUsingMaster(source);

            if (useVolume) source.volume = oldVolume;
        }
    }
}