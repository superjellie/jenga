using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Jenga {
    public class PropertyWindow : EditorWindow {

        public static HashSet<PropertyWindow> windows = new();

        public Object target;
        public string path;

        public static PropertyWindow GetWindowFor(SerializedProperty prop) {
            var target = prop.serializedObject.targetObject;
            var path = prop.propertyPath;
            foreach (var win in windows)
                if (win.target == target && win.path == path)
                    return win;

            // var newWin = CreateWindow<PropertyWindow>(
            //     $"Inspect: {target}:{path}"
            // );

            var newWin = ScriptableObject.CreateInstance<PropertyWindow>();
            newWin.target = target;
            newWin.path = path; 
            newWin.titleContent = new GUIContent($"Property Inspection");

            newWin.Show();
            return newWin;
        } 

        public static bool HasWindowFor(SerializedProperty prop) {
            var target = prop.serializedObject.targetObject;
            var path = prop.propertyPath;
            foreach (var win in windows)
                if (win.target == target && win.path == path)
                    return true;

            return false;
        }


        public void OnEnable() => windows.Add(this);
        public void OnDisable() => windows.Remove(this);
    

        public void CreateGUI() {
            if (target == null) return;

            var so = new SerializedObject(target);
            var prop = so.FindProperty(path);

            if (prop == null) return;

            rootVisualElement.Add(new Label() {
                text = $"{target.name}: {path}"
            });

            rootVisualElement.Add(new PropertyField(prop));
            rootVisualElement.Bind(so);
        }
    }
}
