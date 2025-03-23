using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEditor;
using UnityEditor.AssetImporters;
using System.IO;
using System.Text ;

namespace Jenga {

    // Wrong Way CSV importer
    // Imports comma-separated-values table and treats it as if it was
    // database. Column are marked by "Column:", unmarked columns, blank lines,
    // etc are ignored. Optionally links table with update link to
    // google sheets or smth like this and createes UI button in interface.
    [ScriptedImporter(version: 1, ext: "wwcsv")]
    [Icon("Packages/com.github.superjellie.jenga/Editor/Icons/database.png")]
    public class WWCSVImporter : ScriptedImporter {
        
        // [Header("Parser")]
        public string columnPrefix = "Column:";
        public string columnEnd    = "Column:End";
        public char   escape       = '\\';
        public char   separator    = ',';
        public char   quote        = '"';

        [Tooltip("Should unqouted text without separators be skipped")]
        public bool   skipUnquoted = false;

        // [Header("Linking")]
        [TextArea]
        public string[] links = { };

        // Preview
        public WWDatabaseAsset.Match[] previewMatches = { };
        public float previewColumnWidth  = 150f;
        public int   previewMaxLineLength = 80;
        public int   previewMaxEntries   = 50;
        public int   previewStartOnEntry = 0;
        
        public WWDatabaseAsset asset;
        public override void OnImportAsset(AssetImportContext ctx) {
            asset = ScriptableObject.CreateInstance<WWDatabaseAsset>();
            var text = ScriptableObject.CreateInstance<WWDatabaseAsset>();
            ctx.AddObjectToAsset("asset", asset);
            ctx.SetMainObject(asset);

            var lines = File.ReadAllLines(ctx.assetPath);
            var buildCtx = new WWDatabaseAsset.BuildContext() 
                { columnPrefix = columnPrefix, columnEnd = columnEnd };

            foreach (var line in lines) 
                buildCtx.AddLine(ReadCSVLine(line));

            asset.Build(buildCtx);
            EditorUtility.SetDirty(this);
        }

        List<string> ReadCSVLine(string line) {

            List<string> values = new();
            values.Add(""); // Add one word by default

            const int ST_UNQUOTED       = 0;
            const int ST_QUOTED_REGULAR = 1;
            const int ST_QUOTED_ESCAPE  = 2;
            
            int state = ST_UNQUOTED;

            for (int i = 0; i < line.Length; ++i) {

                // If we right after escape, just add character to last word
                if (state == ST_QUOTED_ESCAPE)
                    values[^1] += line[i];

                // If encounter escape inside quoted part, skip and 
                // enter ST_QUOTER_ESCAPE
                else if (state == ST_QUOTED_REGULAR && line[i] == escape)
                    state = ST_QUOTED_ESCAPE;

                // If inside quote encounter unescaped quote then 
                // skip and end quoted part
                else if (state == ST_QUOTED_REGULAR && line[i] == quote)
                    state = ST_UNQUOTED;

                // If inside quote and none of the above, then add char
                else if (state == ST_QUOTED_REGULAR)
                    values[^1] += line[i];

                // If outside of quotation encounter separator
                // then skip and add new word
                else if (state == ST_UNQUOTED && line[i] == separator)
                    values.Add("");

                // If outside of quotation encounter quote
                // then skip and add start quotation
                else if (state == ST_UNQUOTED && line[i] == quote)
                    state = ST_QUOTED_REGULAR;

                // If some unqouted text then either accept it or skip
                else if (state == ST_UNQUOTED)
                    { if (skipUnquoted) values[^1] += line[i]; }

                // all possible cases should be covered
            }

            return values;
        }
    }


    [CustomEditor(typeof(WWCSVImporter))]
    public class WWCSVImporterEditor : ScriptedImporterEditor {

        public bool parserFoldout = false;
        public bool statsFoldout = false;
        public bool previewFoldout = false;
        public Vector2 previewScroll;

        public int[] previewPointers = null;
        public override void OnInspectorGUI() {

            //
            var styTableEntry = new GUIStyle(GUI.skin.label);
            styTableEntry.wordWrap = true;

            //
            var so = serializedObject;
            var target = so.targetObject as WWCSVImporter;
            so.Update();

            if (GUILayout.Button("Sync with Linked Tables")) 
                SyncLinkedTables(target);

            // EditorGUI.BeginDisabledGroup(!propUpdating.boolValue);

            // Parser settings
            var propColumnPrefix = so.FindProperty("columnPrefix");
            var propColumnEnd    = so.FindProperty("columnEnd");
            var propEscape       = so.FindProperty("escape");
            var propSeparator    = so.FindProperty("separator");
            var propQuote        = so.FindProperty("quote");

            parserFoldout = EditorGUILayout
                .BeginFoldoutHeaderGroup(parserFoldout, "Parser Settings");

            if (parserFoldout) {
                EditorGUILayout.PropertyField(propColumnPrefix);
                EditorGUILayout.PropertyField(propColumnEnd);
                EditorGUILayout.PropertyField(propEscape);
                EditorGUILayout.PropertyField(propSeparator);
                EditorGUILayout.PropertyField(propQuote);
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            // Statistics
            var asset = so.FindProperty("asset")
                .objectReferenceValue as WWDatabaseAsset;

            statsFoldout = EditorGUILayout
                .BeginFoldoutHeaderGroup(statsFoldout, "Statistics");
            EditorGUI.BeginDisabledGroup(true);

            if (asset != null && statsFoldout) {
                EditorGUILayout
                    .IntField("Keys Count", asset.keys.Length);
                EditorGUILayout
                    .IntField("Lines Count", asset.pointers.Length);
            }

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndFoldoutHeaderGroup();

            // Linking
            var propLinks = so.FindProperty("links");
            EditorGUILayout.PropertyField(propLinks);

            EditorGUI.BeginChangeCheck();

            // Preview Settings
            var propPreviewColumnWidth = so.FindProperty("previewColumnWidth");
            var propPreviewMaxEntries  = so.FindProperty("previewMaxEntries");
            var propPreviewStartEntry  = so.FindProperty("previewStartOnEntry");
            var propPreviewMaxLine     = so.FindProperty("previewMaxLineLength");

            previewFoldout = EditorGUILayout.BeginFoldoutHeaderGroup(
                previewFoldout, "Preview Visuals"
            );
            if (previewFoldout) {
                EditorGUILayout.PropertyField(propPreviewColumnWidth);
                EditorGUILayout.PropertyField(propPreviewMaxEntries);
                EditorGUILayout.PropertyField(propPreviewStartEntry);
                EditorGUILayout.PropertyField(propPreviewMaxLine);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            //
            var propPreviewMatches = so.FindProperty("previewMatches");
            EditorGUILayout.PropertyField(propPreviewMatches);

            var previewMatches = target.previewMatches;

            // Preview Table
            if (EditorGUI.EndChangeCheck() || previewPointers == null) {
                so.ApplyModifiedProperties();
                
                if (previewMatches.Length > 0) {
                    var pointersSet = asset.MatchPointers(previewMatches);

                    if (pointersSet != null) {
                        previewPointers = new int[pointersSet.Count];
                        pointersSet.CopyTo(previewPointers); 
                        System.Array.Sort(previewPointers);
                    } else previewPointers = new int[0];
                } else {
                    previewPointers = new int[asset.pointers.Length];
                    for (int i = 0; i < previewPointers.Length; ++i)
                        previewPointers[i] = i;
                }
            }
            

            previewScroll = EditorGUILayout.BeginScrollView(
                previewScroll, GUILayout.MaxHeight(500f)
            );

            var rectHeader = EditorGUILayout.BeginHorizontal();
            EditorGUI.DrawRect(rectHeader, new Color(.3f, .3f, .5f, 1f));
            for (int column = 0; column < asset.keys.Length; ++column) {
                var key = asset.keys[column];
                EditorGUILayout.LabelField(
                    key, GUILayout.Width(propPreviewColumnWidth.floatValue)
                );
            }
            EditorGUILayout.EndHorizontal();

            if (propPreviewStartEntry.intValue > 0)
                EditorGUILayout.LabelField(
                    $"HIDDEN {propPreviewStartEntry.intValue} entries",
                    EditorStyles.boldLabel
                );


            for (int i = propPreviewStartEntry.intValue; 
                i < previewPointers.Length 
                && i - propPreviewStartEntry.intValue 
                    < propPreviewMaxEntries.intValue; 
                ++i
            ) {
                var rectLine = EditorGUILayout.BeginHorizontal();
                EditorGUI.DrawRect(
                    rectLine, 
                    i % 2 == 0 ? Color.white * .3f : Color.white * .5f
                );

                var ptr = previewPointers[i];

                for (int column = 0; column < asset.keys.Length; ++column) {
                    var index = asset.pointers[ptr].indicesInSorted[column];

                    var value = index > -1 
                        ? asset.columnsValues[column].data[index].data
                        : null;

                    GUILayout.Label(
                        string.IsNullOrEmpty(value) 
                            ? "" 
                            : value.Substring(0, 
                                Mathx.Min
                                    (propPreviewMaxLine.intValue, value.Length)
                            ),
                        styTableEntry,
                        GUILayout.Width(propPreviewColumnWidth.floatValue)
                    );
                }

                EditorGUILayout.EndHorizontal();

            }
            
            EditorGUILayout.EndScrollView();

            var excessEntries = previewPointers.Length 
                - propPreviewStartEntry.intValue
                - propPreviewMaxEntries.intValue;

            if (excessEntries > 0)
                EditorGUILayout.LabelField(
                    $"HIDDEN {excessEntries} entries",
                    EditorStyles.boldLabel
                );

            //
            so.ApplyModifiedProperties();
            ApplyRevertGUI();
        }

        // This is blocking function, by design
        void SyncLinkedTables(WWCSVImporter importer) {

            if (importer.links.Length == 0) return;

            EditorUtility.DisplayProgressBar(
                "Updating Wrong Way CSV",
                "Sending Web Requests...",
                0f
            );

            var requests = new UnityWebRequest[importer.links.Length];
            for (int i = 0; i < importer.links.Length; ++i) {
                requests[i] = UnityWebRequest.Get(importer.links[i].Trim());
                requests[i].SendWebRequest();
            }

            var numDone = 0;
            while (numDone < requests.Length) {

                System.Threading.Thread.Sleep(200);

                numDone = 0;
                var totalProgress = 0f;
                for (int i = 0; i < requests.Length; ++i) {
                    totalProgress += requests[i].downloadProgress;
                    if (requests[i].isDone)
                        numDone++;
                }

                EditorUtility.DisplayProgressBar(
                    "Updating Wrong Way CSV",
                    $"Waiting for Web Requests"
                    + $" ({requests.Length - numDone} of {requests.Length})...",
                    .8f * (totalProgress / requests.Length)
                );
            }

            EditorUtility.DisplayProgressBar(
                "Updating Wrong Way CSV",
                "Writing to file",
                .9f
            );


            var text = new StringBuilder();
            foreach (var req in requests) {
                text.Append(req.downloadHandler.text);

                // Add column ends for correcteness
                text.Append("\n");
                for (int i = 0; i < 100; ++i) 
                    text.Append($"\"{importer.columnEnd}\",");
                

                text.Append("\n");
            }

            var assetPath = AssetDatabase.GetAssetPath(importer.asset);
            var path = Path.Combine(Application.dataPath, "..", assetPath);
            File.WriteAllText(path, text.ToString());

            EditorUtility.ClearProgressBar();
            AssetDatabase.Refresh();
        }

    }


    [CustomPropertyDrawer(typeof(WWDatabaseAsset.Match))]
    public class WWDatabaseAssetMatchPropertyDrawer : PropertyDrawer {
        public override void OnGUI
            (Rect pos, SerializedProperty prop, GUIContent label) {

            EditorGUI.BeginProperty(pos, label, prop);
            pos = EditorGUI.PrefixLabel(pos, label);

            var propKey = prop.FindPropertyRelative("key");
            var propVal = prop.FindPropertyRelative("value");

            var w = pos.width; var h = pos.height;
            var posKey = new Rect(pos.x, pos.y, 100f, h);
            var posVal = new Rect(pos.x + 100f, pos.y, w - 100f, h);

            propKey.stringValue 
                = EditorGUI.TextField(posKey, propKey.stringValue);
            propVal.stringValue 
                = EditorGUI.TextField(posVal, propVal.stringValue);
            EditorGUI.EndProperty();
        }
    }
}
