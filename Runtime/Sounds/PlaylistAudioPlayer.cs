using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu(typeof(AudioPlayer), "Composite/Playlist", 1)]
    public class PlaylistAudioPlayer : AudioPlayer {

        // Usage
        public enum SequenceMode { InOrder, Reverse, Shuffle, Randomize }
        public enum PlaybackMode { PlayOne, PlayAll }
        public SequenceMode sequenceMode = SequenceMode.InOrder;
        public PlaybackMode playbackMode = PlaybackMode.PlayOne;
        public RNGAsset rng;

        [ALay.ListView(showFoldoutHeader = false)]
        public AudioPlayerReference[] items;

        int currentIndex = 0;
        public override IEnumerator PlayUsing(AudioSource source) {
            if (items.Length == 0) yield break;

            if (playbackMode == PlaybackMode.PlayAll)
                currentIndex = 0;

        REPEAT:

            if (currentIndex >= items.Length) {
                if (playbackMode == PlaybackMode.PlayOne)
                    currentIndex = 0;
                else 
                    yield break;
            }

            if (sequenceMode == SequenceMode.Shuffle && currentIndex == 0)
                AQRY.Shuffle<AudioPlayerReference>(rng.GetInt, items);

            if (sequenceMode == SequenceMode.Randomize)
                currentIndex = rng.GetIntInRange(0, items.Length);

            var player 
                = sequenceMode == SequenceMode.Reverse 
                    ? items[items.Length - currentIndex - 1]
                    : items[currentIndex];
            // Debug.Log(currentIndex);
            currentIndex++;

            if (player != null)
                yield return player.PlayUsing(source);
            else
                yield return null; 


            if (playbackMode == PlaybackMode.PlayAll)
                goto REPEAT;

            yield break;
        }
    }
}