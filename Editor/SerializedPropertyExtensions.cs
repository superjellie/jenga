using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static class SerializedPropertyExtensions {
        public static SerializedProperty FindPropertyOnParent(
            this SerializedProperty property, string path
        ) {
            var index = property.propertyPath.LastIndexOf('.');

            var newPath = index >= 0 
                ? $"{property.propertyPath.Substring(0, index + 1)}{path}"
                : path;
                
            return property.serializedObject.FindProperty(newPath);
        }
    }
}
