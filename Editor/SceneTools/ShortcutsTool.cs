using System.Collections;
using System.Buffers;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    public class ShortcutsTool : SceneTool {

        public override string category => "Context";
        public override string title => "Shortcuts";
        public override int order => 800;

        [System.Serializable]
        public class Shortcut {
            public string key;
            public string title;

            public TypeName master;
            public System.Action action;

            public EventModifiers defaultMods; 
            public EventModifiers currentMods;

            public KeyCode defaultPreKey;
            public KeyCode currentPreKey;

            public KeyCode defaultKey;
            public KeyCode currentKey;
        }

        public List<Shortcut> shortcuts = new();
        public bool isDisabled = false;

        public Color clrConflictShortcut = Color.magenta;
        public Color clrPressedShortcut = new Color(.7f, .8f, 1f, 1f);

        public void SetShortcut(
            string key, TypeName master, 
            EventModifiers defaultMods, KeyCode defaultPreKey, KeyCode defaultKey,
            string title, System.Action action
        ) {
            var i = shortcuts.FindIndex(x => x != null && x.key == key);

            // EditorUtility.SetDirty(this); 
            if (i != -1) {
                shortcuts[i].defaultMods   = defaultMods;
                shortcuts[i].defaultPreKey = defaultPreKey;
                shortcuts[i].defaultKey    = defaultKey;
                shortcuts[i].title         = title;
                shortcuts[i].action        = action;
                shortcuts[i].master        = master;
                shortcuts[i].key           = key;
            } else {
                // Debug.Log(key);
                shortcuts.Add(new Shortcut() {
                    title         = title,
                    defaultMods   = defaultMods,
                    currentMods   = defaultMods,
                    defaultKey    = defaultKey,
                    currentKey    = defaultKey,
                    defaultPreKey = defaultPreKey,
                    currentPreKey = defaultPreKey,
                    action        = action,
                    master        = master,
                    key           = key
                });
            }
        }

        HashSet<KeyCode> pressedKeys = new();
        EventModifiers   currentMods = EventModifiers.None;
        Vector2 scrollPos;

        public override void OnToolCustomGUI(
            EditorWindow win, SerializedObject so
        ) {
            var propDisabled = so.FindProperty("isDisabled");

            if (GUILayout.Button(isDisabled ? "Enable" : "Disable"))
                propDisabled.boolValue = !propDisabled.boolValue;

            var wasGUIEnabled = GUI.enabled;
            if (propDisabled.boolValue) 
                GUI.enabled = false;

            HandleEvent(win);
            EditorGUI.indentLevel--;


            var size = Mathx.Min(shortcuts.Count, 50);
            // Debug.Log(size);

            var shortcutArray = ArrayPool<Shortcut>.Shared.Rent(size);
            shortcuts.CopyTo(0, shortcutArray, 0, size);

            // var view = ArrayView.Slice<Shortcut>(shortcutArray, 0, size);
            // AQRY.SortBy<Shortcut>(shortcutArray, (shortcut, i) => {
            //     if (shortcut == null) return 10f;
            //     var isPreKey = pressedKeys.Contains(shortcut.currentPreKey);
            //     var isKey    = pressedKeys.Contains(shortcut.currentKey);
            //     var isMods   = shortcut.currentMods == Event.current.modifiers;
            //     return isPreKey && isMods ? 0f : isPreKey ? 1f : 2f;
            // });

            var prefferedHeight = size * (
                EditorGUIUtility.singleLineHeight
                + EditorGUIUtility.standardVerticalSpacing
            );

            scrollPos = EditorGUILayout.BeginScrollView(
                scrollPos, GUILayout.ExpandHeight(false),
                GUILayout.MinHeight(Mathf.Min(prefferedHeight, 200f))
            );

            for (int i = 0; i < size; ++i) {
                var shortcut = shortcutArray[i];


                if (shortcut == null || shortcut.action == null
                    || !toolbox.IsActive(shortcut.master))
                    continue;

                EditorGUILayout.BeginHorizontal();

                // Modifiers
                var oldColor = GUI.backgroundColor;
                var styMods = new GUIStyle(GUI.skin.button);
                if (shortcut.defaultMods != shortcut.currentMods)
                    styMods.fontStyle = FontStyle.Bold;
                if (currentMods == shortcut.currentMods)
                    GUI.backgroundColor = clrPressedShortcut;

                var width50 = GUILayout.Width(50f);
                GUILayout.Box(shortcut.currentMods.ToString(), styMods, width50);
                GUI.backgroundColor = oldColor;

                // Pre Key
                var styPreKey = new GUIStyle(GUI.skin.button);
                if (shortcut.defaultPreKey != shortcut.currentPreKey)
                    styPreKey.fontStyle = FontStyle.Bold;
                if (pressedKeys.Contains(shortcut.currentPreKey))
                    GUI.backgroundColor = clrPressedShortcut;

                GUILayout.Box(shortcut.currentPreKey.ToString(), styPreKey, width50);
                GUI.backgroundColor = oldColor;


                // Key
                var styKey = new GUIStyle(GUI.skin.button); 
                if (shortcut.defaultKey != shortcut.currentKey)
                    styKey.fontStyle = FontStyle.Bold;
                if (pressedKeys.Contains(shortcut.currentKey))
                    GUI.backgroundColor = clrPressedShortcut;

                GUILayout.Box(shortcut.currentKey.ToString(), styKey, width50);
                GUI.backgroundColor = oldColor;

                // Title
                EditorGUILayout.LabelField(
                    "", shortcut.title, GUILayout.MinWidth(0f)
                );

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            // var propClrConflictShortcut 
            //     = so.FindProperty("clrConflictShortcut");
            var propClrPressedShortcut 
                = so.FindProperty("clrPressedShortcut");

            EditorGUILayout.PropertyField(propClrPressedShortcut);
            // EditorGUILayout.PropertyField(propClrConflictShortcut);

            ArrayPool<Shortcut>.Shared.Return(shortcutArray);
            
            EditorGUI.indentLevel++;
            GUI.enabled = wasGUIEnabled;
        }


        public override void OnDeactivate() { 
            shortcuts.Clear();
        }

        public override void OnSceneGUI(EditorWindow win) {
            if (SceneToolWindow.HasWindowFor(this.GetType()))
                HandleEvent(SceneToolWindow.GetWindowFor(this.GetType(), "Title"));
            else
                HandleEvent(win);
        }

        void HandleEvent(EditorWindow window) {

            if (isDisabled) return;


            if (Event.current.type == EventType.KeyDown)
                pressedKeys.Add(Event.current.keyCode);
            else if (Event.current.type == EventType.KeyUp)
                pressedKeys.Remove(Event.current.keyCode);
            else 
                return;

            currentMods = Event.current.modifiers;

            if (Event.current.type == EventType.KeyDown)
                foreach (var shortcut in shortcuts) {
                    var isPrePressed = pressedKeys.Contains(shortcut.currentPreKey);
                    var isPressed = pressedKeys.Contains(shortcut.currentKey);

                    if (shortcut == null || shortcut.action == null)
                        continue;

                    var master = toolbox.GetActiveTool(shortcut.master);
                    if (master == null) continue;

                    if (currentMods == shortcut.currentMods && (
                        isPressed && shortcut.currentPreKey == KeyCode.None
                        || isPrePressed && shortcut.currentPreKey != KeyCode.None
                    ))  Event.current.Use();

                    if (currentMods == shortcut.currentMods 
                        && isPrePressed && isPressed
                        && Event.current.keyCode == shortcut.currentKey
                    ) {
                        shortcut.action.Invoke();
                        ToolboxWindow.UpdateToolbox();
                        break;
                    }
                }

            window.Repaint();
        }

    }
}