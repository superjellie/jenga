using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Jenga {
    public class ToolboxWindow : EditorWindow {

    	public static ToolboxWindow activeInstance = null;
        public static PreviewRenderUtility previewUtility;

    	public SceneToolbox toolbox = null;
        public bool debugView = false;
        public Vector2 scrollPosition;
        public float lastSaveTime = 0f;

        [MenuItem("Window/Jenga/Scene Tools")]
        public static void ShowWindow() {
            var win = GetWindow<ToolboxWindow>();
            win.titleContent = new GUIContent("Tools");
        }

        void OnBecameVisible() {
            activeInstance = this;
            UpdateToolbox();
        }

        void OnEnable() {
            UpdateToolbox();

            if (previewUtility != null)
                previewUtility.Cleanup();

            previewUtility = new(true);

            // Setup camera
            var cam = previewUtility.camera;
            cam.fieldOfView = 30f;
            cam.nearClipPlane = 0.01f;
            cam.farClipPlane = 1000f;
            cam.cameraType = CameraType.SceneView;
        }

        void OnDisable() {
            previewUtility.Cleanup();
            previewUtility = null;

            if (activeInstance != this) return;
        }

        public static void UpdateToolbox() {
            SceneToolWindow.RepaintAll();

            if (activeInstance == null) return;
            activeInstance.Repaint();

            if (activeInstance.toolbox == null) return;
            activeInstance.toolbox.UpdateActiveTools();
        }

        void OnGUI() {
        	var wasGUIEnabled = GUI.enabled;
        	if (activeInstance != this) GUI.enabled = false;

            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginChangeCheck();

            toolbox = (SceneToolbox)EditorGUILayout.ObjectField(
                "", toolbox, typeof(SceneToolbox), false
        	);

            if (EditorGUI.EndChangeCheck()) UpdateToolbox();

            var oldColor = GUI.backgroundColor;
            if (debugView) GUI.backgroundColor = Color.yellow;

            if (GUILayout.Button("Debug", GUILayout.Width(50f)))
                debugView = !debugView;

            GUI.backgroundColor = oldColor;
            EditorGUILayout.EndHorizontal();

            scrollPosition = EditorGUILayout.BeginScrollView(
                scrollPosition, GUILayout.ExpandHeight(true)
            );

            if (toolbox != null && !debugView) {
                foreach (var tool in toolbox.activeTools)
                    if (tool != null)
                        tool.OnToolGUI(this, new SerializedObject(tool));

            }

            if (toolbox != null && debugView)
                foreach (var tool in toolbox.GetTools())
                    if (tool != null)
                        tool.OnDebugGUI(new SerializedObject(tool));


            if (toolbox != null
                && (float)EditorApplication.timeSinceStartup - lastSaveTime > 1f) {
                foreach (var tool in toolbox.GetTools())
                    AssetDatabase.SaveAssetIfDirty(tool);
                lastSaveTime = (float)EditorApplication.timeSinceStartup;
            }

        	GUI.enabled = wasGUIEnabled;
            EditorGUILayout.EndScrollView();
        }

        [DidReloadScripts]
        static void Init() {
            SceneView.duringSceneGui -= OnSceneGUI;
            SceneView.duringSceneGui += OnSceneGUI;

            EditorApplication.update -= OnUpdate;
            EditorApplication.update += OnUpdate;
        }

        static void OnUpdate() {
            if (activeInstance == null) return;
            if (activeInstance.toolbox == null) return;
            if (activeInstance.debugView) return;

            activeInstance.toolbox.OnUpdate();

            foreach (var tool in activeInstance.toolbox.activeTools)
                if (tool != null)
                    tool.OnUpdate(activeInstance);

        }

        static void OnSceneGUI(SceneView sv) {
        	if (activeInstance == null) return;
            if (activeInstance.toolbox == null) return;
            if (activeInstance.debugView) return;

            foreach (var tool in activeInstance.toolbox.activeTools)
                if (tool != null)
                    tool.OnSceneGUI(activeInstance);
        }
    }
}