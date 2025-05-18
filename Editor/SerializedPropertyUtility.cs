#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

namespace Jenga {
    public static class SerializedPropertyUtility {

        public static Color[] colors = {
            Color.white,
            Color.red,
            Color.green,
            Color.blue,
            Color.green,
            Color.yellow,
            Color.magenta,
            Color.cyan,
            ColorFromHex("#FF5733"),
            ColorFromHex("#bfc9ca"),
            ColorFromHex("#bb8fce"),
            ColorFromHex("#dc7633"),
        };

        public static Color ColorFromHex(string hex) {
            if (ColorUtility.TryParseHtmlString(hex, out var color))
                return color;
            return Color.black;
        }

        public static Color ColorFromId(long managedID) 
            => colors[Mathx.Abs((int)(managedID % colors.Length))];

        public static StyleSheet jengaStyle 
            = AssetDatabase.LoadAssetAtPath<StyleSheet>(
                "Packages/com.github.superjellie.jenga/"
                + "Editor/jenga-editor-styles.uss"
            );

        public static string ussNoLabelsInChildrenClassName 
            = "jenga--no-labels-in-children";
        // public static string ussHideHeaderClassName 
        //     = "jenga--hide-header";

        public static System.Type GetFieldType(FieldInfo info) {
            var type = info.FieldType;

            return 
                type.IsArray 
                    ? type.GetElementType()
                : type.IsGenericType 
                    && type.GetGenericTypeDefinition() == typeof(List<>) 
                    ? type.GenericTypeArguments[0]
                : type; 
        }


        public static System.Type GetManagedType(SerializedProperty prop) {
            if (prop.propertyType != SerializedPropertyType.ManagedReference)
                return null;
            if (prop.managedReferenceId < 0)
                return null;

            var typeName = prop.managedReferenceFullTypename;

            if (string.IsNullOrEmpty(typeName)) return null;

            var splitIndex = typeName.IndexOf(' ');
            var assembly = Assembly.Load(typeName.Substring(0, splitIndex));
            return assembly.GetType(typeName.Substring(splitIndex + 1));
        }


        public static void SetManagedReference(
            SerializedProperty property, System.Type type
        ) { 
            object result = null;

            if (type != null && property.managedReferenceValue != null) {
                
                // Preserve serialized values if we can 
                // ... no better not to preserve)))
                // string json = JsonUtility.ToJson(property.managedReferenceValue);
                // result = JsonUtility.FromJson(json, type);

            }

            if (result == null) {
                result = type != null 
                    ? System.Activator.CreateInstance(type) 
                    : null;
            }
            
            property.managedReferenceValue = result;
        }

        public static System.Type GetValueType(SerializedProperty property) {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
                return GetManagedType(property);
            else
                return property.boxedValue?.GetType(); 
        }
        public static object GetValue(SerializedProperty property) {
            if (property.propertyType == SerializedPropertyType.ManagedReference)
                return property.managedReferenceValue;
            else
                return property.boxedValue; 
        }

        public static System.Action<T> GetMethodByName<T>(
            SerializedProperty property, string name
        ) {
            var type = GetValueType(property);
            if (type == null) return null;

            var methodInfo = type.GetMethod(name);
            if (methodInfo == null) return null;

            return (t) => methodInfo
                .Invoke(GetValue(property), new object[] { t });
        }
    }
}
#endif