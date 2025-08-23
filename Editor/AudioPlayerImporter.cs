using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AssetImporters;

namespace Jenga {
    // [ScriptedImporter(version: 1, ext: "wav")]
    public class AudioPlayerImporter : ScriptedImporter {

        public AudioPlayer player;

        public override void OnImportAsset(AssetImportContext ctx) {
            Debug.Log("HEr");
            // asset = ScriptableObject.CreateInstance<AudioPlayerAsset>();
            // EditorUtility.SetDirty(this);
        }
    }
}
