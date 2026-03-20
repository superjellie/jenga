using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {
    
        static Stack<Rect> draggableViewAreas = new();

        public static void BeginDraggableViewGroup(
            Rect area, Texture2D background, string dataID
        ) {

            BeginDataGroup(dataID);
            var scale = GetDataValueOrDefault("scale", 1f);
            var pos   = GetDataValueOrDefault("pos", Vector2.zero);
            scale = Mathf.Clamp(scale, .1f, 10f);

            var viewRect = new Rect(
                pos.x, pos.y, 
                area.width, area.height
            );

            BeginRectViewGroup(area, viewRect);

            // GUI.DrawTextureWithTexCoords(
            //     viewRect, background, 
            //     new Rect(0f, 0f, 
            //         viewRect.width / background.width * 5f, 
            //         viewRect.height / background.height * 5f
            //     )
            // );

            GUI.Box(new Rect(0f, 0f, 100f, 100f), "Zero");
            GUI.Box(new Rect(viewRect.x, viewRect.y, 100f, 100f), "View");

            draggableViewAreas.Push(viewRect);
        }

        public static void 
        EndDraggableViewGroup() {
            var area = draggableViewAreas.Pop();
            var evt = Event.current;

            var mouseInArea = area.Contains(evt.mousePosition);
            EditorGUIUtility.AddCursorRect(area, MouseCursor.Pan);

            if (evt.type == EventType.MouseDrag && mouseInArea) {
                var scale = GetDataValueOrDefault("scale", 1f);
                var pos   = GetDataValueOrDefault("pos", Vector2.zero);
                pos += evt.delta * scale;

                // pos = Vector2.zero;
                SetDataValue("pos", pos);
                evt.Use();
            } else if (evt.type == EventType.ScrollWheel && mouseInArea) {
                var scale = GetDataValueOrDefault("scale", 1f);
                for (int i = 0; i < evt.delta.y; ++i) scale *= 1.1f; 
                for (int i = 0; i > evt.delta.y; --i) scale /= 1.1f;

                scale = Mathf.Clamp(scale, .1f, 10f);
                SetDataValue("scale", scale);
                evt.Use();
            }

            EndRectViewGroup();
            EndDataGroup();
        }

    }
}
