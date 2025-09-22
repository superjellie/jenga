using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jenga {

    public static class SerializedPropertyExtensions {

        public static bool IsManagedReference(this SerializedProperty property)
            => property.propertyType == SerializedPropertyType.ManagedReference;

        public static SerializedProperty FindPropertyOnParent(
            this SerializedProperty property, string path
        ) {
            var index = property.propertyPath.LastIndexOf('.');

            var newPath = index >= 0 
                ? $"{property.propertyPath.Substring(0, index + 1)}{path}"
                : path;
                
            return property.serializedObject.FindProperty(newPath);
        }

        public static bool TryGetParentProperty(
            this SerializedProperty property, out SerializedProperty parent
        ) {
            var index = property.propertyPath.LastIndexOf('.');

            if (index >= 0) {
                var newPath 
                    = $"{property.propertyPath.Substring(0, index)}";
                parent = property.serializedObject.FindProperty(newPath);
                return true;
            }

            parent = null;
            return false;
        }

        public static bool 
        SameAs(this SerializedProperty self, SerializedProperty other) 
            => SerializedProperty.EqualContents(self, other);

        public static string GetEditorDataKey(
            this SerializedProperty property, bool includeGUID = true
        ) {
            if (property == null) return "null";
            if (property.serializedObject == null) return "null";

            var obj = property.serializedObject.targetObject;
            if (obj == null) return "null";

            var myName = property.propertyType
                == SerializedPropertyType.ManagedReference
                    ? $"{property.managedReferenceId}"
                    : property.name;

            var guid = GlobalObjectId.GetGlobalObjectIdSlow(obj);

            if (property.TryGetParentProperty(out var parent)) {
                var pName = parent.GetEditorDataKey(includeGUID: false);
                return $"{(includeGUID ? guid + ":" : "")}{pName}.{myName}";
            } else 
                return $"{(includeGUID ? guid + ":" : "")}{myName}";
        }

        public static System.Type GetCurrentType(this SerializedProperty prop) {
            if (prop.propertyType == SerializedPropertyType.ManagedReference) {
                if (prop.managedReferenceId < 0) return null;

                var typeName = prop.managedReferenceFullTypename;
                if (string.IsNullOrEmpty(typeName)) return null;
                var splitIndex = typeName.IndexOf(' ');
                var assembly = Assembly.Load(typeName.Substring(0, splitIndex));
                return assembly.GetType(typeName.Substring(splitIndex + 1));
            }

            return null;
        }

        public static System.Type GetFieldType(this SerializedProperty prop) {
            if (prop.propertyType == SerializedPropertyType.ManagedReference) {
                var typeName = prop.managedReferenceFieldTypename;
                if (string.IsNullOrEmpty(typeName)) return null;
                
                var splitIndex = typeName.IndexOf(' ');
                var assembly = Assembly.Load(typeName.Substring(0, splitIndex));
                return assembly.GetType(typeName.Substring(splitIndex + 1));
            }

            return null;
        }

        public static string GetDisplayName(this SerializedProperty property) {
            return ObjectNames.NicifyVariableName(property.name);
        }

        public static void EditScript(this SerializedProperty property) {
            var type = property?.GetCurrentType();
            if (type == null) return;

            if (type.TryGetAttribute<AddTypeMenuAttribute>(out var atm))
                atm.reg.EditScript();
        }

        public static bool TrySetType(
            this SerializedProperty prop, System.Type type,
            bool doNotApply = false
        ) { 
            if (prop.propertyType != SerializedPropertyType.ManagedReference) 
                return false;

            object result = null;

            if (type != null && prop.managedReferenceValue != null) {
                
                // Preserve serialized values if we can 
                // ... no better not to preserve)))
                // string json = JsonUtility.ToJson(prop.managedReferenceValue);
                // result = JsonUtility.FromJson(json, type);

            }

            if (result == null) {
                result = type != null 
                    ? System.Activator.CreateInstance(type) 
                    : null;
            }
            
            prop.managedReferenceValue = result;

            if (!doNotApply)
                prop.serializedObject.ApplyModifiedProperties(); 
                
            return true; // ???
        }

        public static void WrapWith(
            this SerializedProperty property, System.Type type,
            bool doNotApply = false
        ) {
            if (property == null) return;
            if (property.propertyType 
                    != SerializedPropertyType.ManagedReference) return;

            var value = property.managedReferenceValue;
            if (type.TryGetFieldWithAttribute<WrapperAttribute>(out var fi)) {

                var newValue = System.Activator.CreateInstance(type);

                if (fi.FieldType.IsArray) {
                    var newArray = System.Array
                        .CreateInstance(fi.FieldType.GetElementType(), 1);
                    newArray.SetValue(value, 0);
                    fi.SetValue(newValue, newArray);
                } else 
                    fi.SetValue(newValue, value);
                property.managedReferenceValue = newValue;

                if (!doNotApply)
                    property.serializedObject.ApplyModifiedProperties(); 
            }
        }

        // ********************* Wrapping *******************************
        public static bool IsWrapper(this SerializedProperty property)
            => property.GetCurrentType()
            ?.TryGetFieldWithAttribute<WrapperAttribute>(out var _)
            ?? false;

        public static void ReplaceWithWrapped(
            this SerializedProperty property, bool doNotApply = false
        ) {
            if (property == null) return;
            if (property.propertyType 
                    != SerializedPropertyType.ManagedReference) return;

            var type = property.GetCurrentType();
            if (type == null) return;

            var currentValue = property.managedReferenceValue;
            if (currentValue == null) return;

            if (type.TryGetFieldWithAttribute<WrapperAttribute>(out var fi)) {
                var value = fi.GetValue(currentValue);

                if (fi.FieldType.IsArray) {
                    var array = value as object[];
                    if (array == null || array.Length == 0) 
                        property.managedReferenceValue = null;
                    else
                        property.managedReferenceValue = array[0];
                } else
                    property.managedReferenceValue = value;


                if (!doNotApply)
                    property.serializedObject.ApplyModifiedProperties(); 
            }
        }


        // ********************* Copy & Paste *******************************
        public static void ClipCopy(this SerializedProperty property) {

            if (property == null) return; 
            if (property.propertyType 
                    != SerializedPropertyType.ManagedReference) return;

            EditorGUIUtility.systemCopyBuffer = JsonConvert.SerializeObject(
                property.managedReferenceValue,
                Formatting.Indented,
                new JsonSerializerSettings() { 
                    Error = (s, a) => a.ErrorContext.Handled = true, 
                    PreserveReferencesHandling 
                        = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Objects,
                    ContractResolver = new ClipboardResolver() 
                }
            );
        }
        
        public static void FillFromValue(
            this SerializedProperty property, object value,
            bool doNotApply = false
        ) {
            if (property == null) return; 
            if (property.propertyType 
                    != SerializedPropertyType.ManagedReference) return;

            property.managedReferenceValue = value;

            if (!doNotApply)
                property.serializedObject.ApplyModifiedProperties(); 
        }

        public static object ParseFromClipboard(System.Type type) 
            => JsonConvert.DeserializeObject(
                EditorGUIUtility.systemCopyBuffer, type,
                new JsonSerializerSettings() { 
                    Error = (s, a) => a.ErrorContext.Handled = true, 
                    PreserveReferencesHandling 
                        = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.Objects,
                    ContractResolver = new ClipboardResolver()
                }
            );

        class UnityObjectConverter : JsonConverter {
            public override void WriteJson(
                JsonWriter writer, object value, JsonSerializer serializer
            ) {
                writer.WriteValue((value as Object).GetInstanceID());
            }

            public override object ReadJson(
                JsonReader reader, System.Type objectType, object existingValue, 
                JsonSerializer serializer
            ){
                // Debug.Log($"{reader.Value}, {reader.ValueType}");
                var instanceID = (System.Int64)reader.Value;
                return EditorUtility.InstanceIDToObject((int)instanceID);
            }

            public override bool CanConvert(System.Type objectType) {
                return objectType.IsSubclassOf(typeof(Object));
            }

        }

        class ClipboardResolver : DefaultContractResolver {
            protected override JsonContract CreateContract(System.Type objectType) {
                JsonContract contract = base.CreateContract(objectType);
                if (objectType.IsSubclassOf(typeof(Object)))
                    contract.Converter = new UnityObjectConverter();
                
                return contract;
            }
        }

        // ********************* Walking *******************************
        public static IEnumerable<SerializedProperty> 
        Children(this SerializedProperty property) {
            var it = property.Copy(); 
            var last = property.GetEndProperty(); 

            if (SerializedProperty.EqualContents(it, last)) yield break;
            if (!it.NextVisible(true) 
                || SerializedProperty.EqualContents(it, last)) yield break;
            do {
                yield return it.Copy();
            } while (it.NextVisible(false) 
                && !SerializedProperty.EqualContents(it, last));
        }

        public static IEnumerable<SerializedProperty> 
        Subproperties(this SerializedProperty property) {
            var stack = new Stack<SerializedProperty>();
            stack.Push(property.Copy());

            while (stack.TryPop(out var item)) {
                // Debug.Log(item.propertyPath);
                yield return item.Copy();

                if (item.isArray)
                    for (int i = item.arraySize - 1; i >= 0; --i)
                        stack.Push(item.GetArrayElementAtIndex(i));
                else
                    foreach (var child in item.Children().Reverse())
                        stack.Push(child);
            }

        }

        public struct SubReference {
            public SerializedProperty property;
            public SerializedProperty parent;
            public int depth;
        }

        public static IEnumerable<SubReference> 
        Subreferences(this SerializedProperty property) {
            var refs = new Stack<SubReference>();
            refs.Push(new () { property = property, depth = 0 });

            foreach (var sub in property.Subproperties()) {
                SubReference parent;

                while (refs.TryPeek(out parent) 
                    && sub.depth <= parent.property.depth)
                    refs.Pop();

                if (sub.IsManagedReference()) {
                    var rf = new SubReference() { 
                        property = sub,
                        parent = parent.property,
                        depth = refs.Count
                    };

                    refs.Push(rf);
                    yield return rf;
                }           
            }
        }

        // ********************* References *******************************
        public static IEnumerable<SerializedProperty> 
        GetAllLinkedProperties(this SerializedProperty property) {
            if (!property.IsManagedReference()) yield break;
            if (property.managedReferenceId <= 0) yield break;
            var it = property.serializedObject.GetIterator();
            do {
                if (!it.IsManagedReference()) continue;
                if (it.managedReferenceId == property.managedReferenceId)
                    yield return it.Copy();
            } while (it.Next(true));
        }

        public static SerializedReferenceLink
        GetLink(this SerializedProperty property) {
            if (!property.IsManagedReference()) 
                return SerializedReferenceLink.Null;
            return new() {
                instanceID = property.serializedObject.targetObject
                    .GetInstanceID(),
                referenceID = property.managedReferenceId
            };
        }

        public static int
        GetLinkedPropertiesCount(this SerializedProperty property) {
            if (!property.IsManagedReference()) 
                return 0;
                
            return SerializedReferenceUtility.GetLinkedPropertiesCount(
                property.serializedObject, property.managedReferenceId
            );
        }
    }
}
