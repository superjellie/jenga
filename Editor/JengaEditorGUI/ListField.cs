using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {

        public delegate Rect ArrayItemLayouter(int index);
        public delegate void ArrayItemDrawer(Rect position, int index);
        public delegate void ArrayResizer(int oldSize, int newSize);

        public static Rect ListField(
            Rect position, string name, int size, string id, float maxHeight, 
            ArrayItemLayouter layouter, ArrayItemDrawer drawer,
            ArrayResizer resizer
        ) {
            JengaEditorGUI.BeginDataGroup(id);
            var foldout = GetDataValueOrDefault<bool>("foldout", false);
            var scroll = GetDataValueOrDefault<Vector2>("scroll", Vector2.zero);

            var totalRect = new Rect();

            totalRect.width = position.width - 15f;
            totalRect.height = 10f;

            for (int i = 0; i < size; ++i) {
                var r = layouter(i);
                // totalRect.width = Mathf.Max(totalRect.width, r.width);
                totalRect.height += r.height;
            }

            var rectHeader = position.LineCut(out position); 
            var rectSize = rectHeader.RightCut(50f, out rectHeader); 

            var rect = position
                .TopCut(Mathf.Min(maxHeight, totalRect.height), out position);


            foldout = EditorGUI.Foldout(rectHeader, foldout, name);
            var newSize = EditorGUI.DelayedIntField(rectSize, size);

            EditorGUI.indentLevel++;

            if (foldout) {
                EditorGUI.HelpBox(rect.IndentCut(), "", MessageType.None);

                scroll = GUI.BeginScrollView(rect, scroll, totalRect);
                totalRect.TopCut(5f, out totalRect);
                for (int i = 0; i < size; ++i) {
                    var wantRect = layouter(i);
                    var drawRect = totalRect
                        .TopCut(wantRect.height, out totalRect);
                    drawer(drawRect, i);
                }
                GUI.EndScrollView(); 

                var lineRect = position.LineCut(out position);
                lineRect.RightCut(10f, out lineRect);
                var btnRemoveRect = lineRect.RightCut(30f, out lineRect);
                var btnAddRect = lineRect.RightCut(30f, out lineRect);

                if (GUI.Button(btnRemoveRect, "-")) newSize = size - 1;
                if (GUI.Button(btnAddRect, "+")) newSize = size + 1;
            }
            EditorGUI.indentLevel--;
            
            if (size != newSize)
                resizer(size, newSize);

            SetDataValue("foldout", foldout);
            SetDataValue("scroll", scroll);
            JengaEditorGUI.EndDataGroup();

            return position;
        }
        public static Rect ListField(
            Rect position, SerializedProperty property, float maxHeight
        ) => ListField(
            position, property.GetDisplayName(), property.arraySize,
            property.GetEditorDataKey(), 
            maxHeight, 
            layouter: (i) => {
                var height = EditorGUI.GetPropertyHeight(
                    property.GetArrayElementAtIndex(i), ShouldShowChildren()
                );
                var width = EditorGUIUtility.currentViewWidth;
                return new Rect(0f, 0f, width, height);
            },
            drawer: (rect, i) => EditorGUI.PropertyField(
                rect, property.GetArrayElementAtIndex(i), 
                ShouldShowChildren()
            ),
            resizer: (oldSize, newSize) => {
                for (int i = oldSize - 1; i >= newSize; --i)
                    property.DeleteArrayElementAtIndex(i);
                for (int i = oldSize; i < newSize; ++i)
                    property.InsertArrayElementAtIndex(i);
            }
        );
    }
}
