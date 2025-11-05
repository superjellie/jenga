using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/Playlist")]
    public class PlaylistAudioPlayer : AudioPlayer {

        // Usage
        public enum SequenceMode { InOrder, Reverse, Shuffle, Randomize }
        public enum PlaybackMode { PlayOne, PlayAll }
        public SequenceMode sequenceMode = SequenceMode.InOrder;
        public PlaybackMode playbackMode = PlaybackMode.PlayOne;
        public RNGAsset rng;
        public bool loop = false;



        [SerializeReference, TypeMenu, Wrapper]
        public AudioPlayer[] players;

        int currentIndex = 0;
        public override IEnumerator PlayUsing(AudioSource source) {
        LOOP:
            if (players.Length == 0) yield break;

            if (playbackMode == PlaybackMode.PlayAll)
                currentIndex = 0;


        REPEAT:

            if (currentIndex >= players.Length) {
                if (playbackMode == PlaybackMode.PlayOne)
                    currentIndex = 0;
                else if (playbackMode == PlaybackMode.PlayAll && loop)
                    currentIndex = 0;
                else
                    yield break;
            }

            if (sequenceMode == SequenceMode.Shuffle && currentIndex == 0)
                AQRY.Shuffle<AudioPlayer>(rng.GetInt, players);

            if (sequenceMode == SequenceMode.Randomize)
                currentIndex = rng.GetIntInRange(0, players.Length);

            var player 
                = sequenceMode == SequenceMode.Reverse 
                    ? players[^currentIndex]
                    : players[currentIndex];
            // Debug.Log(currentIndex);
            currentIndex++;

            if (player != null)
                yield return player.PlayUsingMaster(source);
            else
                yield return null; 


            if (playbackMode == PlaybackMode.PlayAll)
                goto REPEAT;


            yield break;
        }
    }
}