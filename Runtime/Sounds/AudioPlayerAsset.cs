using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    [CreateAssetMenu(fileName = "AudioPlayer", menuName = "Jenga/Audio Player")]
    /** ScriptableObject Asset containing AudioPlayer.
     * @see Jenga.AssetAudioPlayer
     * @see @ref manual-audio-player
     */
    public class AudioPlayerAsset : ScriptableObject {

        [SerializeReference, TypeMenu]
        public AudioPlayer player;
    }
}