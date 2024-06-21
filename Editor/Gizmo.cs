using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static class Gizmo  {

        public static Material matGrid = null;

        public static bool ShouldDrawGizmos<T>() {
            if (!Handles.ShouldRenderGizmos()) return false;
            if (GizmoUtility.TryGetGizmoInfo(typeof(T), out var info)) 
                return info.gizmoEnabled;
            return true;
        }

        public static void DrawGrid(
            Vector3 origin, Vector3 axisA, Vector3 axisB
        ) {
            if (matGrid == null) return;
            matGrid.SetVector("origin", origin);
            matGrid.SetVector("axisA", axisA);
            matGrid.SetVector("axisB", axisB);
            matGrid.SetColor("lineColor", Color.black);
            matGrid.SetFloat("lineWidth", 1f);
            matGrid.SetColor("bigLineColor", Color.black);
            matGrid.SetFloat("bigLineWidth", 2f);
            matGrid.SetInteger("bigLineFreq", 5);
            matGrid.SetColor("tileColor", new Color(0f, 0f, 0f, 0f));
            matGrid.SetColor("fogColor", new Color(0f, 0f, 0f, 0f));
            matGrid.SetFloat("fogDensity", .8f);
            matGrid.SetFloat("fogStrength", .04f);
            GL.PushMatrix();
            matGrid.SetPass(0);
            GL.Begin(GL.QUADS);
            GL.Vertex(origin + Mathx.Vec3( 10000f, 0f,  10000f));
            GL.Vertex(origin + Mathx.Vec3(-10000f, 0f,  10000f));
            GL.Vertex(origin + Mathx.Vec3(-10000f, 0f, -10000f));
            GL.Vertex(origin + Mathx.Vec3( 10000f, 0f, -10000f));
            GL.End();
            GL.PopMatrix();
        }

        // Set-ups
        // Inits
        [InitializeOnLoadMethod]
        static void Init() {
            /* Init materials */ {
                var shader = Shader.Find("Jenga/RectGrid");
                matGrid = new Material(shader);
            }
        }
    }
}
