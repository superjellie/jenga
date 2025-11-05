using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {
    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/WithSettings")]
    public class WithSettingsAudioPlayer : AudioPlayer, ISerializationCallbackReceiver { 

        // Usage
        [HideInInspector] public bool useVolume;
        [HideInInspector] public bool usePitch;
        [HideInInspector] public bool useLoop;
        [HideInInspector] public bool useSpatialBlend;
        [HideInInspector] public bool useMixerGroup;

        [UsageToggle("useVolume"), Range(0f, 1f)] 
        public float volume = 1f;
        [UsageToggle("usePitch"), Range(-3f, 3f)] 
        public float pitch = 1f;
        [UsageToggle("useLoop")] 
        public bool  loop;
        [UsageToggle("useSpatialBlend"), Range(0f, 1f)] 
        public float spatialBlend = 1f;
        [UsageToggle("useMixerGroup")]   
        public AudioMixerGroup outputAudioMixerGroup;
        public bool keepOnListener;

        [SerializeReference, TypeMenu, Wrapper]
        public AudioPlayer player;

        public AudioPlayerReference item;
        public void OnBeforeSerialize() { }
        public void OnAfterDeserialize() { 
            player = item.value;
        }

        AudioListener listener_;
        public AudioListener listener => listener_ != null 
            ? listener_ 
            : listener_ = Object.FindAnyObjectByType<AudioListener>();

        public override IEnumerator PlayUsing(AudioSource source) {

            var oldVolume           = source.volume; 
            var oldPitch            = source.pitch;
            var oldLoop             = source.loop;
            var oldSpatialBlend     = source.spatialBlend;
            var oldMixerGroup       = source.outputAudioMixerGroup;
            var oldPosition         = source.transform.position;

            if (useVolume)          source.volume           = volume;
            if (usePitch)           source.pitch            = pitch;
            if (useLoop)            source.loop             = loop;
            if (useSpatialBlend)    source.spatialBlend     = oldSpatialBlend;
            if (useMixerGroup) source.outputAudioMixerGroup = outputAudioMixerGroup;

            var master = CoroutineMaster.GetOnObject(source.gameObject);
            
            Coroutine crtn = null;
            if (keepOnListener)
                crtn = master.StartCoroutine(KeepOnListener(source));

            yield return player.PlayUsingMaster(source);

            if (keepOnListener)
                master.StopCoroutine(crtn);

            if (useVolume)          source.volume           = oldVolume;
            if (usePitch)           source.pitch            = oldPitch;
            if (useLoop)            source.loop             = oldLoop;
            if (useSpatialBlend)    source.spatialBlend     = oldSpatialBlend;
            if (useMixerGroup) source.outputAudioMixerGroup = oldMixerGroup;
        
            if (keepOnListener) source.transform.position = oldPosition;
        }
        IEnumerator KeepOnListener(AudioSource source) {

            while (true) {
                if (source != null && listener != null)
                    source.transform.position = listener.transform.position;
                yield return null;
            }
        }
    }

}