using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/Clip")]
    public class ClipAudioPlayer : AudioPlayer {

        public AudioClip clip;
        protected virtual bool ignoreMetadata => false;

        static HashSet<(AudioSource, AudioClip)> takenMetadataOn = new();

        public override IEnumerator PlayUsing(AudioSource source) {

            AudioPlayerAsset asset = null;
            if (!ignoreMetadata && MetadataMasterAsset.main != null
                && !takenMetadataOn.Contains((source, clip))) {
                asset = MetadataMasterAsset.main
                    .GetMetadata<AudioPlayerAsset>(clip);
            }

            if (asset != null) {
                takenMetadataOn.Add((source, clip));
                yield return asset.player.PlayUsing(source);
                takenMetadataOn.Remove((source, clip));
            } else {
                var volumeFactor = 1f;
                if (CommonAudioModifiers.TryGet(source, out var cam))
                    volumeFactor = cam.volumeFactor;

                source.PlayOneShot(clip, source.volume * volumeFactor);
                yield return null;
                yield return new WaitWhile(() => source != null && source.isPlaying);
            }
        }

    }

    // Just wanted to separate them to reduce bugs
    // Still need to do smth about preventing recoursion
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/ClipIgnoreMetadata")]
    public class ClipIgnoreMetadataAudioPlayer : ClipAudioPlayer {
        protected override bool ignoreMetadata => true;
    }
}