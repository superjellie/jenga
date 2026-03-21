using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Jenga {
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class RegisterInToolSelectorAttribute : System.Attribute { 
        public string name = $"Unnamed";

        public RegisterInToolSelectorAttribute(string name) => this.name = name;
    }

    [GeneralTool(alwaysShow = true)]
    [ToolChannel]
    // [RequireTool(typeof(ShortcutsTool))]
    public class ToolSelector : SceneTool {

        // public ShortcutsTool shortcuts => 
        //     toolbox.GetActiveTool<ShortcutsTool>();

        public override string category => "Default";
        public override string title => "SceneTool Selector";
        public override int order => 10;

        public List<string> mainTypeNames = new();

        public static System.Type[] registered = { };
        public static string[]      registeredNames = { };

        public override ToolChannelState[] channelStates => 
            mainTypeNames.Count > 0 
                ? new ToolChannelState[] { new($"SceneTool", $"{mainTypeNames[0]}") }
                : new ToolChannelState[] { };

        // string switchToToolName = "Dummy";

        public override void OnToolCustomGUI(
            EditorWindow win, SerializedObject self
        ) {
            EditorGUI.BeginChangeCheck();
            var propMainTypeNames = self.FindProperty("mainTypeNames");
            var toRemove = -1;
            for (int i = 0; i < propMainTypeNames.arraySize; ++i) {
                var propTypeName = propMainTypeNames.GetArrayElementAtIndex(i);

                var id = System.Array.FindIndex(
                    registeredNames, x => x == propTypeName.stringValue
                );

                EditorGUILayout.BeginHorizontal();
                id = EditorGUILayout.Popup(id, registeredNames);
                
                if (GUILayout.Button("-", GUILayout.Width(20f)))
                    toRemove = i;
                
                EditorGUILayout.EndHorizontal();
                propTypeName.stringValue = id >= 0 && id < registeredNames.Count()
                    ? registeredNames[id] : "Dummy";
            }

            if (toRemove >= 0)
                propMainTypeNames.DeleteArrayElementAtIndex(toRemove);

            else if (GUILayout.Button("Add SceneTool"))
                propMainTypeNames
                    .InsertArrayElementAtIndex(propMainTypeNames.arraySize);


            if (EditorGUI.EndChangeCheck()) {
                self.ApplyModifiedProperties();

                var types = new System.Type[propMainTypeNames.arraySize];
                for (int i = 0; i < propMainTypeNames.arraySize; ++i) {
                    var propTypeName = propMainTypeNames.GetArrayElementAtIndex(i);

                    var id = System.Array.FindIndex(
                        registeredNames, x => x == propTypeName.stringValue
                    );

                    types[i] = id >= 0 && id < registered.Length 
                        ? registered[id]
                        : typeof(DummyTool);
                }

                // switchToToolName = "Dummy";                

                toolbox.UpdateMainTypes(types);
                toolbox.UpdateActiveTools();
            }
        }


        public override void OnUpdate(EditorWindow win) {
            // shortcuts.SetShortcut(
            //     "ToolSelector/SwitchTool", this.GetType(),
            //     EventModifiers.Control, KeyCode.None, KeyCode.T, 
            //     $"Switch To {switchToToolName}",
            //     () => {
            //         Undo.RecordObject(this, $"Switch tool to {switchToToolName}");

            //         if (mainTypeNames.Count == 0)
            //             mainTypeNames.Add()

            //         (mainTypeNames[0], switchToToolName) 
            //             = (switchToToolName, mainTypeNames[0]);

            //         var mainTypeIndex = System.Array.FindIndex(
            //             registeredNames, x => x == mainTypeName
            //         );

            //         toolbox.UpdateMainTypes(registered[mainTypeIndex]);
            //         toolbox.UpdateActiveTools();
            //     }
                
            // ); 
        }

        [DidReloadScripts]
        static void Init() {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(i => i.GetTypes())
                .Where(t => typeof(SceneTool).IsAssignableFrom(t))
                .Where(t => {
                    var attrs = t.GetCustomAttributes(
                        typeof(RegisterInToolSelectorAttribute), false
                    );
                    return attrs.Count() > 0;
                });

            registered = types.ToArray();

            registeredNames = registered.Select(t => {
                var attrs = t.GetCustomAttributes(
                        typeof(RegisterInToolSelectorAttribute), false
                    );
                return (attrs[0] as RegisterInToolSelectorAttribute).name;
            }).ToArray();
        }
    }
}