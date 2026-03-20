using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {

        static Stack<Matrix4x4> guiMatrixStack = new();

        public static void 
        BeginTransformedGroup(Matrix4x4 transform) {
            guiMatrixStack.Push(GUI.matrix);
            GUI.matrix *= transform;
        }

        public static void EndTransformedGroup() {
            GUI.matrix = guiMatrixStack.Pop();
        }

        public static void
        BeginRectViewGroup(Rect guiRect, Rect viewRect) {
            // GUI.BeginGroup(guiRect);
            
            GUI.BeginClip(viewRect, viewRect.position, Vector2.zero, false);

            var matrix = Matrix4x4.TRS(
                -3f * viewRect.position + guiRect.position,
                // Vector3.zero,
                Quaternion.identity,
                Vector3.one
            );
            BeginTransformedGroup(matrix);
        }

        public static void EndRectViewGroup() {
            EndTransformedGroup();
            GUI.EndClip();
        }

    }
}
