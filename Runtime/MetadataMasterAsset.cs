using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    [CreateAssetMenu(
        fileName = "MetadataMaster", menuName = "Jenga/Metadata Master")]
    public class MetadataMasterAsset : ScriptableObject {

        public static MetadataMasterAsset main;

        [System.Serializable]
        public struct MetadataPointers {
            public Object metadata;
            public Object owner;
        }

        public List<MetadataPointers> assets = new();

        public T GetMetadata<T>(Object owner) where T : Object {
            foreach (var asset in assets)
                if (owner == asset.owner && asset.metadata is T t)
                    return t;
            return null;
        }



#if UNITY_EDITOR
        public T CreateMetadata<T>(Object owner) where T : ScriptableObject {
            var instance = ScriptableObject.CreateInstance<T>();
            instance.name = $"{owner.name}:{typeof(T)}";
            UnityEditor.AssetDatabase.AddObjectToAsset(instance, this);
            assets.Add(new() { metadata = instance, owner = owner });

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            return instance;
        }
        public Object GetMetadata<T>(string guid) where T : Object {
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var owner = UnityEditor.AssetDatabase.LoadMainAssetAtPath(path);
            return GetMetadata<T>(owner);
        }

        public T CreateMetadata<T>(string guid) where T : ScriptableObject {
            var instance = ScriptableObject.CreateInstance<T>();
            instance.name = guid;
            UnityEditor.AssetDatabase.AddObjectToAsset(instance, this);
            var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            var owner = UnityEditor.AssetDatabase.LoadMainAssetAtPath(path);
            // Debug.Log(owner);
            assets.Add(new() { metadata = instance, owner = owner });

            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
            return instance;
        }

        public void RemoveMetadata(Object asset) {
            assets.RemoveAll(x => x.metadata == asset);
            UnityEditor.AssetDatabase.RemoveObjectFromAsset(asset);
            UnityEditor.EditorUtility.SetDirty(this);
            UnityEditor.AssetDatabase.SaveAssets();
        }
#endif
    }
}
