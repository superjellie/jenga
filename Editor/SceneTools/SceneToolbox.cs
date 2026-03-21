using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Jenga;
using UnityEditor;
using UnityEditor.Callbacks;

namespace Jenga {

    [System.AttributeUsage(System.AttributeTargets.Class, AllowMultiple = true)]
    public class RequireToolAttribute : System.Attribute {
        public System.Type type;
        public RequireToolAttribute(System.Type type) => this.type = type;
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class GeneralToolAttribute : System.Attribute {
        public bool alwaysShow = false;
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ToolChannelAttribute : System.Attribute { }

    [CreateAssetMenu(menuName="Jenga/Create Toolbox", fileName="New Toolbox")]
    public class SceneToolbox : ScriptableObject {

        // Readonly
        // channels that are active in toolbox
        public List<SceneTool> activeChannels = new();

        // Readonlu
        // channels that are active in toolbox
        public List<SceneTool> activeTools = new();

        // Readonly
        // currently enabled channel settings per tool type
        public SerializableDictionary<TypeName, List<ToolChannelState>> 
            channelSettings = new();

        public SceneTool[] GetTools() {
            var path = AssetDatabase.GetAssetPath(this);
            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            var view = AQRY.Where(ArrayView.Array(assets), (x, i) => x is SceneTool);
            return AQRY.MakeArray(view.length, (i) => view[i] as SceneTool);
        }

        ToolChannelState[] lastActiveChannels = {};
        public ToolChannelState[] activeChannelStates {
            get {
                var ret = activeChannels
                    .SelectMany(x => x.channelStates)
                    .ToList();

                ret.Sort((x, y) => {
                    var cmp = x.channel.CompareTo(y.channel);
                    return cmp != 0 ? cmp : x.state.CompareTo(y.state);
                });

                var retarr = ret.ToArray();

                bool isDifferent = lastActiveChannels.Length != retarr.Length;
                if (!isDifferent)
                    for (int i = 0; i < retarr.Length; ++i)
                        if (!lastActiveChannels[i].SameAs(retarr[i])) {
                            isDifferent = true;
                            break;
                        }

                if (isDifferent)
                    ToolboxWindow.UpdateToolbox();

                lastActiveChannels = retarr;

                return retarr;
            }
        }

        //
        public List<TypeName> mainTypes = new();


        //
        public bool toUpdate = true;
        public void UpdateActiveTools() => toUpdate = true;

        public void OnUpdate() {

            if (!toUpdate) return;
            toUpdate = false;

            var allTypes = new List<System.Type>();

            allTypes.AddRange(GetAlwaysShownTypes());

            foreach (var mainType in mainTypes)
                allTypes.Add(mainType);

            var activeChannelTypes = new List<System.Type>();

            for (int i = 0; i < allTypes.Count; ++i) {
                var type = allTypes[i];

                if (type == null) continue;

                var attributes = type.GetCustomAttributes(
                    typeof(RequireToolAttribute), false
                );

                foreach (var attr in attributes) {
                    var req = attr as RequireToolAttribute;
                    if (!allTypes.Contains(req.type))
                        allTypes.Add(req.type);
                }

                var channelAttrs = type.GetCustomAttributes(
                    typeof(ToolChannelAttribute), false
                );

                if (channelAttrs.Length > 0)
                    activeChannelTypes.Add(type);
            }

            Undo.RecordObject(this, "Update active tools");
            // lastMainTypes = mainTypes;

            foreach (var tool in activeTools) {
                if (tool != null)
                    tool.OnDeactivate();
            }

            activeTools.Clear();
            activeChannels.Clear();

            foreach (var type in activeChannelTypes)
                activeChannels.Add(GetTool(type, null));


            foreach (var type in allTypes) {
                var tool = GetTool(type, activeChannels);
                if (tool == null) continue;
                activeTools.Add(tool);
            }

            activeTools.Sort((x, y) => x.order.CompareTo(y.order));

            // foreach (var tool in activeTools)
            //     tool.toolbox = this;

            foreach (var tool in activeTools)
                tool.OnActivate();
        }

        public SceneTool GetActiveTool(System.Type type) {
            foreach (var tool in activeTools)
                if (tool.GetType() == type)
                    return tool;
            return null;
        }

        public T GetActiveTool<T>() where T : SceneTool => GetActiveTool(typeof(T)) as T;
        public bool IsActive(System.Type type) => GetActiveTool(type) != null;
        public bool IsActive<T>() => GetActiveTool(typeof(T)) != null;

        public void UpdateMainTypes(params System.Type[] types) {
            Undo.RecordObject(this, "Update main types");
            mainTypes.Clear();
            foreach (var type in types) 
                mainTypes.Add(type);
        }

        public SceneTool GetTool(System.Type type, List<SceneTool> chans) {
            if (type == null) return null;

            if (!channelSettings.ContainsKey(type))
                channelSettings[type] = new();

            var enabledChannels
                = chans != null && !IsGeneralType(type)
                    ? chans.SelectMany(x => ToolChannelState.Intersect(
                            x.channelStates, channelSettings[type].ToArray()
                        ).ToArray()
                    ).ToArray()
                    : new ToolChannelState[] { };

            System.Array.Sort<ToolChannelState>(
                enabledChannels, (x, y) => {
                    var cmp = x.channel.CompareTo(y.channel);
                    return cmp != 0 ? cmp : x.state.CompareTo(y.state);
                }
            );

            var tools = GetTools();

            foreach (var tool in tools)
                if (tool.GetType() == type && tool.MatchChannels(enabledChannels))
                    return tool;

            var so = ScriptableObject.CreateInstance(type) as SceneTool;
            Undo.RegisterCreatedObjectUndo(so, "Create new SceneTool");
            Undo.RecordObject(so, "Setup new SceneTool");
            so.targetChannels = enabledChannels;
            so.name = $"{so.category}/{so.title}";
            // so.toolbox = this;
            AssetDatabase.AddObjectToAsset(so, AssetDatabase.GetAssetPath(this));
            AssetDatabase.SaveAssets();
            return so;
        }

        public System.Type[] GetAlwaysShownTypes() {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            var types = assemblies.SelectMany(i => i.GetTypes())
                .Where(t => typeof(SceneTool).IsAssignableFrom(t))
                .Where(t => {
                    var attrs = t.GetCustomAttributes(
                        typeof(GeneralToolAttribute), false
                    );
                    if (attrs.Count() == 0) return false;

                    return (attrs[0] as GeneralToolAttribute).alwaysShow;
                });
            return types.ToArray();
        }

        public bool IsGeneralType(System.Type type) {
            if (type == null) return false;

            var attrs = type.GetCustomAttributes(
                typeof(GeneralToolAttribute), false
            );

            return attrs.Count() > 0;
        }

    }
}
