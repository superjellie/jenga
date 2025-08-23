using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {

    [System.Serializable]
    [AddTypeMenu(typeof(AudioPlayer), "Play/Asset", 1)]
    public class AssetAudioPlayer : AudioPlayer {


        // Usage
        [ALay.Skip] public bool useVolume;
        [ALay.UsageToggle("useVolume"), Range(0f, 1f)] public float volume = 1f;
        public AudioPlayerAsset asset;

        AudioPlayerAsset instance;
        public override IEnumerator PlayUsing(AudioSource source) {

            var oldVolume = source.volume; 

            if (useVolume) source.volume = volume;

            if (instance == null)
                instance = Object.Instantiate(asset);
            yield return instance.player.PlayUsing(source);

            if (useVolume) source.volume = oldVolume;
        }
    }
}