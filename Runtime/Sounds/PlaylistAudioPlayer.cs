using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/Playlist")]
    /** @brief Multifunctional audio playlist
     * Use this one if you want to merge many sounds in one playlist
     * You can either play all of them every time, or play them one-by-one
     * @see @ref manual-audio-player
     */
    public class PlaylistAudioPlayer : AudioPlayer {

        // Usage
        /** @brief How to sequence sounds in this playlist */
        public enum SequenceMode { 
            InOrder /** Play them one-by-one in order */, 
            Reverse /** Play them one-by-one in reverse */, 
            Shuffle  /** Shuffle list and then play them one-by-one. Resuffle if looping */, 
            Randomize /** Next sound is random, possibly same one. */
        }

        /** @brief What to do when played */
        public enum PlaybackMode { 
            PlayOne /** Play One sound and stop. Always Loops. */, 
            PlayAll /** Play all playlist. */
        }
        
        public SequenceMode sequenceMode = SequenceMode.InOrder;
        public PlaybackMode playbackMode = PlaybackMode.PlayOne;

        /** @brief RNG is needed if using 
         * Jenga.PlaylistAudioPlayer.SequenceMode.Shuffle 
         * or Jenga.PlaylistAudioPlayer.SequenceMode.Randomize */
        public RNGAsset rng;

        /** Whether to loop playlist 
         * in Jenga.PlaylistAudioPlayer.PlaybackMode.PlayAll mode */
        public bool loop = false;


        [SerializeReference, TypeMenu, Wrapper]
        public AudioPlayer[] players;

        int currentIndex = 0;
        public override IEnumerator PlayUsing(AudioSource source) {
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
                    ? players[^(currentIndex + 1)]
                    : players[currentIndex];
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