using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {

        public static void SplitView(
            Rect position, string id,
            out Rect firstPane, out Rect secondPane,
            bool horizontal = false
        ) {
            JengaEditorGUI.BeginDataGroup(id);
            var leftOffset = GetDataValueOrDefault<float>("leftOffset", 100f);


            if (horizontal)
                firstPane = position.TopCut(leftOffset, out secondPane);
            else
                firstPane = position.LeftCut(leftOffset, out secondPane);

            var r = firstPane;
            var lineRect = horizontal
                ? new Rect(r.xMin, r.yMax - 1f, r.width, 2f) 
                : new Rect(r.xMax - 1f, r.y, 2f, r.height);
            // Debug.Log(lineRect); 

            EditorGUI.DrawRect(lineRect, Color.black);

            var rectDrag = lineRect.Expand(5f, 5f);
            var cursor = horizontal 
                ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal;
                
            if (DragArea(rectDrag, currentDataRoot, cursor)) 
                leftOffset = horizontal 
                    ? Event.current.mousePosition.y - position.y
                    : Event.current.mousePosition.x - position.x;

            leftOffset = Mathf.Clamp(leftOffset, 20f, position.width - 20f);

            SetDataValue("leftOffset", leftOffset);
            JengaEditorGUI.EndDataGroup();
        }
    }
}
