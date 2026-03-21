using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;

namespace Jenga {
    [RegisterInToolSelector("Sticky Place")]
    [RequireTool(typeof(AssetsBrowserTool))]
    [RequireTool(typeof(SnappingTool))]
    [RequireTool(typeof(ShortcutsTool))]
    public class StickyPlaceTool : SceneTool {
        public override string category => "SceneTool";
        public override string title    => "Sticky Place";
        public override int order => 100;

        public AssetsBrowserTool assetsBrowser => 
            toolbox.GetActiveTool<AssetsBrowserTool>();

        public SnappingTool snapping => 
            toolbox.GetActiveTool<SnappingTool>();

        public ShortcutsTool shortcuts => 
            toolbox.GetActiveTool<ShortcutsTool>();

        public Quaternion rotation => Quaternion.Euler(euler);
        public Vector3 euler;
        public Vector3 scale  = Vector3.one;
        public Vector3 mirror = Vector3.one;
        public Vector3 offset;

        public float previewCameraEulerY = 0f;
        public float previewCameraEulerX = 0f;
        public float previewCameraDistance = 1f;
        public PivotMode pivotMode;

        public string assetPath = "Assets/Prefabs/";

        [Header("Colors")]
        public Color clrPlaceBounds  = Color.yellow;
        public Color clrPlacePreview = new Color(0f, 1f, 0f, .4f);

        GameObject asset;

        public enum PivotMode {
            Pivot, Center, BottomCenter, LeftCenter, OriginCorner 
        }

        public override void OnActivate() { 
            assetsBrowser.assetPath = assetPath;
        }

        public Matrix4x4 GetPivotModeMatrix(PivotMode mode, Bounds bounds) {
            var c = bounds.center;
            var s = bounds.size * .5f;
            return mode == PivotMode.Pivot ? Matrix4x4.identity
                : mode == PivotMode.Center 
                    ? Matrix4x4.Translate(c)
                : mode == PivotMode.LeftCenter 
                    ? Matrix4x4.Translate(new Vector3(c.x - s.x, c.y, c.z))
                : mode == PivotMode.BottomCenter 
                    ? Matrix4x4.Translate(new Vector3(c.x, c.y - s.y, c.z))
                : mode == PivotMode.OriginCorner 
                    ? Matrix4x4.Translate(
                        new Vector3(c.x - s.x, c.y - s.y, c.z - s.z)
                    )
                : Matrix4x4.identity;
        }

        public override void OnToolCustomGUI(
            EditorWindow win, SerializedObject so
        ) {
            var propRotation = so.FindProperty("euler");
            var propOffset = so.FindProperty("offset");
            var propScale = so.FindProperty("scale");
            var propMirror = so.FindProperty("mirror");
            var propPivotMode = so.FindProperty("pivotMode");
            var propAssetPath = so.FindProperty("assetPath");
            var propClrPlaceBounds = so.FindProperty("clrPlaceBounds");
            var propClrPlacePreview = so.FindProperty("clrPlacePreview");

            var viewWidth = EditorGUIUtility.currentViewWidth;

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(propAssetPath);

            if (EditorGUI.EndChangeCheck()) {
                assetsBrowser.assetPath = assetPath;
                assetsBrowser.isChanged = true;
            }

            if (viewWidth > 600f) EditorGUILayout.BeginHorizontal();

            if (viewWidth <= 600f) {
                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
            }

            MakeObjectPreview(
                so, win, GUILayout.Width(200f), GUILayout.Height(200f)
            );

            if (viewWidth <= 600f) {
                GUILayout.FlexibleSpace();
                EditorGUILayout.EndHorizontal();
            }
            
            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(propPivotMode);
            EditorGUILayout.PropertyField(propRotation);
            EditorGUILayout.PropertyField(propOffset);
            EditorGUILayout.PropertyField(propScale);
            EditorGUILayout.PropertyField(propMirror);
            EditorGUILayout.EndVertical();

            if (viewWidth > 600f) EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(propClrPlaceBounds);
            EditorGUILayout.PropertyField(propClrPlacePreview);
        }

        public override void OnSceneGUI(EditorWindow win) {
            SceneTools.PaintScene<(Matrix4x4, GameObject)>(
                canvas: (ray) => {

                    var wasHit = SceneTools.RaycastGameObject(ray, out var hit);
                    if (wasHit) {
                        var master = hit.gameObject;
                        // var masterGO = master != null ? master.gameObject : null;
                        // return (hit, masterGO != null);

                        var pos = hit.point;
                        var up = hit.normal;

                        var forward = Vector3.Cross(Vector3.right, up);
                        if (forward.magnitude < Mathx.SMALL)
                            forward = Vector3.forward;

                        var right = Vector3.Cross(forward, up);
                        var rot = Quaternion.LookRotation(forward, up);

                        return (
                            (Matrix4x4.TRS(pos, rot, Vector3.one), master), 
                            true
                        );
                    }

                    var plane = new Plane(Vector3.up, Vector3.zero);
                    var hitPlane = plane.Raycast(ray, out var enter);

                    if (hitPlane)
                        return (
                            (Matrix4x4.Translate(ray.GetPoint(enter)), null), 
                            true
                        );

                    return ((Matrix4x4.identity, null), false);
                },

                preview: (pair) => {
                    if (snapping == null) return;
                    if (asset == null) return;

                    
                    var mat = snapping.SnapInOwnRotation(
                        Matrix4x4.identity, pair.Item1
                    ) * GetOffsetMatrix();

                    SceneTools.DrawGameObjectPreview(
                        asset, mat, clrPlacePreview
                    );
                },

                paint: (pair) => {
                    if (snapping == null) return;
                    if (asset == null) return;

                    // var localMatrix = pair.Item1;
                    var parent = pair.Item2 != null ? 
                        pair.Item2.transform.parent : null;

                    var mat = snapping.SnapInOwnRotation(
                        Matrix4x4.identity, pair.Item1
                    ) * GetOffsetMatrix();

                    var go = SceneTools.SpawnGameObject(asset, mat, parent);
                    Selection.activeObject = go;
                }
            );
        } 

        public Matrix4x4 GetOffsetMatrix(bool noScale = false) {
            if (asset == null) return Matrix4x4.identity;
            var bounds = SceneTools.CalculateGameObjectBounds(asset);

            return Matrix4x4.Rotate(rotation)
                * (noScale ? Matrix4x4.identity 
                    : Matrix4x4.Scale(Vector3.Scale(mirror, scale)))
                * GetPivotModeMatrix(pivotMode, bounds).inverse
                * Matrix4x4.Translate(offset);
        }


        static GameObject go;
        static string lastGUID = null;
        
        void MakeObjectPreview(
            SerializedObject so, EditorWindow window, 
            params GUILayoutOption[] options
        ) {

            // if (assetsBrowser == null) return;

            if (assetsBrowser.selectedGUID != null
                && (go == null 
                    || lastGUID != assetsBrowser.selectedGUID
                )
            ) {
                DestroyImmediate(go);

                var pth = AssetDatabase.GUIDToAssetPath(assetsBrowser.selectedGUID);
                asset = AssetDatabase.LoadMainAssetAtPath(pth) as GameObject;

                if (asset != null) {
                    go = GameObject.Instantiate(asset) as GameObject;  
                    lastGUID = assetsBrowser.selectedGUID;
                }
            }


            if (ToolboxWindow.previewUtility != null && go != null) {

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.BeginVertical(GUILayout.Width(20f));

                if (GUILayout.Button("+")) {
                    var propDist = so.FindProperty("previewCameraDistance");
                    propDist.floatValue = Mathf.Clamp(
                        propDist.floatValue - .1f, .2f, 2f
                    );
                    so.ApplyModifiedProperties();
                    window.Repaint();
                }

                if (GUILayout.Button("-")) {
                    var propDist = so.FindProperty("previewCameraDistance");
                    propDist.floatValue = Mathf.Clamp(
                        propDist.floatValue + .1f, .2f, 2f
                    );
                    so.ApplyModifiedProperties();
                    window.Repaint();
                }

                EditorGUILayout.EndVertical();

                go.transform.localRotation = Quaternion.identity;
                go.transform.localPosition = Vector3.zero;
                go.transform.localScale = Vector3.one;
                var bounds = SceneTools.CalculateGameObjectBounds(go);

                var pivotMatrix = GetPivotModeMatrix(pivotMode, bounds);

                var matrix = Matrix4x4.Rotate(rotation)
                    * pivotMatrix.inverse
                    * Matrix4x4.Translate(offset);

                var rect = GUILayoutUtility.GetRect(
                    new GUIContent(""), 
                    GUI.skin.box, 
                    options
                );

                ToolboxWindow.previewUtility.BeginPreview(
                    new Rect(0, 0, 200f, 200f), GUIStyle.none
                );
                var cam = ToolboxWindow.previewUtility.camera;
                ToolboxWindow.previewUtility.AddSingleGO(go);
                go.transform.localPosition = matrix.GetPosition();
                go.transform.localRotation = matrix.rotation;
                cam.Render();

                var oldHandlesMatrix = Handles.matrix;
                var oldHandlesColor  = Handles.color;
                Handles.SetCamera(ToolboxWindow.previewUtility.camera);

                Handles.matrix = matrix;
                Handles.color = clrPlaceBounds;  
                Handles.DrawWireCube(bounds.center, bounds.size);

                Handles.matrix = Matrix4x4.identity;
                Handles.color = Color.green;
                Handles.DrawLine(Vector3.zero, Vector3.up, 2f);
                Handles.color = Color.blue;
                Handles.DrawLine(Vector3.zero, Vector3.forward, 2f);
                Handles.color = Color.red;
                Handles.DrawLine(Vector3.zero, Vector3.right, 2f);

                Handles.matrix = oldHandlesMatrix;
                Handles.color  = oldHandlesColor;

                CalculateCameraPosition(cam, bounds, matrix);

                ToolboxWindow.previewUtility.EndAndDrawPreview(rect);

                // var screenRect = GUIUtility.GUIToScreenRect(rect);
                if (Event.current.type == EventType.MouseDrag
                    && rect.Contains(Event.current.mousePosition)
                ) {
                    var propX = so.FindProperty("previewCameraEulerX");
                    var propY = so.FindProperty("previewCameraEulerY");
                    propX.floatValue -= Event.current.delta.y;
                    propY.floatValue += Event.current.delta.x;
                    so.ApplyModifiedProperties();
                    window.Repaint();
                }


                EditorGUILayout.EndHorizontal();
            }
        }

        public void CalculateCameraPosition(
            Camera cam, Bounds enclose, Matrix4x4 matrix
        ) {

            float eulerY = previewCameraEulerY;
            float eulerX = previewCameraEulerX;
            float distMul = previewCameraDistance;

            Quaternion rot = Quaternion.Euler(eulerX, eulerY, 0f);

            var size = matrix 
                * new Vector4(enclose.size.x, enclose.size.y, enclose.size.z, 0f);
            var pos = matrix 
                * new Vector4(
                    enclose.center.x, enclose.center.y, enclose.center.z, 1f
                );
            var pos3 = new Vector3(pos.x, pos.y, pos.z) / pos.w;

            float hdiam = size.magnitude;
            float hfov  = cam.fieldOfView * Mathf.Deg2Rad / 2f;

            float distance = hdiam / Mathf.Tan(hfov) * distMul;

            Vector3 direction = rot * Vector3.forward;

            cam.transform.position = pos3 + direction * distance;
            cam.transform.LookAt(pos3);
        }

        public override void OnUpdate(EditorWindow win) {
            if (shortcuts == null) return;
            
            shortcuts.SetShortcut(
                "StickyPlaceTool/Rotate-Y", this.GetType(), 
                EventModifiers.Shift, KeyCode.None, KeyCode.Q, "Rotate -Y",
                () => RotatePlacement(Vector3.down)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Rotate+Y", this.GetType(), 
                EventModifiers.Shift, KeyCode.None, KeyCode.E, "Rotate +Y",
                () => RotatePlacement(Vector3.up)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Rotate+X", this.GetType(),
                EventModifiers.Shift, KeyCode.None, KeyCode.D, "Rotate +X",
                () => RotatePlacement(Vector3.right)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Rotate-X", this.GetType(), 
                EventModifiers.Shift, KeyCode.None, KeyCode.A, "Rotate -X",
                () => RotatePlacement(Vector3.left)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Rotate+Z", this.GetType(), 
                EventModifiers.Shift, KeyCode.None, KeyCode.C, "Rotate +Z",
                () => RotatePlacement(Vector3.forward)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Rotate-Z", this.GetType(), 
                EventModifiers.Shift, KeyCode.None, KeyCode.Z, "Rotate -Z",
                () => RotatePlacement(Vector3.back)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/MirrorY", this.GetType(),
                EventModifiers.Shift, KeyCode.None, KeyCode.W, "Mirror Y",
                () => MirrorPlacement(new Vector3(1f, -1f, 1f))
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/MirrorX", this.GetType(),
                EventModifiers.Shift, KeyCode.None, KeyCode.S, "Mirror X",
                () => MirrorPlacement(new Vector3(-1f, 1f, 1f))
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/MirrorZ", this.GetType(),
                EventModifiers.Shift, KeyCode.None, KeyCode.X, "Mirror Z",
                () => MirrorPlacement(new Vector3(1f, 1f, -1f))
            ); 


            shortcuts.SetShortcut(
                "StickyPlaceTool/Offset+X", this.GetType(),
                EventModifiers.None, KeyCode.P, KeyCode.D, "Offset +X",
                () => OffsetPlacement(Vector3.right)
            ); 


            shortcuts.SetShortcut(
                "StickyPlaceTool/Scale+", this.GetType(), 
                EventModifiers.Shift, KeyCode.None, KeyCode.Equals, "Scale +",
                () => ScalePlacement(1f)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Scale-", this.GetType(), 
                EventModifiers.Shift, KeyCode.None, KeyCode.Minus, "Scale -",
                () => ScalePlacement(-1f)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Offset+Y", this.GetType(), 
                EventModifiers.None, KeyCode.P, KeyCode.E, "Offset +Y",
                () => OffsetPlacement(Vector3.up)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Offset-Y", this.GetType(), 
                EventModifiers.None, KeyCode.P, KeyCode.Q, "Offset placement -Y",
                () => OffsetPlacement(Vector3.down)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Offset+Z", this.GetType(), 
                EventModifiers.None, KeyCode.P, KeyCode.W, "Offset placement +Z",
                () => OffsetPlacement(Vector3.forward)
            ); 

            shortcuts.SetShortcut(
                "StickyPlaceTool/Offset-Z", this.GetType(), 
                EventModifiers.None, KeyCode.P, KeyCode.S, "Offset placement -Z",
                () => OffsetPlacement(Vector3.back)
            ); 

        }

        void RotatePlacement(Vector3 step) {
            if (snapping == null) return;
            Undo.RecordObject(this, "Rotate placement");
            euler += Vector3.Scale(snapping.eulerStep, step);    
        }

        void OffsetPlacement(Vector3 step) {
            if (snapping == null) return;
            Undo.RecordObject(this, "Offset placement");
            offset += Vector3.Scale(snapping.step, step);    
        }

        void ScalePlacement(float step) {
            if (snapping == null) return;
            Undo.RecordObject(this, "Scale placement");
            scale += snapping.scaleStep * step;    
        }

        void MirrorPlacement(Vector3 axes) {
            // if (snapping == null) return;
            Undo.RecordObject(this, "Mirror placement");
            mirror = Vector3.Scale(mirror, axes);    
        }

        public override void OnDeactivate() {
            if (go != null)
                DestroyImmediate(go);
        }
    }
}

