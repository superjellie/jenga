using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static partial class JengaEditorGUI {
        public static void 
        Line(Vector2 pA, Vector2 pB, Color color, Texture2D texture = null) {
            if (Event.current.type != EventType.Repaint) return;
            Drawing.DrawLine(pA, pB, color, 1f, true, texture);
        }

        // Line with an equation dy/dx = s / a
        // https://math.stackexchange.com/questions/3557767/how-to-construct-a-catenary-of-a-specified-length-through-two-specified-points
        public static void Catenary(
            Vector2 p1, Vector2 p2, float length, Color color, 
            Texture2D texture = null
        ) { 
            var dx = p2.x - p1.x;
            var dy = -p2.y + p1.y;

            var xm = .5f * (p1.x + p2.x);
            var ym = -.5f * (p1.y + p2.y);

            var dx2 = dx * dx;
            var dy2 = dy * dy;
            var len2 = length * length;

            if (len2 <= dx2 + dy2 || dx2 < .01f * len2) goto TOO_CLOSE;

            var r = Mathf.Sqrt(len2 - dy2) / dx;

            // Newton's iteration
            var A0 = r < 3f ? Mathf.Sqrt(6f * r - 6f)
                : Mathf.Log(2f * r) + Mathf.Log(Mathf.Log(2f * r));
            var A = Intf.Newton(A => A * r - Mathx.Sinh(A), A0, 5);

            //
            var a = dx / (2f * A);
            var b = xm - a * Mathx.Atanh(dy / length);
            var c = -p1.y - a * Mathx.Cosh((p1.x - b) / a);

            //
            var nSteps = Mathf.Max(Mathf.Ceil(Mathf.Abs(dx) / 50f), 5);
            // Debug.Log($"a = {a}, b = {b}, c = {c}, nSteps = {nSteps}");
            Vector2 p = p1;
            for (int i = 0; i < nSteps - 1; ++i) {
                var xn = p1.x + (i + 1) * dx / nSteps;
                var yn = a * Mathx.Cosh((xn - b) / a) + c;
                var pn = new Vector2(xn, -yn);

                Line(p, pn, color, texture); p = pn;
            }

            Line(p, p2, color, texture);
            return;
        TOO_CLOSE:
            Line(p1, p2, color, texture);
        }
    }
}
