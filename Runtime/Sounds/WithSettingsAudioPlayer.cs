using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Jenga {

    [System.Serializable]
    [AddTypeMenu("Jenga.AudioPlayer/WithSettings")]
    public class WithSettingsAudioPlayer : AudioPlayer { 

        // Usage
        [HideInInspector] public bool useVolume;
        [HideInInspector] public bool usePitch;
        [HideInInspector] public bool useLoop;
        [HideInInspector] public bool useMute;
        [HideInInspector] public bool useSpatialBlend;
        [HideInInspector] public bool useStereoPan;
        [HideInInspector] public bool usePriority;
        [HideInInspector] public bool useMixerGroup;
        [HideInInspector] public bool useReverbZoneMix;

        //
        [UsageToggle("useMixerGroup")]   
        public AudioMixerGroup outputAudioMixerGroup;

        // Playback
        [UsageToggle("useLoop")] 
        public bool  loop;
        [UsageToggle("useMute")] 
        public bool  mute;

        // General
        [UsageToggle("useVolume"), Range(0f, 1f)] 
        public float volume = 1f;
        [UsageToggle("usePitch"), Range(-3f, 3f)] 
        public float pitch = 1f;
        [UsageToggle("useSpatialBlend"), Range(0f, 1f)] 
        public float spatialBlend = 1f;
        [UsageToggle("useStereoPan"), Range(-1f, 1f)] 
        public float stereoPan = 0f;
        [UsageToggle("usePriority"), Range(0, 256)] 
        public int priority = 128;
        [UsageToggle("useReverbZoneMix"), Range(0f, 1.1f)]   
        public float reverbZoneMix; 

        // Custom
        public bool keepOnListener;
        

        // Bypass
        [System.Serializable]
        public struct BypassSettings {
            [HideInInspector] public bool useBypassEffects;
            [HideInInspector] public bool useBypassListenerEffects;
            [HideInInspector] public bool useBypassReverbZones;

            [UsageToggle("useBypassEffects")]
            public bool bypassEffects;
            [UsageToggle("useBypassListenerEffects")]
            public bool bypassListenerEffects;
            [UsageToggle("useBypassReverbZones")]
            public bool bypassReverbZones;
        }

        public BypassSettings bypassSettings = new();

        // 3D Sound Settings
        [System.Serializable]
        public struct Audio3DSettings {

            // Usage
            [HideInInspector] public bool useDopplerLevel;
            [HideInInspector] public bool useSpread;
            [HideInInspector] public bool useRolloffMode;
            [HideInInspector] public bool useMinDistance;
            [HideInInspector] public bool useMaxDistance;

            // Settings
            [UsageToggle("useDopplerLevel"), Range(0f, 5f)] 
            public float dopplerLevel;
            [UsageToggle("useSpread"), Range(0f, 360f)]
            public float spread;
            [UsageToggle("useRolloffMode")]
            public AudioRolloffMode rolloffMode;
            [UsageToggle("useMinDistance")] 
            public float minDistance;
            [UsageToggle("useMaxDistance")] 
            public float maxDistance;
        } 

        public Audio3DSettings soundSettings3D = new() {
            dopplerLevel = 1f, spread = 0f, 
            rolloffMode = AudioRolloffMode.Logarithmic,
            minDistance = 1f, maxDistance = 500f
        };

        [SerializeReference, TypeMenu, Wrapper]
        public AudioPlayer player;

        AudioListener listener_;
        public AudioListener listener => listener_ != null 
            ? listener_ 
            : listener_ = Object.FindAnyObjectByType<AudioListener>();

        public override IEnumerator PlayUsing(AudioSource source) {

            var master = CoroutineMaster.GetOnObject(source.gameObject);

            // Save Playback
            var oldLoop = source.loop;
            var oldMute = source.mute;

            // Save General
            var oldVolume        = source.volume; 
            var oldPitch         = source.pitch;
            var oldSpatialBlend  = source.spatialBlend;
            var oldStereoPan     = source.panStereo;
            var oldPriority      = source.priority;
            var oldReverbZoneMix = source.reverbZoneMix;

            // Save Bypass Settings
            var oldBypassEffects         = source.bypassEffects;
            var oldBypassListenerEffects = source.bypassListenerEffects;
            var oldBypassReverbZones     = source.bypassReverbZones;

            // Save Audio3DSettings
            var oldDopplerLevel = source.dopplerLevel;
            var oldSpread       = source.spread;
            var oldRolloffMode  = source.rolloffMode;
            var oldMinDistance  = source.minDistance;
            var oldMaxDistance  = source.maxDistance;  

            // Save main
            var oldMixerGroup = source.outputAudioMixerGroup;
            var oldPosition   = source.transform.position;

            // Update Playback
            if (useLoop) source.loop = loop;
            if (useMute) source.mute = mute;

            // Update General
            if (useVolume)        source.volume        = volume;
            if (usePitch)         source.pitch         = pitch;
            if (useSpatialBlend)  source.spatialBlend  = spatialBlend;
            if (useStereoPan)     source.panStereo     = stereoPan;
            if (usePriority)      source.priority      = priority;
            if (useReverbZoneMix) source.reverbZoneMix = reverbZoneMix;

            // Update Bypass Settings
            if (bypassSettings.useBypassEffects) 
                source.bypassEffects = bypassSettings.bypassEffects;
            if (bypassSettings.useBypassListenerEffects) 
                source.bypassListenerEffects = bypassSettings.bypassListenerEffects;
            if (bypassSettings.useBypassReverbZones) 
                source.bypassReverbZones = bypassSettings.bypassReverbZones;

            // Update Audio3DSettings
            if (soundSettings3D.useDopplerLevel) 
                source.dopplerLevel = soundSettings3D.dopplerLevel;
            if (soundSettings3D.useSpread)       
                source.spread = soundSettings3D.spread;
            if (soundSettings3D.useRolloffMode)  
                source.rolloffMode = soundSettings3D.rolloffMode;
            if (soundSettings3D.useMinDistance)  
                source.minDistance = soundSettings3D.minDistance;
            if (soundSettings3D.useMaxDistance)  
                source.maxDistance = soundSettings3D.maxDistance;

            // Update Main
            if (useMixerGroup) 
                source.outputAudioMixerGroup = outputAudioMixerGroup;

            // Play subplayer
            if (keepOnListener)
                yield return CoroutineMaster.RunWithMain(
                    player.PlayUsing(source),
                    KeepOnListener(source)
                );
            else
                yield return player.PlayUsing(source);


            // Restore Main
            if (keepOnListener) source.transform.position = oldPosition;
            if (useMixerGroup) source.outputAudioMixerGroup = oldMixerGroup;

            // Restore Audio3DSettings      
            if (soundSettings3D.useDopplerLevel) 
                source.dopplerLevel = oldDopplerLevel;
            if (soundSettings3D.useSpread)       
                source.spread = oldSpread;
            if (soundSettings3D.useRolloffMode)  
                source.rolloffMode = oldRolloffMode;
            if (soundSettings3D.useMinDistance)  
                source.minDistance = oldMinDistance;
            if (soundSettings3D.useMaxDistance)  
                source.maxDistance = oldMaxDistance;

            // Restore Bypass  
            if (bypassSettings.useBypassEffects)
                source.bypassEffects = oldBypassEffects;
            if (bypassSettings.useBypassListenerEffects)
                source.bypassListenerEffects = oldBypassListenerEffects;
            if (bypassSettings.useBypassReverbZones)
                source.bypassReverbZones = oldBypassReverbZones;

            // Restore General
            if (useVolume)        source.volume        = oldVolume;
            if (usePitch)         source.pitch         = oldPitch;
            if (useSpatialBlend)  source.spatialBlend  = oldSpatialBlend;
            if (useStereoPan)     source.panStereo     = oldStereoPan;
            if (usePriority)      source.priority      = oldPriority;
            if (useReverbZoneMix) source.reverbZoneMix = oldReverbZoneMix;
            
            // Restore Playback
            if (useLoop) source.loop = oldLoop;
            if (useMute) source.mute = oldMute;
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