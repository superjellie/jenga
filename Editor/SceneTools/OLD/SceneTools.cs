using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Search;
using UnityEditor.Callbacks;
using Controls = UnityEditor.IMGUI.Controls;

namespace Jenga {

    public static class SceneTools {
        public static GUIContent iconPointer 
            = EditorGUIUtility.IconContent("d_Grid.Default");
        public static GUIContent iconPrefab
            = EditorGUIUtility.IconContent("d_Prefab Icon");
        public static GUIContent iconFocus
            = EditorGUIUtility.IconContent("d_Animation.FilterBySelection");

        public static void DrawWindow(
            Vector2 anchor, Vector2 offset, Vector2 size, 
            string title, System.Action onGUI
        ) {
            Handles.BeginGUI();

            var view = SceneView.currentDrawingSceneView.cameraViewport;
            var origin = new Vector2(anchor.x * view.width, anchor.y * view.height);
            var rect = new Rect(origin + offset, size);

            GUILayout.Window(
                GUIUtility.GetControlID(new GUIContent(title), FocusType.Passive), 
                rect, (x) => onGUI(), title
            );

            Handles.EndGUI();
        }

        public static void BeginWorldGroup(Vector3 pos, Vector2 size) {
            Handles.BeginGUI();
            var scr = HandleUtility.WorldToGUIPoint(pos);
            // var scale = HandleUtility.GetHandleSize(pos);
            GUILayout.BeginArea(new Rect(scr - .5f * size, size));
        }

        public static void EndWorldGroup() {
            GUILayout.EndArea();
            Handles.EndGUI();
        }


        public static void PaintScene<T>(
            System.Func<Ray, (T, bool)> canvas, System.Action<T> preview,
            System.Action<T> paint, bool allowDrag = false 
        ) {
            var rect = SceneView.currentDrawingSceneView.cameraViewport;
            var mousePos = Event.current.mousePosition;
            if (!rect.Contains(mousePos)) return;

            var ray = HandleUtility.GUIPointToWorldRay(mousePos);
            var (target, hit) = canvas(ray);

            if (Event.current.type == EventType.Layout) {
                HandleUtility.AddDefaultControl(
                    GUIUtility.GetControlID(FocusType.Passive)
                );
            }

            if (hit) {
                preview(target);

                if (Event.current.type == EventType.MouseDown 
                    && Event.current.button == 0
                    || allowDrag && Event.current.type == EventType.MouseDrag
                    && Event.current.button == 0) {
                    paint(target);
                    Event.current.Use();
                }
            }
        }

        [System.Serializable]
        public class SearchData {
            public int selected = -1;
            public GUIContent[] previews = {};
            public Object[]     targets = {};
            public SearchItem[] items = {};
            public Vector2 scrollPosition;

            public Controls.SearchField field;
            public string query = "";
            public bool isUpdating = false;
            public bool hasSearched = false;
        }

        public static Object DrawSearchSelector(
            SearchData ctx, string searchSetup, int columns
        ) {
            if (ctx.field == null) ctx.field = new();

            GUILayout.BeginHorizontal();
            var oldQuery = ctx.query;
            var newQuery = ctx.field.OnToolbarGUI(ctx.query);
            // var newQuery = oldQuery;
            ctx.query = newQuery;

            {
                var style = new GUIStyle(GUI.skin.button);
                style.padding = new RectOffset(2, 2, 2, 2);
                var toggled = GUILayout.Toggle(
                    ctx.selected == -1, iconPointer, style,
                    GUILayout.Width(15f), GUILayout.Height(15f)
                );
                if (toggled) ctx.selected = -1;
            }


            if (newQuery != oldQuery || !ctx.hasSearched) {
                ctx.isUpdating = true;
                SearchService.Request(
                    searchSetup + " " + ctx.query, (ctxx, items) => {
                        ctx.previews = new GUIContent[items.Count];
                        ctx.targets = new Object[items.Count];
                        ctx.items = new SearchItem[items.Count];
                        for (int i = 0; i < items.Count; ++i) {
                            ctx.items[i] = items[i];
                            ctx.previews[i] = new GUIContent(
                                ctx.items[i].GetPreview(
                                    ctxx, new Vector2(50f, 50f), 
                                    FetchPreviewOptions.Large, true
                                )
                            );
                            ctx.targets[i] = items[i].ToObject();
                        }
                        ctx.isUpdating = false;
                    }
                );
                ctx.hasSearched = true;
            }

            EditorGUI.BeginDisabledGroup(ctx.isUpdating);

            GUILayout.EndHorizontal();
            ctx.scrollPosition = GUILayout.BeginScrollView(ctx.scrollPosition);

            for (int i = 0; i < ctx.items.Length; ++i) {
                if (ctx.items[i] == null) continue;
                ctx.previews[i] = new GUIContent(ctx.items[i].preview);
                if (ctx.previews[i] == null)
                    ctx.previews[i] = iconPrefab;
            }

            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                style.fixedHeight = 50f;
                style.fixedWidth  = 50f;

                ctx.selected = GUILayout.SelectionGrid(
                    ctx.selected, ctx.previews, columns, style: style
                );
            }

            var target = ctx.targets.Length > 0 && ctx.selected >= 0? 
                ctx.targets[ctx.selected % ctx.targets.Length]
                : null;

            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            if (target != null) {
                GUILayout.Label(target.name);
                GUILayout.FlexibleSpace();
                var assetPath = PrefabUtility
                    .GetPrefabAssetPathOfNearestInstanceRoot(target);
                if (assetPath != null && GUILayout.Button("Open")) {
                    PrefabStageUtility.OpenPrefab(assetPath);
                }

                if (assetPath != null && GUILayout.Button("Locate")) {
                    EditorUtility.FocusProjectWindow(); 
                    Selection.activeObject = AssetDatabase
                        .LoadAssetAtPath(assetPath, typeof(Object));
                }
            }
            GUILayout.EndHorizontal(); 

            EditorGUI.EndDisabledGroup();

            return target;
        }


        // // Hacking into unity internal method for mesh raycasting
        delegate bool IntersectRayMeshDelegate(
            Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit
        );

        static readonly IntersectRayMeshDelegate intersectRayMeshFunc
            = (IntersectRayMeshDelegate)
            typeof(UnityEditor.HandleUtility)
                .GetMethod("IntersectRayMesh", 
                    System.Reflection.BindingFlags.Static 
                    | System.Reflection.BindingFlags.NonPublic)
                .CreateDelegate(typeof(IntersectRayMeshDelegate));

        public static bool IntersectRayMesh(
            Ray ray, Mesh mesh, Matrix4x4 matrix, out RaycastHit hit
        ) => intersectRayMeshFunc(ray, mesh, matrix, out hit);

        static GameObject lastRaycastedGO = null;
        public static bool PickGameObject(Ray ray, out GameObject go) {
            go = lastRaycastedGO;
            if (Event.current.type != EventType.MouseMove 
                && Event.current.type != EventType.MouseDrag) {
                return lastRaycastedGO != null;
            }

            var pos = HandleUtility.WorldToGUIPoint(
                ray.origin + ray.direction * 100f
            );

            lastRaycastedGO = HandleUtility.PickGameObject(pos, false);
            go = lastRaycastedGO;
            return go != null;
        }

        public class MeshRaycastHit {
            public Vector3 point;
            public Vector3 normal;
            public MeshFilter meshFilter;

            public GameObject gameObject => meshFilter.gameObject;
            public Transform transform => meshFilter.transform;
        }

        public static bool RaycastGameObject(Ray ray, out MeshRaycastHit hit) {

            if (PickGameObject(ray, out var go)) {
                var filter = go.GetComponent<MeshFilter>();
                if (filter != null) {
                    var mesh = filter.sharedMesh;
                    var matrix = go.transform.localToWorldMatrix;
                    var wasHit = IntersectRayMesh(ray, mesh, matrix, out var rhit);
                    hit = new MeshRaycastHit() {
                        point = rhit.point,
                        normal = rhit.normal,
                        meshFilter = filter
                    };
                    return wasHit;
                }
            }

            hit = new MeshRaycastHit();
            return false;
        }

        public static Bounds CalculateGameObjectBounds(GameObject go) {
            var bounds = new Bounds(go.transform.position, Vector3.zero);
            foreach (var renderer in go.GetComponentsInChildren<Renderer>()) 
                bounds.Encapsulate(renderer.bounds);
            return bounds;
        }

        static Material previewMaterial;
        public static void DrawGameObjectPreview(
            GameObject go, Matrix4x4 matrix, Color color,
            bool showBounds = true
        ) {
            // if (Event.current.type != EventType.Repaint)
            //     return;
            if (previewMaterial == null) 
                previewMaterial = new Material(Shader.Find("UI/Unlit/Transparent"));
            
            previewMaterial.SetColor("_Color", color);

            foreach (var renderer in go.GetComponentsInChildren<Renderer>()) {
                // var prms = new RenderParams(mat);
                var mesh = renderer.GetComponent<MeshFilter>().sharedMesh;
                previewMaterial.SetPass(0);

                var trs = matrix * renderer.transform.localToWorldMatrix;
                Graphics.DrawMeshNow(mesh, trs);
            }

            if (showBounds) {
                var bounds = CalculateGameObjectBounds(go);
                var oldMatrix = Handles.matrix;
                Handles.matrix = matrix;
                Handles.DrawWireCube(bounds.center, bounds.size);
                Handles.matrix = oldMatrix;
            }
        }

        public static void DrawGameObjectPreview(
            GameObject go, Vector3 pos, Quaternion rotation, Color color,
            bool showBounds = true
        ) => DrawGameObjectPreview(
            go, Matrix4x4.TRS(pos, rotation, Vector3.one), color,
            showBounds: showBounds
        );
        

        public static GameObject SpawnGameObject(
            GameObject prefab, Matrix4x4 matrix, Transform parent
        ) {
            var ps = PrefabStageUtility.GetCurrentPrefabStage();

            if (parent == null && ps != null)
                parent = ps.prefabContentsRoot.transform;

            Undo.IncrementCurrentGroup();

            var o = PrefabUtility.InstantiatePrefab(prefab.gameObject);

            var go = o as GameObject;
            Undo.RegisterCreatedObjectUndo(o, $"Instantiate {go.name}");

            Undo.RegisterCompleteObjectUndo(go, $"Init {go.name}");
            var instTRS = matrix * prefab.transform.localToWorldMatrix;
            go.transform.SetFromLocalMatrix(instTRS, true, false, 1f);

            Undo.SetTransformParent(go.transform, parent, $"Parent {go.name}");
            Undo.SetCurrentGroupName($"Spawn {go.name}");
            return go;

        }

        public static GameObject SpawnGameObject(
            GameObject prefab, 
            Vector3 position, Quaternion rotation,
            Transform parent
        ) => SpawnGameObject(prefab, position, rotation, Vector3.one, parent);

        public static GameObject SpawnGameObject(
            GameObject prefab, 
            Vector3 position, Quaternion rotation, Vector3 scale,
            Transform parent
        ) => SpawnGameObject(
            prefab, Matrix4x4.TRS(position, rotation, scale), parent
        );




        public static (Vector3 pos, Quaternion rot) SnapToMesh(
            SceneTools.MeshRaycastHit hit, Vector3 snap, Vector3 offset, 
            Quaternion offsetRotation
        ) {

            var pos = hit.point;

            var up = hit.normal;
            var forward = Vector3.Cross(Vector3.right, up);
            if (forward.magnitude < Mathx.SMALL)
                forward = Vector3.forward;
            var right = Vector3.Cross(forward, up);

            var rot = Quaternion.LookRotation(forward, up);
            var invRot = Quaternion.Inverse(rot);

            var posInMeshCoords = invRot * pos;
            var posInMeshSnapped = 
                Snapping.Snap(posInMeshCoords, snap, SnapAxis.All);

            return (rot * (posInMeshSnapped + offset), rot * offsetRotation);
        }


        // Private
    }
}