using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    public static class RectExtensions {

#if UNITY_EDITOR
        public static Rect LineCut(this Rect r) 
            => r.TopCut(UnityEditor.EditorGUIUtility.singleLineHeight);

        public static Rect IndentCut(this Rect r) 
            => r.RightCut(r.width - UnityEditor.EditorGUI.indentLevel * 10f);

        public static Rect LineCut(this Rect r, out Rect other) 
            => r.TopCut(UnityEditor.EditorGUIUtility.singleLineHeight, out other);
#endif

        public static Rect LeftCut(this Rect r, float amount) 
            => new(r.x, r.y, amount, r.height);

        public static Rect RightCut(this Rect r, float amount) 
            => new(r.x + r.width - amount, r.y, amount, r.height);

        public static Rect MoveLeft(this Rect r, float amount) 
            => new(r.x - amount, r.y, r.width, r.height);

        public static Rect MoveRight(this Rect r, float amount) 
            => new(r.x + amount, r.y, r.width, r.height);

        public static Rect MoveUp(this Rect r, float amount) 
            => new(r.x, r.y + amount, r.width, r.height);

        public static Rect MoveDown(this Rect r, float amount) 
            => new(r.x, r.y - amount, r.width, r.height);

        public static Rect TopCut(this Rect r, float amount) 
            => new Rect(r.x, r.y, r.width, amount);

        public static Rect BottomCut(this Rect r, float amount) 
            => new Rect(r.x, r.y + r.height - amount, r.width, amount);

        public static Rect LeftInverseCut(this Rect r, float amount) 
            => new(r.x + amount, r.y, r.width - amount, r.height);

        public static Rect RightInverseCut(this Rect r, float amount) 
            => new(r.x, r.y, r.width - amount, r.height);

        public static Rect TopInverseCut(this Rect r, float amount) 
            => new Rect(r.x, r.y + amount, r.width, r.height - amount);

        public static Rect BottomInverseCut(this Rect r, float amount) 
            => new Rect(r.x, r.y, r.width, r.height - amount);

        public static Rect LeftCut(this Rect r, float amount, out Rect other) {
            other = new(r.x + amount, r.y, r.width - amount, r.height);
            return LeftCut(r, amount);
        }

        public static Rect RightCut(this Rect r, float amount, out Rect other) {
            other = new(r.x, r.y, r.width - amount, r.height);
            return RightCut(r, amount);
        }

        public static Rect TopCut(this Rect r, float amount, out Rect other) {
            other = new(r.x, r.y + amount, r.width, r.height - amount);
            return TopCut(r, amount);
        }

        public static Rect BottomCut(this Rect r, float amount, out Rect other) {
            other = new(r.x, r.y, r.width, r.height - amount);
            return BottomCut(r, amount);
        }

        public static Rect Expand(this Rect r, float marginX, float marginY) {
            return new Rect(
                r.x - marginX, r.y - marginY, 
                r.width + 2f * marginX, r.height + 2f * marginY
            );
        }

        public static Rect Shrink(this Rect r, float marginX, float marginY) {
            return r.Expand(-marginX, -marginY);
        }

        public static Rect Copy(this Rect r) => r;
    }
}