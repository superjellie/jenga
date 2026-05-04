using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class CommonAudioModifiers : MonoBehaviour {
        
        public float volumeFactor = 1f;
        // public float pitchFactor = 1f;

        public static CommonAudioModifiers Get(AudioSource source) {
            if (source.TryGetComponent<CommonAudioModifiers>(out var cam))
                return cam;
            return source.gameObject.AddComponent<CommonAudioModifiers>();
        }

        public static bool 
        TryGet(AudioSource source, out CommonAudioModifiers cam) 
            => source.TryGetComponent<CommonAudioModifiers>(out cam);

    }

    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/AudioModificator")]
    public class AudioModificatorPlayer : AudioPlayer {
            
        public float volumeFactor = 1f;

        [SerializeReference, TypeMenu, Wrapper]
        public AudioPlayer player;

        public override IEnumerator PlayUsing(AudioSource source) {
            var master = CoroutineMaster.GetOnObject(source.gameObject);
            var cam = CommonAudioModifiers.Get(source);
            var oldFactor = cam.volumeFactor;
            cam.volumeFactor *= volumeFactor;
            yield return player.PlayUsing(source);
            cam.volumeFactor = oldFactor;
        }

    }
}
