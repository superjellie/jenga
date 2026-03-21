using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {

        public static bool SelectionRect<T>(
            Rect rect, string group, T id, bool multi = false
        ) {
            var style = EditorStyles.toolbarButton;
            var selected = GetDataValueOrDefault<List<T>>(
                group, default(List<T>)
            );

            if (selected == null)
                selected = new();

            var eq = selected.Contains(id);
            var doToggle = GUI.Toggle(rect, eq, "", style);
            
            if (!multi)
                selected.Clear();
            if (doToggle && !eq)
                { selected.Add(id); SetDataValue(group, selected); } 
            else if (!doToggle && eq)
                { selected.Remove(id); SetDataValue(group, selected); }

            return doToggle;
        }

    }
}
