#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    [CustomPropertyDrawer(typeof(EnumValue))]
    public class EnumValuePropertyDrawer : PropertyDrawer {

        public override void 
        OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {
            var propAsset = prop.FindPropertyRelative("asset");
            var propValue = prop.FindPropertyRelative("value");
            var asset = propAsset.objectReferenceValue as EnumAsset;
            var value = propValue.stringValue;

            EditorGUI.BeginProperty(pos, label, prop);
            pos = EditorGUI.PrefixLabel(pos, label);

            var rectAsset = pos.LeftCut(100f, out var rectPopup);

            EditorGUI.PropertyField(rectAsset, propAsset);
            var cnt = new GUIContent(propValue.stringValue);

            var oldColor = GUI.color;
            GUI.color = asset != null && asset.values.Contains(value) 
                ? GUI.color
                : Color.red; 

            var doDrop 
                = EditorGUI.DropdownButton(rectPopup, cnt, FocusType.Passive);
            GUI.color = oldColor;
            
            if (doDrop && asset != null) {
                var menu = new GenericMenu(); 
                
                foreach (var v in asset.values)
                    menu.AddItem(new GUIContent(v), v == value, 
                        () => {
                            propValue.stringValue = v;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                    );

                menu.DropDown(rectPopup);
            }


            EditorGUI.EndProperty();
        }

    }
}

#endif