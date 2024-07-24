using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Jenga;

using System.Text.RegularExpressions;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

[AddComponentMenu("Jenga/GoogleTable", 10)]
public class GoogleTable : MonoBehaviour {  

    [System.Serializable]
    public class GoogleTableLink {
        public string tableKey;
        public bool isSheet = false;
        public string sheetKey;

        public string lastCsv = "";
        public UnityWebRequest currentRequest = null;

        public string GetLink() {
            if (isSheet)
                return $"https://docs.google.com/spreadsheets/d/{tableKey}"
                    +  $"/gviz/tq?tqx=out:csv&gid={sheetKey}&headers=0";
            else
                return $"https://docs.google.com/spreadsheets/d/{tableKey}"
                    +  $"/gviz/tq?tqx=out:csv&headers=0";
        }

    }

    public GoogleTableLink[] urls;
    public bool updating = false;

    [System.Serializable]
    public class Entry {
        public string[] values = {};
        public string[] keys = {};

        public string Get(string key, string dflt) {
            var (x, i) = AQRY.Search<string>(keys, (x, i) => x == key);
            return i >= 0 ? x : dflt;
        }

        public T Get<T>(string key, T dflt) {
            var str = Get(key, null);
            if (str == null) return dflt;

            try {
                var value = System.Convert.ChangeType(str, typeof(T));
                if (value is T x) return x;
                return dflt;
            } catch {
                return dflt;
            }
        }
    }

    [HideInInspector, SerializeField] 
    public Entry[] table = {};

#if UNITY_EDITOR
    public void SendLoadRequests() {
        updating = true;
        foreach (var url in urls) {
            url.currentRequest = UnityWebRequest.Get(url.GetLink());
            url.currentRequest.SendWebRequest();
        }

        EditorUtility.SetDirty(this);
    }    

    public bool IsAllRequestsLoaded() {
        foreach (var url in urls) {
            if (url.currentRequest != null && !url.currentRequest.isDone)
                return false;

            // switch (req.result) {
            // case UnityWebRequest.Result.ConnectionError:
            //     Debug.LogWarning($"Cant connect to {url}");
            //     continue;
            // case UnityWebRequest.Result.DataProcessingError:
            // case UnityWebRequest.Result.ProtocolError:
            //     Debug.LogWarning(
            //         $"Error when downloading form {url}: {req.error}"
            //     );
            //     continue;
            // case UnityWebRequest.Result.Success:
            //     break;
            // }
        }
        return true;
    }


    public void UpdateTable() {

        var tableList = new List<Entry>();
        foreach (var url in urls) {
            string csv = url.currentRequest.downloadHandler.text;
            url.lastCsv = csv;
            var lines = ParseCSV(csv);

            var keys = new List<string>();

            foreach (var line in lines) {
                for (int i = keys.Count; i < line.Count; ++i) 
                    keys.Add("");
                for (int i = 0; i < line.Count; ++i)
                    if (line[i].StartsWith("Column:"))
                        keys[i] = line[i];

                var myKeys = AQRY.Where<string>(keys.ToArray(), 
                    (x, i) => keys[i].Length > 0 
                        && !line[i].StartsWith("Column:")
                );
                var myValues = AQRY.Where<string>(line.ToArray(), 
                    (x, i) => i < keys.Count 
                        && keys[i].Length > 0 && !line[i].StartsWith("Column:")
                );

                if (keys.Count > 0)
                    tableList.Add(
                        new Entry() {
                            values = myValues.Copy(),
                            keys = myKeys.Copy()
                        }
                    );

            }
        }

        table = tableList.ToArray();
        updating = false;
        EditorUtility.SetDirty(this);

    }
#endif

    private List<List<string>> ParseCSV(string csv) {
        var res = new List<List<string>>();
        bool isString = false;
        string lastItem = "";
        List<string> lastLine = new List<string>();
        for (int i = 0; i < csv.Length; ++i) {
            var c = csv[i];
            var next = i + 1 < csv.Length ? csv[i + 1] : 'x';
            if (c == '"' && !isString) { isString = true; }
            else if (isString && c == '"' && next == '"') {
                lastItem += '"'; ++i;
            } else if (c == '"' && isString) { isString = false; }
            else if (c == ',' && !isString) { 
                lastLine.Add(lastItem); 
                lastItem = ""; 
            } else if (c == '\n' && !isString) { 
                if (lastItem.Length > 0) lastLine.Add(lastItem); 
                if (lastLine.Count > 0) res.Add(lastLine); 
                lastItem = "";
                lastLine = new List<string>();
            } 
            else lastItem += c;
        }

        if (lastItem.Length > 0) lastLine.Add(lastItem);
        if (lastLine.Count > 0) res.Add(lastLine);

        return res;
    }


}

#if UNITY_EDITOR
[CustomEditor(typeof(GoogleTable))]
public class GoogleTableEditor : Editor {

    public Vector2 scrollPos = Vector2.zero;
    public float itemWidth = 100f;
    public float itemHeight = 10f;
    public float loadStartTime = 0f;

    public override void OnInspectorGUI() {
        GoogleTable db = target as GoogleTable;

        var propUrls = serializedObject.FindProperty("urls");
        EditorGUILayout.PropertyField(propUrls, true);

        if (!db.updating) {
            if (GUILayout.Button("Update Database")) {
                db.SendLoadRequests();
                loadStartTime = Time.time;
            }
        } else GUILayout.Button("Updating...");

        if (db.updating && db.IsAllRequestsLoaded()) db.UpdateTable();


        var styKeys   = EditorStyles.boldLabel;
        var styValues = EditorStyles.label;

        itemWidth  = EditorGUILayout.FloatField("Item Width", itemWidth);
        itemHeight = EditorGUILayout.FloatField("Item Height", itemHeight);

        var lastKeys = new string[0];
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        foreach (var entry in db.table) {

            var isKeysNew = false;
            if (lastKeys.Length != entry.keys.Length)
                isKeysNew = true;
            else for (int i = 0; i < entry.keys.Length; ++i)
                if (entry.keys[i] != lastKeys[i]) {
                    isKeysNew = true;
                    break;
                }

            if (isKeysNew) {
                lastKeys = entry.keys;
                EditorGUILayout.BeginHorizontal();
                for (int i = 0; i < entry.keys.Length; ++i) {
                    var key = lastKeys[i];
                    EditorGUILayout.LabelField(
                        key, styKeys, 
                        GUILayout.Width(itemWidth), GUILayout.Height(itemHeight)
                    );
                } 
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < entry.values.Length; ++i) {
                var value = entry.values[i];
                EditorGUILayout.LabelField(
                    value, styValues, 
                    GUILayout.Width(itemWidth), GUILayout.Height(itemHeight)
                );
            } 
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();

        serializedObject.ApplyModifiedProperties();
    }
}
#endif