using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static class MenuFunctions {
    

        [MenuItem("Assets/Add Audio Player", true)]
        public static bool CanAddAudioPlayer() {
            if (Selection.assetGUIDs.Length == 0)   
                return false;

            foreach (var guid in Selection.assetGUIDs) {
                var clip = AssetDatabase.LoadAssetAtPath(
                    AssetDatabase.GUIDToAssetPath(guid), 
                    typeof(AudioClip)
                ) as AudioClip;
                if (clip == null) return false;
            }

            return true;
        }

        [MenuItem("Assets/Add Audio Player")]
        public static void AddAudioPlayer() {
            foreach (var guid in Selection.assetGUIDs) {
                var path = AssetDatabase.GUIDToAssetPath(guid);

                var clip = AssetDatabase.LoadAssetAtPath(
                    AssetDatabase.GUIDToAssetPath(guid), 
                    typeof(AudioClip)
                ) as AudioClip;

                var player = AssetDatabase.LoadAssetAtPath(
                    AssetDatabase.GUIDToAssetPath(guid), 
                    typeof(AudioPlayerAsset)
                ) as AudioPlayerAsset;

                if (player != null) continue;

                var newPlayer = ScriptableObject
                    .CreateInstance<AudioPlayerAsset>();
                newPlayer.player = new ClipAudioPlayer() { clip = clip };

                AssetDatabase.AddObjectToAsset(newPlayer, path);
            }
        }


    }
}
