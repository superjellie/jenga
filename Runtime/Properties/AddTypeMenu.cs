using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jenga {

#if UNITY_EDITOR
    public struct TypeMenuEntry {
        public System.Type type;
        public int order;
        public string pathToSource;
        public int sourceLineNumber;
    }
#endif

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class AddTypeMenuAttribute : System.Attribute {

#if UNITY_EDITOR
        public static PathTree<TypeMenuEntry> registry = new();

        public TypeMenuEntry reg;
        public string path;
#endif

        [System.Obsolete]
        public AddTypeMenuAttribute(
            System.Type typeFamily, string path, int order = 1000,
            [CallerFilePath] string pathToSource = null,
            [CallerLineNumber] int sourceLineNumber = 1
        ) {
            
        }

        public AddTypeMenuAttribute(
            string path, int order = 1000,
            [CallerFilePath] string pathToSource = null,
            [CallerLineNumber] int sourceLineNumber = 1
        ) {
#if UNITY_EDITOR
            this.path = path;
            reg.order = order;
            reg.sourceLineNumber = sourceLineNumber;
            // Debug.Log(pathToSource);
            reg.pathToSource = pathToSource;
#endif
        }

#if UNITY_EDITOR
        static AddTypeMenuAttribute() {
            var types = TypeCache.GetTypesWithAttribute<AddTypeMenuAttribute>();
            foreach (var type in types) 
                if (type.TryGetAttribute<AddTypeMenuAttribute>(out var atm)) {
                    atm.reg.type = type;
                    registry.Add(atm.path, atm.reg);
                }
        }
#endif

    }
#if UNITY_EDITOR

    public static class TypeMenuEntryExtensions {
        public static IEnumerable<(string, TypeMenuEntry)> 
        GetWrappers(this PathTree<TypeMenuEntry> tree) {
            foreach (var (path, entry) in tree.Walk()) 
                if (entry.type
                        .TryGetFieldWithAttribute<WrapperAttribute>(out var _))
                    yield return (path, entry);
        }

        public static void EditScript(this TypeMenuEntry entry) {
            var p2s = entry.pathToSource;
            var pi = UnityEditor.PackageManager.PackageInfo.FindForAssetPath(p2s);

            var path = pi == null 
                ? Path.Combine("Assets", 
                    Path.GetRelativePath(Application.dataPath, p2s))
                : Path.Combine("Packages/" + pi.name, 
                    Path.GetRelativePath(pi.resolvedPath, p2s));

            var asset = AssetDatabase.LoadMainAssetAtPath(path);

            if (asset != null)
                AssetDatabase.OpenAsset(asset, entry.sourceLineNumber);
        }
    }

#endif
}