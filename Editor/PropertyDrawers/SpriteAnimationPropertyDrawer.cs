#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;

namespace Jenga {

    [CustomPropertyDrawer(typeof(SpriteAnimation))]
    public class SpriteAnimationPropertyDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI(
            SerializedProperty prop
        ) {
            var propTex = prop.FindPropertyRelative("texture");
            var propFPS = prop.FindPropertyRelative("fps");
            var propSprites = prop.FindPropertyRelative("sprites");

            var root = new VisualElement() { 
                style = { flexDirection = FlexDirection.Row }
            };

            var veTexture = new PropertyField() {
                bindingPath = propTex.propertyPath,
                label = preferredLabel,
                style = { flexGrow = 1f }
            };

            veTexture.RegisterValueChangeCallback(evt => {
                propSprites.ClearArray(); 

                var obj = propTex.objectReferenceValue;
                if (obj != null) {
                    var path = AssetDatabase.GetAssetPath(obj);
                    var dt = AssetDatabase
                        .LoadAllAssetRepresentationsAtPath(path);

                    foreach (var o in dt)
                        if (o is Sprite sprite) {
                            var len = propSprites.arraySize;
                            propSprites.InsertArrayElementAtIndex(len);
                            var item = propSprites.GetArrayElementAtIndex(len);
                            item.objectReferenceValue = o;
                        }
                }

                prop.serializedObject.ApplyModifiedProperties();
            });

            root.Add(veTexture);

            root.Add(new PropertyField() {
                bindingPath = propFPS.propertyPath,
                label = "",
                style = { flexGrow = 1f, maxWidth = 30f }
            });

            return root;
        }
    }
}
#endif