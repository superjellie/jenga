using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    [CustomPropertyDrawer(typeof(ILayoutMe))]
    public class ILayoutMePropertyDrawer : PropertyDrawer {

        public static IEnumerable<MemberInfo> VisibleMembers(System.Type type) {
            return type.FindMembers(
                MemberTypes.Field | MemberTypes.Method,
                BindingFlags.Static | BindingFlags.Instance
                | BindingFlags.Public | BindingFlags.NonPublic,
                (memb, o) => 
                    memb is FieldInfo fi && !fi.IsStatic
                        && fi.FieldType.IsSerializable
                        && !fi.HasCustomAttribute<HideInInspectorAttribute>()
                    || memb is MethodInfo mi && mi.IsStatic
                        && !mi.HasCustomAttribute<HideInInspectorAttribute>()
                        && mi.HasCustomAttribute<MethodAttribute>()
            );
        }
    
        public override void 
        OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {

            EditorGUI.BeginProperty(pos, label, prop);

            var line = pos.LineCut(out pos);
            var rectFoldout = line.LeftCut(15f);
            line = EditorGUI.PrefixLabel(
                line, GUIUtility.GetControlID(FocusType.Passive), label
            );

            prop.isExpanded 
                = EditorGUI.Foldout(rectFoldout, prop.isExpanded, "");

            if (prop.isExpanded) {
                EditorGUI.indentLevel++;
                foreach (var child in prop.DirectChildren()) {
                    var h = EditorGUI.GetPropertyHeight(child);
                    var rect = pos.TopCut(h, out pos);
                    EditorGUI.PropertyField(rect, child);
                }
                EditorGUI.indentLevel--;   
            }


            EditorGUI.EndProperty();
        }

        public override float 
        GetPropertyHeight(SerializedProperty prop, GUIContent label) {
            var h = EditorGUIUtility.singleLineHeight;

            if (prop.isExpanded)
                foreach (var child in prop.DirectChildren()) {
                    h += EditorGUI.GetPropertyHeight(child);
                }

            return h;
        }


    }
}
