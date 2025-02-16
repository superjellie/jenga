using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace Jenga {
    public class CoroutineHolderBehaviour : MonoBehaviour { }

    [System.Serializable]
    public struct SceneLink {
        public string path;

        public Scene Get() => SceneManager.GetSceneByPath(path);
    }

    [CreateAssetMenu(
        fileName = "New Level Context",
        menuName = "Jenga/Level Context",
        order = 1000
    )]
    public class LevelContext : ScriptableObject {
        
        public SceneLink mainScene;
        public SceneLink[] additionalScenes = { };

        public static float loadingProgress = 0f;
        public static bool  waitingForActivation = false;
        public static bool  allowActivation = false;

        public void Load(
            bool unloadOtherScenes = true, 
            bool completeAutomatically = true
        ) {

            allowActivation = completeAutomatically;

            var scenesToLoad = new HashSet<string>();
            var scenesToUnload = new HashSet<string>();
            scenesToLoad.Add(mainScene.path);
            
            foreach (var scene in additionalScenes)
                scenesToLoad.Add(scene.path);

            for (int i = 0; i < SceneManager.sceneCount; ++i) {
                var scene = SceneManager.GetSceneAt(i);
                if (!scene.isLoaded) continue;

                if (scenesToLoad.Contains(scene.path))
                    scenesToLoad.Remove(scene.path);
                else if (unloadOtherScenes)
                    scenesToUnload.Add(scene.path);
            }

            var go = new GameObject("SceneLoader");
            var holder = go.AddComponent<CoroutineHolderBehaviour>();
            DontDestroyOnLoad(go);

            IEnumerator Routine() {
                var ops = new HashSet<AsyncOperation>();
                var mode = LoadSceneMode.Additive;

                foreach (var toLoad in scenesToLoad)
    #if UNITY_EDITOR
                    if (Application.isEditor)
                        ops.Add(
                            EditorSceneManager.LoadSceneAsyncInPlayMode(
                                toLoad, new LoadSceneParameters(mode)
                            )
                        );
                    else
    #endif 
                        ops.Add(SceneManager.LoadSceneAsync(toLoad, mode));


                var allDone = false;
                while (!allDone) {
                    allDone = true;
                    waitingForActivation = true;

                    foreach (var op in ops) {
                        if (!op.isDone) allDone = false;
                        if (op.progress < .9f) waitingForActivation = false;
                        loadingProgress = Mathf.Min(loadingProgress, op.progress);
                        op.allowSceneActivation = allowActivation;
                    }

                    yield return null;
                }

                SceneManager.SetActiveScene(mainScene.Get());

                foreach (var toUnload in scenesToUnload) {
                    // Debug.Log(toUnload);
                    // ops.Add(SceneManager.UnloadSceneAsync(toUnload));
                    SceneManager.UnloadSceneAsync(toUnload);
                }

                Resources.UnloadUnusedAssets();

                for (int i = 0; i < SceneManager.sceneCount; ++i) {
                    var scene = SceneManager.GetSceneAt(i);
                    if (!scene.isLoaded) continue;

                    foreach (var go in scene.GetRootGameObjects())
                        go.BroadcastMessage(
                            "OnLevelLoad", null,
                            SendMessageOptions.DontRequireReceiver
                        );
                }

                Destroy(go);
            }

            holder.StartCoroutine(Routine());
        }

    #if UNITY_EDITOR
        [ContextMenu("Load In Editor")]
        public void LoadInEditor() {
            var want = EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            if (!want) return;
            
            EditorSceneManager
                .OpenScene(mainScene.path, OpenSceneMode.Single);

            foreach (var scene in additionalScenes)
                EditorSceneManager
                    .OpenScene(scene.path, OpenSceneMode.Additive);
        }

    #endif
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(SceneLink))]
    public class ScenePropertyDrawer : PropertyDrawer {
        
        public override void OnGUI(
            Rect pos, SerializedProperty prop, GUIContent label
        ) {
            var propPath = prop.FindPropertyRelative("path");
            label = EditorGUI.BeginProperty(pos, label, prop);

            var oldAsset 
                = AssetDatabase.LoadAssetAtPath<SceneAsset>(propPath.stringValue);

            var newAsset = EditorGUI.ObjectField(
                pos, label, oldAsset, typeof(SceneAsset), false
            ) as SceneAsset;

            propPath.stringValue = AssetDatabase.GetAssetPath(newAsset);

            EditorGUI.EndProperty();
        }

    }

    #endif
}