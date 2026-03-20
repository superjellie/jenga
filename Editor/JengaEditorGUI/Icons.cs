using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Jenga {
    public static partial class JengaEditorGUI {

        public static string pathJengaEditor 
            = "Packages/com.github.superjellie.jenga/Editor";
        public static string pathIconEyeOff 
            = $"{pathJengaEditor}/Icons/eye_toggle_off.png";
        public static string pathIconEyeOn 
            = $"{pathJengaEditor}/Icons/eye_toggle_on.png";

        public static Texture2D iconEyeOff 
            = (Texture2D)EditorGUIUtility.Load(pathIconEyeOff);
        public static Texture2D iconEyeOn 
            = (Texture2D)EditorGUIUtility.Load(pathIconEyeOn);
        public static Texture2D iconEyeOnYellow 
            = iconEyeOn.MultiplyColor(Color.yellow);
        public static Texture2D iconEyeOnGreen
            = iconEyeOn.MultiplyColor(Color.green);
        public static Texture2D iconEyeOffBlue
            = iconEyeOff.MultiplyColor(Color.blue);
        

        static GUIStyle styEyeToggle_ = null;
        public static GUIStyle styEyeToggle { get {
            if (styEyeToggle_ == null) { 
                styEyeToggle_ = new(EditorStyles.toggle);  
                styEyeToggle_.name      = "jenga_eye_toggle";
                styEyeToggle_.normal    = new() { background = iconEyeOff }; 
                styEyeToggle_.active    = new() { background = iconEyeOff }; 
                styEyeToggle_.focused   = new() { background = iconEyeOff }; 
                styEyeToggle_.hover     = new() { background = iconEyeOff }; 
                styEyeToggle_.onNormal  = new() { background = iconEyeOnYellow }; 
                styEyeToggle_.onActive  = new() { background = iconEyeOnYellow }; 
                styEyeToggle_.onFocused = new() { background = iconEyeOnYellow }; 
                styEyeToggle_.onHover   = new() { background = iconEyeOnYellow }; 
                styEyeToggle_.fixedWidth = 24f;
                styEyeToggle_.fixedHeight = 21f;
                styEyeToggle_.border = new(0, 0, 0, 0);
                styEyeToggle_.overflow = new(0, 0, 4, -1);
            } 
            return styEyeToggle_;
        } }

    }
}
