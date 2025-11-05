using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {

        static GUIStyle propRefPlugStyle = null;
        public static bool layoutLeftPlugs = false;
        public static void PropertyReferencePlug(
            Rect rect, SerializedProperty property, GUIContent label = null
        ) {
            if (propRefPlugStyle == null) {
                propRefPlugStyle = new GUIStyle(EditorStyles.miniBoldLabel);
                propRefPlugStyle.richText = true;
                // propRefPlugStyle.alignment = TextAnchor.MiddleRight;
            }

            var line = rect.LineCut();
            var cntName = label == null 
                ? new GUIContent(property.displayName) : label;

            var rectName = line.LeftCut(
                Mathf.Min(EditorGUIUtility.labelWidth, line.width), 
                out var rectType
            );

            var rectLink = layoutLeftPlugs
                ? rectName.LeftCut(20f, out rectName)
                    .MoveRight(EditorGUI.indentLevel * 12f)
                : rectType.RightCut(20f, out rectType);

            var style = propRefPlugStyle;
            var borderRect = rectType.LeftCut(-2f);
            TypeMenuContext(rectType, property);

            // EditorGUI.HandlePrefixLabel(line, rectName, cntName);
            // EditorGUI.DrawRect(rectName.Shrink(2f, 2f), Color.blue);
            // EditorGUI.DrawRect(rectType.Shrink(2f, 2f), Color.magenta);
            // EditorGUI.DrawRect(borderRect, Color.gray);
            EditorGUI.LabelField(rectName, cntName);

            var type = property.GetCurrentType();

            if (type == null)
                GUI.Label(rectType, "<color=yellow>[NULL]</color>", style);
            else if (type.TryGetAttribute<AddTypeMenuAttribute>(out var atm))
                GUI.Label(rectType, atm.path, style);
            else 
                GUI.Label(rectType, "<color=blue>[UNKNOWN]</color>", style);

            LinkCablePlug(rectLink, property);
        }
    }
}
