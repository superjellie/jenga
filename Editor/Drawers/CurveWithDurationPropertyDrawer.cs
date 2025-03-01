#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// [CustomPropertyDrawer(typeof(CurveWithDuration))]
public class CurveWithDurationPropertyDrawer : PropertyDrawer {

    public override void OnGUI(
        Rect pos, SerializedProperty prop, GUIContent label
    ) {
        EditorGUI.BeginProperty(pos, label, prop);

        var propCurve = prop.FindPropertyRelative("curve");
        var propDuration = prop.FindPropertyRelative("duration");

        pos = EditorGUI.PrefixLabel(pos, label);

        var x = pos.x; var y = pos.y; var w = pos.width; var h = pos.height;

        var rectCurve = new Rect(x, y, w - 40f, h);
        var rectDuration = new Rect(x + w - 40f, y, 40f, h);

        EditorGUI.PropertyField(rectCurve, propCurve, GUIContent.none);
        EditorGUI.PropertyField(rectDuration, propDuration, GUIContent.none);

        EditorGUI.EndProperty();
    }
}
#endif