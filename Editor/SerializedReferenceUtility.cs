using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    public struct SerializedReferenceLink {
        public static SerializedReferenceLink Null = new() 
            { instanceID = 0, referenceID = -2 };

        public int instanceID;
        public long referenceID;
        public string subPath;
        public string refPath;

        public string path => string.IsNullOrEmpty(refPath)
            ? subPath
            : $"{refPath}.{subPath}";

        public override bool Equals(object other)
            => other is SerializedReferenceLink link 
            && Equals(link);

        public bool Equals(SerializedReferenceLink other)
            => instanceID == other.instanceID 
            && referenceID == other.referenceID
            && subPath == other.subPath;

        public static bool operator==(
            SerializedReferenceLink a, SerializedReferenceLink b
        ) => a.Equals(b);
        public static bool operator!=(
            SerializedReferenceLink a, SerializedReferenceLink b
        ) => !a.Equals(b);

        public override int GetHashCode() 
            => (instanceID, referenceID, subPath).GetHashCode();

        public bool IsNull() => instanceID == 0 || referenceID < 0;


        static Dictionary<int, SerializedObject> soCache = new();

        public SerializedProperty GetProperty() {

            if (soCache.TryGetValue(instanceID, out var so)) {
                so.Update();
                return so.FindProperty(path);
            }

            if (IsNull()) return null;
            var o = EditorUtility.InstanceIDToObject(instanceID);
            so = new SerializedObject(o);
            soCache[instanceID] = so;
            return so.FindProperty(path);
        }
    }

    public static class SerializedReferenceUtility {

        public static IEnumerable<SerializedProperty> 
        GetAllLinkedProperties(SerializedObject so, long id) {
            var link = new SerializedReferenceLink() { 
                instanceID = so.targetObject.GetInstanceID(), 
                referenceID = id 
            };

            if (linkToPaths.TryGetValue(link, out var cache))
                foreach (var path in cache.paths) 
                    yield return so.FindProperty(path);

        }

        public static int 
        GetLinkedPropertiesCount(SerializedObject so, long id) {
            var link = new SerializedReferenceLink() { 
                instanceID = so.targetObject.GetInstanceID(), 
                referenceID = id 
            };

            return link.GetLinkedPropertiesCount();
        }


        public static int 
        GetLinkedPropertiesCount(this SerializedReferenceLink link) {
            if (linkToPaths.TryGetValue(link, out var cache))
                return cache.paths.Count;
            return 0;
        }

        public struct CachedLinkedProperties {
            public List<string> paths;
        }

        static Dictionary<SerializedReferenceLink, CachedLinkedProperties>
            linkToPaths = new();

        public static void UpdateCachedLinks(SerializedObject so) {
            var it = so.GetIterator();
            var instanceID = so.targetObject.GetInstanceID();

            foreach (var (link, cache) in linkToPaths)
                if (link.instanceID == instanceID)
                    cache.paths.Clear();

            do {
                if (!it.IsManagedReference()) continue;
                var refId = it.managedReferenceId;
                if (refId <= 0) continue;

                var path = it.propertyPath;
                var link = new SerializedReferenceLink() 
                    { instanceID = instanceID, referenceID = refId };

                if (linkToPaths.TryGetValue(link, out var cache))
                    cache.paths.Add(path);
                else 
                    linkToPaths.Add(link, new() { paths = new() { path } });


            } while (it.Next(true));

        }

    }
}
