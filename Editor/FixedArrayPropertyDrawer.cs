using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {

	[CustomPropertyDrawer(typeof(FixedArray<>), true)]
	public class FixedArrayPropertyDrawer : PropertyDrawer {

		bool foldout = false;
		float lineHeight => EditorGUIUtility.singleLineHeight;
		float spacing => EditorGUIUtility.standardVerticalSpacing;
		float lastHeight = 0f;

		public override void OnGUI(
			Rect pos, SerializedProperty property, GUIContent label
		) {
        	var propArray = property.FindPropertyRelative("array");

        	label = EditorGUI.BeginProperty(pos, label, property);

        	var posHeader = new Rect(pos.x, pos.y, pos.width, lineHeight);

        	foldout = EditorGUI.BeginFoldoutHeaderGroup(
        		posHeader, foldout, label
        	);
        	EditorGUI.indentLevel++;

        	var posy = pos.y + lineHeight + spacing;

        	if (foldout)
        	for (int i = 0; i < propArray.arraySize; ++i) {
        		var propItem = propArray.GetArrayElementAtIndex(i);
        		var height = EditorGUI.GetPropertyHeight(propItem, true);
        		var posItem = new Rect(pos.x, posy, pos.width, height);
        		EditorGUI.PropertyField(posItem, propItem, true);

        		posy += height + spacing;
        	}

        	EditorGUI.indentLevel--;
        	EditorGUI.EndFoldoutHeaderGroup();
        	EditorGUI.EndProperty();

        	lastHeight = posy - spacing;
    	}

    	public override float GetPropertyHeight(
    		SerializedProperty prop, GUIContent label
    	) => lastHeight;
	} 


}