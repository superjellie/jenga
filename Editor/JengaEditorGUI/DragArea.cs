using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {
    
        static string draggedID = null;
        static bool isDragging => draggedID != null;
        public static bool 
        DragArea(Rect area, string id, MouseCursor cursor) {
            var evt = Event.current;

            var mouseInArea = area.Contains(evt.mousePosition);
            // Debug.Log(mouseInArea);

            if (!isDragging && evt.type == EventType.MouseDown && mouseInArea)
                draggedID = id;

            else if (isDragging && evt.type == EventType.MouseUp) {
                Event.current.Use();
                draggedID = null;
            }

            if (draggedID == id && evt.isMouse)
                Event.current.Use();

            if (draggedID == id || mouseInArea)
                EditorGUIUtility.AddCursorRect(area, cursor);

            return draggedID == id;
        }

    }
}
