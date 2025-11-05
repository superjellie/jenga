using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Jenga {
    public class AudioMetadataWindow : EditorWindow {

        public MetadataMasterAsset master;
        
        [MenuItem("Window/Jenga/Audio Metadata Editor")]
        public static void ShowWindow() {
            var wnd = GetWindow<AudioMetadataWindow>();
            wnd.titleContent = new GUIContent("Audio Metadata Editor");
        }

        string lastMetadataGUID; 
        public void CreateGUI() {
            VisualElement root = rootVisualElement;

            root.Add(new PropertyField() { bindingPath = "master" });

            var so = new SerializedObject(this);
            root.Bind(so);


            var buttonAdd = new Button() { text = "Create Audio Metadata" };
            buttonAdd.clicked += () => {
                if (Selection.assetGUIDs.Length == 0) return;
                if (master == null) return;

                var guid = Selection.assetGUIDs[0];
                var metadata = master.GetMetadata<AudioPlayerAsset>(guid);
                if (metadata != null) return;

                var path = AssetDatabase.GUIDToAssetPath(guid);

                var data = master.CreateMetadata<AudioPlayerAsset>(guid);
                var clip = (AudioClip)AssetDatabase
                    .LoadAssetAtPath(path, typeof(AudioClip));
                data.player = new WithSettingsAudioPlayer() {
                    player = new ClipAudioPlayer() 
                        { clip = clip, ignoreMetadata = true }
                };
            };
            root.Add(buttonAdd);

            var buttonRemove = new Button() { text = "Remove Audio Metadata" };
            buttonRemove.clicked += () => {
                if (Selection.assetGUIDs.Length == 0) return;
                if (master == null) return;

                var guid = Selection.assetGUIDs[0];
                var metadata = master.GetMetadata<AudioPlayerAsset>(guid);
                if (metadata == null) return;

                master.RemoveMetadata(metadata);
            };
            root.Add(buttonRemove);

            var inspector = new InspectorElement();
            root.Add(inspector);
            root.schedule.Execute(() => {

                if (Selection.assetGUIDs.Length == 0) goto NO_SELECTION;
                if (master == null) goto NO_SELECTION;

                var guid = Selection.assetGUIDs[0];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var owner = (AudioClip)AssetDatabase
                    .LoadAssetAtPath(path, typeof(AudioClip));
                if (owner == null) goto NO_SELECTION;

                var metadata = master.GetMetadata<AudioPlayerAsset>(owner);

                if (metadata == null) goto NO_METADATA;

                goto HAS_INSPECTOR;

            HAS_INSPECTOR:
                if (lastMetadataGUID != guid) {
                    buttonAdd.style.display = DisplayStyle.None;
                    buttonRemove.style.display = DisplayStyle.Flex;
                    inspector.visible = true;
                    inspector.Bind(new SerializedObject(metadata));
                    lastMetadataGUID = guid;
                }

                return;

            NO_SELECTION:
                inspector.Unbind();
                inspector.visible = false;
                buttonAdd.style.display = DisplayStyle.None;
                buttonRemove.style.display = DisplayStyle.None;
                lastMetadataGUID = "";
                return;

            NO_METADATA:
                inspector.Unbind();
                inspector.visible = false;
                buttonAdd.style.display = DisplayStyle.Flex;
                buttonRemove.style.display = DisplayStyle.None;
                lastMetadataGUID = "";
                return;

            }).Every(100).StartingIn(0);

        }
    }
}
