using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    [CreateAssetMenu(fileName = "AudioPlayer", menuName = "Jenga/Audio Player")]
    public class AudioPlayerAsset : ScriptableObject {

        [SerializeReference, TypeMenu]
        public AudioPlayer player;
    }
}