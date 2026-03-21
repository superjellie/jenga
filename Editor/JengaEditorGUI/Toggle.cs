using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    public static partial class JengaEditorGUI {

        public static bool Toggle(Rect rect, string label, string name = null) {
            if (name == null) name = label;

            var value = JengaEditorGUI.GetDataValueOrDefault(name, false);
            value = EditorGUI.Toggle(rect, label, value);
            JengaEditorGUI.SetDataValue(name, value);
            return value;
        } 

        public static bool LayoutToggle(string label, string name = null) {
            var rect = GUILayoutUtility
                .GetRect(EditorGUIUtility.currentViewWidth - 10f, 20f);
            return Toggle(rect, label, name);
        } 


    }
}
