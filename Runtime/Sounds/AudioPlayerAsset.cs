using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [CreateAssetMenu(fileName = "AudioPlayer", menuName = "Jenga/Audio Player")]
    public class AudioPlayerAsset : ScriptableObject {
        public AudioPlayer player;
    }
}