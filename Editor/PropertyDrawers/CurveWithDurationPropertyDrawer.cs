using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

    [CustomPropertyDrawer(typeof(CurveWithDuration))]
    public class CurveWithDurationPropertyDrawer : PropertyDrawer {

        public override void OnGUI(
            Rect position, SerializedProperty property, GUIContent label
        ) {
            EditorGUI.BeginProperty(position, label, property);

            position = EditorGUI.PrefixLabel(
                position, GUIUtility.GetControlID(FocusType.Passive), label
            );

            var propCurve = property.FindPropertyRelative("curve");
            var propDur   = property.FindPropertyRelative("duration");

            var rectDur   = position.RightCut(40f, out position);
            var rectCurve = position;

            EditorGUI.PropertyField(rectCurve, propCurve, GUIContent.none);
            EditorGUI.PropertyField(rectDur, propDur, GUIContent.none);

            EditorGUI.EndProperty();
        }

    }

}
