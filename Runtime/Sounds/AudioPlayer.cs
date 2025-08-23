using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [System.Serializable, ALay.HideHeader]
    [AddTypeMenu(typeof(AudioPlayer), "Play/Nothing", 1)]
    public class AudioPlayer : ALay.ILayoutMe {
        public virtual IEnumerator PlayUsing(AudioSource source) => null;
    }

    [System.Serializable]
    [ALay.TypeSelector(typeof(AudioPlayer), path = "value")]
    public class AudioPlayerReference : ALay.ILayoutMe {
        [SerializeReference] public AudioPlayer value = new();

        public virtual Coroutine PlayUsing(AudioSource source) 
            => CoroutineMaster.GetOnObject(source.gameObject)
                .StartCoroutine(value?.PlayUsing(source));
        
        public static implicit operator AudioPlayerReference(AudioPlayer player)
            => new() { value = player };
    }
}