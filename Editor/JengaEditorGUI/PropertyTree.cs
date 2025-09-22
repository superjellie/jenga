using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {
        public static SerializedProperty PropertyTree(
            Rect rect, SerializedProperty property, string id
        ) {
            if (property == null) return null;

            JengaEditorGUI.BeginDataGroup(id);

            var rootDepth = property.depth;
            JengaEditorGUI.BeginForceCollapsedGroup();
            var oldIndent = EditorGUI.indentLevel;

            SerializedProperty result = null;
            var hiddenParents = new HashSet<SerializedProperty>();
            foreach (var subRef in property.Subreferences()) {
                EditorGUI.indentLevel = oldIndent + subRef.depth;
                var sub = subRef.property;

                if (hiddenParents.Contains(subRef.parent)) {
                    hiddenParents.Add(sub);
                    continue;
                }

                var subPath = sub.propertyPath
                    .Substring(subRef.parent?.propertyPath.Length + 1 ?? 0)
                    .Replace(".Array.data", "");

                var cntPath = subRef.parent != null
                    ? new GUIContent(subPath)
                    : new GUIContent(sub.displayName);

                var line = rect.LineCut(out rect);

                var evtType = Event.current.type;
                if (SelectionRect(line, "selected", sub.propertyPath))
                    result = sub;

                // unuse event
                Event.current.type = evtType;

                if (subRef.parent != null) {
                    JengaEditorGUI.BeginDataGroup(sub.propertyPath);
                    var foldout = GetDataValueOrDefault<bool>("foldout", true);

                    EditorGUI.indentLevel -= 1;
                    var newFoldout = EditorGUI.Foldout(line, foldout, "");
                    EditorGUI.indentLevel += 1;

                    if (newFoldout != foldout)
                        SetDataValue("foldout", newFoldout);

                    if (!newFoldout)
                        hiddenParents.Add(sub);

                    JengaEditorGUI.EndDataGroup();
                }

                EditorGUI.PropertyField(line, sub, cntPath, false);
            }

            EditorGUI.indentLevel = oldIndent;
            JengaEditorGUI.EndForceCollapsedGroup();
            JengaEditorGUI.EndDataGroup();
            return result;
        } 
    }
}
