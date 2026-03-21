using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    // TODO: Decide what to do with it
    [System.Serializable]
    public struct TypeName {

        public string assemblyQualifiedName;

        public TypeName(System.Type type)
            => assemblyQualifiedName = type?.AssemblyQualifiedName ?? null;

        public static implicit operator System.Type(TypeName st) 
            => System.Type.GetType(st.assemblyQualifiedName);

        public static implicit operator TypeName(System.Type type)
            => new TypeName(type);

        public override int GetHashCode() => assemblyQualifiedName.GetHashCode();
        public override bool Equals(object o) 
            => o is TypeName t ? Equals(t) : false;
        public bool Equals(TypeName t) 
            => t.assemblyQualifiedName == assemblyQualifiedName;
    }

    [CustomPropertyDrawer(typeof(TypeName))]
    public class TypeNamePropertyDrawer : PropertyDrawer {

        public override void OnGUI(
            Rect pos, SerializedProperty property, GUIContent label
        ) { 
            var propName = property.FindPropertyRelative("assemblyQualifiedName");
            var newLabel = EditorGUI.BeginProperty(pos, label, property);

            var type = System.Type.GetType(propName.stringValue);
            var name = type != null ? type.FullName : $"Missing type";
            
            EditorGUI.LabelField(pos, new GUIContent(name));
            EditorGUI.EndProperty();
        }

    }
}