#if UNITY_EDITOR
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public static class SerializedPropertyUtility {

    public static StyleSheet customStyle 
        = AssetDatabase.LoadAssetAtPath<StyleSheet>(
            "Packages/com.github.superjellie.jenga/"
            + "Editor/custom-editor-styles.uss"
        );

    public static string ussNoLabelPropertyClassName 
        = "custom--no-label-property";

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
}
#endif