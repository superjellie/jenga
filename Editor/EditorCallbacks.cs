using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {


    public class EditorCallbacks : MonoBehaviour {

        public delegate void ValidateOtherComponentDelegate(
            SerializedObject self, SerializedObject other
        );
        public delegate void ValidateDelegate(
            SerializedObject self
        );

        static Dictionary<System.Type, List<ValidateDelegate>> 
            validators = new Dictionary<System.Type, List<ValidateDelegate>>();
        static event System.Action<DestroyGameObjectHierarchyEventArgs> 
            onDestroy;

        static HashSet<Object> validatedLastFrame = new HashSet<Object>();
        
        public static void CallOnValidateOtherComponent<T, Q>(
            ValidateOtherComponentDelegate method
        ) where T : Component where Q : Component { 
            var targetType = typeof(Q);
            if (!validators.ContainsKey(targetType))
                validators.Add(targetType, new List<ValidateDelegate>());
            var vals = validators[targetType];
            vals.Add((so) => {
                var target = so.targetObject as Component;
                var comp = target.GetComponent<T>();
                if (comp == null) return;

                var compSo = new SerializedObject(comp);
                method(compSo, so);
                compSo.ApplyModifiedProperties();
            });
        } 

        public static void CallOnValidate<T>(ValidateDelegate method) 
        where T : Object { 
            var targetType = typeof(T);
            if (!validators.ContainsKey(targetType))
                validators.Add(targetType, new List<ValidateDelegate>());
            var vals = validators[targetType];
            vals.Add(method);
        }

        [InitializeOnLoadMethod]
        static void Init() {
            // ObjectChangeEvents.changesPublished += OnChangesPublished;
            // Undo.postprocessModifications += PostprocessModifications;
        }


        public static void CallOnSceneGUI<T>(
            System.Action<T, SceneView> method
        ) where T : Object { 
            SceneView.duringSceneGui += (sv) => {
                var objects = Object.FindObjectsByType<T>(
                    FindObjectsSortMode.None
                );
                foreach (var obj in objects) method(obj, sv);
            }; 
        }

        public static void CallOnDestroyObject(
            System.Action<DestroyGameObjectHierarchyEventArgs> method
        ) => onDestroy += method;
        

        static void Validate(Object o) {
            var type = o.GetType();
            if (!validators.ContainsKey(type)) return;
            var vals = validators[type];
            
            var so = new SerializedObject(o);
            foreach (var val in vals) val(so);
            so.ApplyModifiedProperties();
        }

    
        static void OnChangesPublished(ref ObjectChangeEventStream stream) {
            for (int i = 0; i < stream.length; ++i) 
            switch (stream.GetEventType(i)) {
                case ObjectChangeKind.ChangeScene: {
                    stream.GetChangeSceneEvent(i, out var evt);
                    break;
                } case ObjectChangeKind.CreateGameObjectHierarchy: {
                    stream.GetCreateGameObjectHierarchyEvent(i, out var evt);
                    var go = EditorUtility.InstanceIDToObject(evt.instanceId) 
                        as GameObject;
                    break;
                } case ObjectChangeKind.ChangeGameObjectStructureHierarchy: {
                    stream.GetChangeGameObjectStructureHierarchyEvent(
                        i, out var evt
                    );
                    var go = EditorUtility.InstanceIDToObject(evt.instanceId) 
                        as GameObject;
                    break;
                } case ObjectChangeKind.ChangeGameObjectStructure: {
                    stream.GetChangeGameObjectStructureEvent(i, out var evt);
                    var go = EditorUtility.InstanceIDToObject(evt.instanceId) 
                        as GameObject;
                    break;
                } case ObjectChangeKind.ChangeGameObjectParent: {
                    stream.GetChangeGameObjectParentEvent(i, out var evt);
                    var go = EditorUtility.InstanceIDToObject(evt.instanceId) 
                        as GameObject;
                    var newParent = EditorUtility.InstanceIDToObject(
                        evt.newParentInstanceId
                    ) as GameObject;
                    var prevParent = EditorUtility.InstanceIDToObject(
                        evt.previousParentInstanceId
                    ) as GameObject;
                    break;
                } case ObjectChangeKind.ChangeGameObjectOrComponentProperties: {
                    stream.GetChangeGameObjectOrComponentPropertiesEvent(
                        i, out var evt
                    );
                    var obj = EditorUtility.InstanceIDToObject(evt.instanceId);
                    Validate(obj);
                    break;
                } case ObjectChangeKind.DestroyGameObjectHierarchy: {
                    stream.GetDestroyGameObjectHierarchyEvent(i, out var evt);
                    var parent = EditorUtility.InstanceIDToObject(
                        evt.parentInstanceId
                    ) as GameObject;
                    onDestroy(evt);
                    break;
                } case ObjectChangeKind.CreateAssetObject: {
                    stream.GetCreateAssetObjectEvent(i, out var evt);
                    var ass = EditorUtility.InstanceIDToObject(evt.instanceId);
                    var path = AssetDatabase.GUIDToAssetPath(evt.guid);
                    break;
                } case ObjectChangeKind.DestroyAssetObject: {
                    stream.GetDestroyAssetObjectEvent(i, out var evt);
                    break;
                } case ObjectChangeKind.ChangeAssetObjectProperties: {
                    stream.GetChangeAssetObjectPropertiesEvent(i, out var evt);
                    var ass = EditorUtility.InstanceIDToObject(evt.instanceId);
                    var path = AssetDatabase.GUIDToAssetPath(evt.guid);
                    break;
                } case ObjectChangeKind.UpdatePrefabInstances: {
                    stream.GetUpdatePrefabInstancesEvent(i, out var evt);                    
                    break;
                }
            }
        }
    }
}
