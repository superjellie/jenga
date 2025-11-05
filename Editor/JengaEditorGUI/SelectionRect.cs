using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {

        public static bool SelectionRect<T>(Rect rect, string group, T id) {
            var style = EditorStyles.toolbarButton;
            var selected = GetDataValueOrDefault<T>(group, default(T));

            var eq = object.Equals(id, selected);
            
            var doToggle = GUI.Toggle(rect, eq, "", style);
            if (doToggle && !eq) SetDataValue(group, id);

            return doToggle;
        }

    }
}
