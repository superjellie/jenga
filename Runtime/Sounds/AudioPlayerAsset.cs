using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Jenga {
    [CreateAssetMenu(fileName = "AudioPlayer", menuName = "Jenga/Audio Player")]
    public class AudioPlayerAsset : ScriptableObject, ISerializationCallbackReceiver {

        [FormerlySerializedAs("player")]
        public AudioPlayerReference playerOLD;
    
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { 
            player = playerOLD.value;
        }

        public AudioPlayer player;
    }
}