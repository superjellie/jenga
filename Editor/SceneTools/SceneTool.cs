using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Jenga {

    // If you want tool to be available in SceneTool Selector
    // use [RegisterInToolSelector(typeof(MyTool), "MyCategory/MyTool")]
    // If you want tool to ignore channels and always be present
    // use [GeneralTool(typeof(MyTool))]
    // If you want tool to depend on another
    // use [RequireTool(typeof(MyOtherTool))]
    public class SceneTool : ScriptableObject {

        // Override with category name
        public virtual string category => "Unspecified";

        // Override with tab title
        public virtual string title => "Unnamed";

        public virtual int order => 0;

        // Override with 

        // Override for channels
        // Channel is a state provider for saving tool data
        // Any tool can be enabled to be saved per channel state
        // States must be unique, so use "MyCategory/MyChannel/MyState" style
        // Channel also needs to be [ToolChannel]
        public virtual ToolChannelState[] channelStates => 
            new ToolChannelState[0];

        // Override to hide tool in GUI (it will not recieve OnToolGUI)
        public virtual bool isHidden => false;

        [HideInInspector] public bool isFoldedOut = false;

        // Target channels for this tool
        // Do not edit, read only
        [HideInInspector] public ToolChannelState[] targetChannels = { };


        // Toolbox, this SceneTool belongs to
        public SceneToolbox toolbox => (SceneToolbox)AssetDatabase.LoadAssetAtPath(
            AssetDatabase.GetAssetPath(this), typeof(SceneToolbox)
        );

        // Override these functions
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }

        public virtual void OnUpdate(EditorWindow win) { }
        public virtual void OnSceneGUI(EditorWindow win) { }

        public virtual void OnToolCustomGUI(
            EditorWindow window, SerializedObject self
        ) {
            var it = self.GetIterator();
            it.NextVisible(true);
            do {
                if (it.name == "m_Script") 
                    continue;
                EditorGUILayout.PropertyField(it, true);                        
            } while (it.NextVisible(false));
        }

        // For Scene Tools Window use
        public bool MatchChannels(ToolChannelState[] channels) {
            if (channels.Length != targetChannels.Length) return false;

            for (int i = 0; i < channels.Length; ++i)
                if (!channels[i].SameAs(targetChannels[i])) 
                    return false;

            return true;
        }

        public void OnToolGUI(EditorWindow window, SerializedObject self) {

            var propFolded = self.FindProperty("isFoldedOut");
            var width20 = GUILayout.Width(20f);
            var width100 = GUILayout.Width(100f);
            var styHelpBox = EditorStyles.helpBox;
            var styWhiteLabel = EditorStyles.whiteLabel;
            var styBoldLabel = EditorStyles.boldLabel;
            var styFoldout = EditorStyles.foldout;

            // var styBack = new GUIStyle(EditorStyles.selectionRect);

            EditorGUIUtility.labelWidth = 120f;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(styHelpBox);

            if (window.GetType() == typeof(ToolboxWindow))
                propFolded.boolValue = GUILayout.Toggle(
                    propFolded.boolValue, "", styFoldout, width20
                );

            var oldColor = GUI.contentColor;
            GUI.contentColor = Color.gray;
            GUILayout.Label(category, styBoldLabel, width100);
            GUI.contentColor = oldColor;
            GUILayout.Label(title, styBoldLabel);

            // Channels
            var channelsCount = toolbox.activeChannels.Count;
            if (!toolbox.IsGeneralType(this.GetType())) {
                GUILayout.FlexibleSpace();

                var settings = toolbox.channelSettings[this.GetType()].ToArray();
                var states = ToolChannelState.Intersect(
                    settings, toolbox.activeChannelStates
                );

                var name = "";
                foreach (var state in states) name += state.shortName;

                if (GUILayout.Button(name, GUILayout.Width(50f))) {

                    var menu = new GenericMenu();

                    foreach (var state in toolbox.activeChannelStates) 
                        if (!state.HasSameChannelAsIn(channelStates)) {
                            menu.AddItem(
                                new GUIContent(
                                    $"Save for {state.channel}: {state.state}"
                                ),
                                state.IsIn(states),
                                () => {
                                    var index = System.Array.FindIndex(
                                        settings, x => x.SameAs(state)
                                    );
                                    if (index >= 0)
                                        toolbox.channelSettings[this.GetType()]
                                            .RemoveAt(index);
                                    else 
                                        toolbox.channelSettings[this.GetType()]
                                            .Add(state);
                                    toolbox.UpdateActiveTools();
                                }
                            );
                        }

                    menu.ShowAsContext();
                }
                
            }

            var hasWindow = SceneToolWindow.HasWindowFor(this.GetType());
     
            // Pin
            var pinIcon = EditorGUIUtility.IconContent("d_ScaleTool");
            if (!hasWindow && GUILayout.Button(pinIcon, GUILayout.Width(25f)))
                SceneToolWindow.GetWindowFor(this.GetType(), $"{category}/{title}");
            var oldBckgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.yellow;
            if (hasWindow && GUILayout.Button(pinIcon, GUILayout.Width(25f)))
                SceneToolWindow.CloseAllWindows(this.GetType());
            GUI.backgroundColor = oldBckgColor;

            GUI.contentColor = oldColor;
            EditorGUILayout.EndHorizontal();

            if (propFolded.boolValue && window.GetType() == typeof(ToolboxWindow)
                && !hasWindow || window.GetType() == typeof(SceneToolWindow)
            ) {
                EditorGUI.indentLevel++;
                OnToolCustomGUI(window, self);
                EditorGUILayout.Space();
                EditorGUI.indentLevel--;   
            }

            EditorGUILayout.EndVertical();
            self.ApplyModifiedProperties();
        }

        public void OnDebugGUI(SerializedObject self) { 
            var propFolded = self.FindProperty("isFoldedOut");
            var width20 = GUILayout.Width(20f);
            var width100 = GUILayout.Width(100f);
            var styHelpBox = EditorStyles.helpBox;
            var styWhiteLabel = EditorStyles.whiteLabel;
            var styBoldLabel = EditorStyles.boldLabel;
            var styFoldout = EditorStyles.foldout;

            // var styBack = new GUIStyle(EditorStyles.selectionRect);

            EditorGUIUtility.labelWidth = 120f;

            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal(styHelpBox);

            propFolded.boolValue = GUILayout.Toggle(
                propFolded.boolValue, "", styFoldout, width20
            );

            var oldColor = GUI.contentColor;
            GUI.contentColor = Color.gray;
            GUILayout.Label(category, styBoldLabel, width100);
            GUI.contentColor = oldColor;
            GUILayout.Label(title, styBoldLabel);

            if (GUILayout.Button("-", width20)) {
                Undo.RecordObject(toolbox, $"Remove links to {name}");
                if (toolbox.activeChannels.Contains(this))
                    toolbox.activeChannels.Remove(this);
                if (toolbox.activeTools.Contains(this))
                    toolbox.activeTools.Remove(this);

                var myToolbox = toolbox;

                AssetDatabase.RemoveObjectFromAsset(this);
                AssetDatabase.SaveAssets();

                myToolbox.UpdateActiveTools();
            }

            EditorGUILayout.EndHorizontal();

            if (propFolded.boolValue) {
                EditorGUI.indentLevel++;

                var propTargetChannels = self.FindProperty("targetChannels");
                EditorGUILayout.PropertyField(propTargetChannels, true); 

                var it = self.GetIterator();
                it.NextVisible(true);
                do {
                    var path = new GUIContent(it.propertyPath);
                    EditorGUILayout.PropertyField(it, path, true);                        
                } while (it.NextVisible(false));

                EditorGUILayout.Space();
                EditorGUI.indentLevel--;   
            }
            EditorGUILayout.EndVertical();
            self.ApplyModifiedProperties();
        }

    }

    public class SceneToolWindow : EditorWindow {

        public TypeName type;

        public static List<SceneToolWindow> instances = new();

        public static SceneToolWindow 
        GetWindowFor(TypeName typeName, string title) {
            foreach (var window in instances)
                if (window != null && window.type.Equals(typeName)) 
                    return window;

            var win = CreateWindow<SceneToolWindow>(title);
            win.type = typeName;
            return win;
        }

        public static void CloseAllWindows(TypeName typeName) {
            var toRemove = new List<EditorWindow>();
            foreach (var window in instances)
                if (window != null && window.type.Equals(typeName))
                    toRemove.Add(window);

            foreach (var window in toRemove)
                window.Close();
        }

        public static bool HasWindowFor(TypeName typeName) {
            foreach (var window in instances)
                if (window != null && window.type.Equals(typeName))
                    return true;
            return false;
        }

        public static void RepaintAll() {
            foreach (var window in instances)
                if (window != null)
                    window.Repaint();

        }

        void OnDisable() {
            instances.Remove(this);
        }

        void OnEnable() {
            instances.Add(this);
        }

        void OnGUI() {
            var toolboxWin = ToolboxWindow.activeInstance;
            
            if (toolboxWin == null || toolboxWin.toolbox == null) {
                EditorGUILayout.HelpBox("No active toolbox", MessageType.Info);
                return;
            }

            var tool = toolboxWin.toolbox.GetActiveTool(type);

            if (tool == null) {
                EditorGUILayout.HelpBox("No active tool", MessageType.Info);
                return;
            }

            tool.OnToolGUI(this, new SerializedObject(tool));
        }
    }
}