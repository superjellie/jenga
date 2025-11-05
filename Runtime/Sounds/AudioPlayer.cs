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
}