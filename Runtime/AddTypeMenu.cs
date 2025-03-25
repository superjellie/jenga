using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.CompilerServices;
using System.IO;


[System.AttributeUsage(System.AttributeTargets.Class)]
public class AddTypeMenuAttribute : System.Attribute {

    public System.Type typeFamily; 
    public string path = "Default/None";
    public string pathToSource = "";
    public int sourceLineNumber;
    public int order = 1000;

    public AddTypeMenuAttribute(
        System.Type typeFamily, string path, int order = 1000,
        [CallerFilePath] string pathToSource = null,
        [CallerLineNumber] int sourceLineNumber = 1
    ) {
        this.typeFamily = typeFamily;
        this.path = path;
        this.order = order;
        this.sourceLineNumber = sourceLineNumber;
        this.pathToSource = Path.Combine(
            "Assets", 
            Path.GetRelativePath(Application.dataPath, pathToSource)
        );
    }

    public struct Registration {
        public System.Type type;
        public string path;
        public int order;
        public string pathToSource;
        public int sourceLineNumber;
    } 

    public static Dictionary<System.Type, List<Registration>> registries 
        = new();

    static AddTypeMenuAttribute() {
        foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            foreach (var type in assembly.GetTypes()) {
                var attrs = type.GetCustomAttributes(
                    typeof(AddTypeMenuAttribute), false
                );
                foreach (var attr in attrs) {
                    var atm = attr as AddTypeMenuAttribute;

                    var reg = new Registration() {
                        type = type, path = atm.path, order = atm.order,
                        pathToSource = atm.pathToSource,
                        sourceLineNumber = atm.sourceLineNumber
                    };

                    if (registries.ContainsKey(atm.typeFamily))
                        registries[atm.typeFamily].Add(reg);
                    else
                        registries.Add(
                            atm.typeFamily, new List<Registration>() { reg }
                        );
                }
            }

        foreach (var (type, registry) in registries)
            registry.Sort((x, y) => x.order.CompareTo(y.order));
    }

}

[System.AttributeUsage(System.AttributeTargets.Class)]
public class InlinePropertyEditorAttribute : System.Attribute { }