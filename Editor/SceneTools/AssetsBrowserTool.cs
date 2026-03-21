using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    [ToolChannel]
    public class AssetsBrowserTool : SceneTool {

        public override string category => "Assets";
        public override string title => "Browser";
        public override int order => 200;

        public override ToolChannelState[] channelStates => new ToolChannelState[] {
            new("Category", currentCategory),
            new("Asset", selectedGUID)
        };

        public string assetPath = $"Assets/Prefabs/";
        public string typeFilter = $"GameHouse";

        public Vector2 scrollPosition;

        public float previewSize = 50f;
        public float categorySize = 100f;
        public int   maxRows     = 5;

        public string currentCategory = null;
        public string selectedGUID = null;
        public string[] guids = { };
        public string[] categories = { };
        public string filter = "";

        public bool isChanged = true;

        public override void OnToolCustomGUI(
            EditorWindow win, SerializedObject so
        ) {
            if (isChanged)
                Search(so);

            var size = 50f;
            var previewWidth = GUILayout.Width(previewSize);
            var previewHeight = GUILayout.Height(previewSize);

            var propScrollPos = so.FindProperty("scrollPosition");
            var propCurrentCat = so.FindProperty("currentCategory");
            var propSelectedGUID = so.FindProperty("selectedGUID");
            var propFilter = so.FindProperty("filter");

            var stySelected = new GUIStyle(EditorStyles.selectionRect);
            stySelected.margin  = GUI.skin.box.margin;
            stySelected.padding = GUI.skin.box.padding;

            var columns  
                = (int)(EditorGUIUtility.currentViewWidth / (size + 5f));
            // var previewMargin = 

            EditorGUI.indentLevel--;
            EditorGUILayout.BeginHorizontal();
            propFilter.stringValue = EditorGUILayout.TextField(
                "", propFilter.stringValue
            ).ToLower();

            if (GUILayout.Button("Clear", GUILayout.Width(50f)))
                filter = "";
            EditorGUILayout.EndHorizontal();
            EditorGUI.indentLevel++;

            if (!AQRY.Contains<string>(categories, (x, i) => x == currentCategory) 
                    && categories.Length > 0
            ) currentCategory = categories[0];

            var selected = System.Array.FindIndex(
                categories, (x) => x == currentCategory
            );

            selected = GUILayout.SelectionGrid(
                selected, categories, 
                (int)(EditorGUIUtility.currentViewWidth / categorySize)
            );

            if (selected >= 0 && selected < categories.Length)
                propCurrentCat.stringValue = categories[selected];
            

            propScrollPos.vector2Value = 
                EditorGUILayout.BeginScrollView(
                    propScrollPos.vector2Value, 
                    GUILayout.MinHeight(previewSize * maxRows)
                );

            if (AQRY.Contains<string>(guids, (x, i) => x == selectedGUID)) {
                EditorGUILayout.BeginHorizontal();
                var path = AssetDatabase.GUIDToAssetPath(selectedGUID);
                var asset = AssetDatabase.LoadMainAssetAtPath(path);
                GUILayout.Label(asset.name);
                
                if (GUILayout.Button("Locate", GUILayout.Width(60f))) {
                    EditorUtility.FocusProjectWindow(); 
                    Selection.activeObject = asset;
                }

                EditorGUILayout.EndHorizontal();
            }

            // Ehm... oh well (:
            // EditorGUILayout.BeginHorizontal();
            // GUILayout.FlexibleSpace();
            // EditorGUILayout.BeginVertical();
            for (int i = 0; i < guids.Length; ) {
                EditorGUILayout.BeginHorizontal();

                for (int j = 0; j < columns && i < guids.Length; i++) {
                    var guid = guids[i];
                    var cat = CategoryOf(guid);

                    if (cat != currentCategory && currentCategory != null)
                        continue;

                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    if (!path.ToLower().Contains(filter)) continue;

                    if (AssetDatabase.IsValidFolder(path))
                        continue;

                    var asset = AssetDatabase.LoadMainAssetAtPath(path);
                    var preview = AssetPreview.GetAssetPreview(asset);

                    if (GUILayout.Button(
                        preview, 
                        selectedGUID == guid
                             ? stySelected 
                             : GUI.skin.box,
                        previewWidth, previewHeight
                    )) {
                        propSelectedGUID.stringValue = guid;
                    }

                    j++;
                }
                EditorGUILayout.EndHorizontal();
            }
            // EditorGUILayout.EndVertical();
            // GUILayout.FlexibleSpace();
            // EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndScrollView();

        }

        public override void OnActivate() => isChanged = true;

        public string CategoryOf(string guid) {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var cat = System.IO.Path.GetFileName(
                System.IO.Path.GetDirectoryName(path)
                    .TrimEnd(System.IO.Path.DirectorySeparatorChar)
            );
            return ObjectNames.NicifyVariableName(cat);
        }

        public void Search(SerializedObject so) {
            var myGUIDs = AssetDatabase.FindAssets(
                $"t:Object", new string[] { assetPath }
            );

            System.Array.Sort(myGUIDs, (x, y) => {
                var pathX = AssetDatabase.GUIDToAssetPath(x);
                var pathY = AssetDatabase.GUIDToAssetPath(y);
                var nameX = System.IO.Path.GetFileName(pathX);
                var nameY = System.IO.Path.GetFileName(pathY);
                return nameX.CompareTo(nameY);
            });

            var cats = new HashSet<string>();

            var isDifferent = myGUIDs.Length != guids.Length;

            for (int i = 0; i < myGUIDs.Length; ++i) {
                if (i < guids.Length && myGUIDs[i] != guids[i])
                    isDifferent = true;
                cats.Add(CategoryOf(myGUIDs[i]));
            }

            var catsArray = new string[cats.Count];
            cats.CopyTo(catsArray);
            System.Array.Sort(catsArray);

            if (isDifferent) {
                Undo.RecordObject(this, "Update GUIDs");
                guids = myGUIDs;
                categories = catsArray;
            }

            isChanged = false;
        }

    }
}