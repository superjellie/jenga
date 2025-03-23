using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {
    [System.Serializable, ALay.Inline]
    public class AudioClipWithMixer : ALay.ILayoutMe {

        [ALay.Style(flexGrow = 1f)] 
        public AudioClip clip;

        [ALay.Style(width = 80f)]
        public AudioMixerGroup group;

        public AudioSource Play(Vector3 pos) {
            var go = new GameObject("AudioClipPlayer", typeof(AudioSource));

            var source = go.GetComponent<AudioSource>();

            source.clip = clip;
            source.outputAudioMixerGroup = group;

            source.Play();

            GameObject.Destroy(go, clip.length);
            return source;
        }

        public AudioSource PlayLooped(Vector3 pos) {
            var source = Play(pos);
            source.loop = true;
            return source;
        }
    }
}