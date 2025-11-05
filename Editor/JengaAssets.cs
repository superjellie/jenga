using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static class JengaAssets {

        public static string packagePath 
            = "Packages/com.github.superjellie.jenga/Editor";

        public static Texture2D texThread = null;
        public static Texture2D texPinFull = null;
        public static Texture2D texPinEmpty = null;


        static JengaAssets() {
            texThread = (Texture2D)AssetDatabase.LoadAssetAtPath(
                $"{packagePath}/Textures/thread.jpg", typeof(Texture2D));
            texPinFull = (Texture2D)AssetDatabase.LoadAssetAtPath(
                $"{packagePath}/Textures/pin_full.png", typeof(Texture2D));
            texPinEmpty = (Texture2D)AssetDatabase.LoadAssetAtPath(
                $"{packagePath}/Textures/pin_empty.png", typeof(Texture2D));
        }

    }
}
