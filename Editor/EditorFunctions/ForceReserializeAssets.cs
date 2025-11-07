using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static class EditorFunctions {

        [MenuItem("Jenga/Force Reserialize Assets")]
        public static void ForceReserializeAssets() {
            AssetDatabase.ForceReserializeAssets();
        }
    }
}
