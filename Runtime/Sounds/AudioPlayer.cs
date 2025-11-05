using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer")]
    [AddTypeMenu("Jenga.AudioPlayer/Nothing")]
    public class AudioPlayer : ALay.ILayoutMe {
        public virtual IEnumerator PlayUsing(AudioSource source) => null;
        public virtual Coroutine PlayUsingMaster(AudioSource source) 
            => CoroutineMaster.GetOnObject(source.gameObject)
                .StartCoroutine(PlayUsing(source));
    }

    [System.Serializable, System.Obsolete]
    public class AudioPlayerReference : ALay.ILayoutMe {
        [SerializeReference] public AudioPlayer value = new();

        public virtual Coroutine PlayUsing(AudioSource source) 
            => CoroutineMaster.GetOnObject(source.gameObject)
                .StartCoroutine(value?.PlayUsing(source));
        
        public static implicit operator AudioPlayerReference(AudioPlayer player)
            => new() { value = player };
    }
}