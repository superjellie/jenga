using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    [FilePath(
        "Jenga/EditorData.asset", 
        FilePathAttribute.Location.PreferencesFolder
    )]
    public class EditorData : ScriptableSingleton<EditorData>, 
        ISerializationCallbackReceiver {

        [System.Serializable]
        struct DataEntry { 
            public string path;
            public string value;
        }

        [SerializeField] List<DataEntry> entries = new();
        public PathTree<string> data = new();

        void ISerializationCallbackReceiver.OnBeforeSerialize() { 
            entries.Clear();
            foreach (var (path, value) in data.Walk())
                entries.Add(new() { path = path, value = value });
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize() {
            data.Clear(); 
            foreach (var entry in entries)
                data.Add(entry.path, entry.value);
        }

        public void DoSave() => Save(true);
        void OnDisable() => Save(true);
    }
 
    public static partial class JengaEditorGUI {
        static string currentDataRoot = "";

        public static void ResetDataGroup() 
            => currentDataRoot = "";
        public static void BeginDataGroup(string key) {
            currentDataRoot += $"/{key}";
            // Debug.Log($"Begin: {currentDataRoot}");
        }
        public static void EndDataGroup() {
            // Debug.Log($"End: {currentDataRoot}");
            currentDataRoot = currentDataRoot.Substring(
                0, currentDataRoot.LastIndexOf('/')
            );
        }

        [System.Serializable]
        struct DataWrapper<T> { public T value; }

        public static T GetDataValueOrDefault<T>(string key, T defaultValue) {
            if (EditorData.instance == null) return defaultValue;
            if (EditorData.instance.data == null) return defaultValue;
            if (EditorData.instance.data
                    .TryGetValue($"{currentDataRoot}/{key}", out var value)) {
                var wrapper = JsonUtility.FromJson<DataWrapper<T>>(value);
                return wrapper.value;
            }
            return defaultValue;
        }

        public static void SetDataValue<T>(string key, T value) {

            
            if (EditorData.instance == null) return;
            if (EditorData.instance.data == null) return;
            var json = JsonUtility
                .ToJson(new DataWrapper<T>() { value = value });

            if (EditorData.instance.data
                    .TryGetValue($"{currentDataRoot}/{key}", out var myJson))
                if (json == myJson)
                    return;

            EditorData.instance.data.Add($"{currentDataRoot}/{key}", json);
            // EditorUtility.SetDirty(EditorData.instance);
            // EditorData.instance.DoSave();
        }


    }
}
