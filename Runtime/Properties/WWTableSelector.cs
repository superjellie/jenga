using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jenga;
using System.Buffers;
#if UNITY_EDITOR
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
#endif

[System.Serializable]
public class WWDatabaseSelector {
    public WWDatabaseAsset asset;
    // public WWDatabaseAsset.Match[] defaultMatchers = { };
    public WWDatabaseAsset.Match[] userMatchers = { };

    public WWDatabaseSelector(params WWDatabaseAsset.Match[] userMatchers) {
        this.userMatchers = userMatchers;
    }

    public WWDatabaseSelector(
        WWDatabaseAsset asset, WWDatabaseAsset.Match[] userMatchers
    ) {
        this.asset = asset;
        this.userMatchers = userMatchers;
    }


    public WWDatabaseSelector(string key) {
        this.userMatchers = new WWDatabaseAsset.Match[] { new(key, "") };
    }

    public WWDatabaseSelector(string key, string value) {
        this.userMatchers = new WWDatabaseAsset.Match[] { new(key, value) };
    }

    public WWDatabaseSelector(WWDatabaseSelector other) {
        asset = other.asset;
        userMatchers = new WWDatabaseAsset.Match[other.userMatchers.Length];

        for (int i = 0; i < userMatchers.Length; ++i)
            userMatchers[i] = other.userMatchers[i];
    }

    // public WWDatabaseSelector(WWDatabaseAsset asset, string key, string value) {
    //     this.userMatchers = new WWDatabaseAsset.Match[] { new(key, value) };
    //     this.asset = asset;
    // }

    public string[] GetData(
        WWDatabaseAsset.Match[] additionalMatchers, string[] columns
    ) {
        var matchers = ArrayPool<WWDatabaseAsset.Match>.Shared
            .Rent(userMatchers.Length + additionalMatchers.Length);

        userMatchers.CopyTo(matchers, 0);
        additionalMatchers.CopyTo(matchers, userMatchers.Length);

        var result 
            = asset?.GetData(matchers, columns) ?? new string[columns.Length];

        ArrayPool<WWDatabaseAsset.Match>.Shared.Return(matchers);            
        return result;
    }

    public int[] GetPointers(params WWDatabaseAsset.Match[] additionalMatchers) {
        var matchers = ArrayPool<WWDatabaseAsset.Match>.Shared
            .Rent(userMatchers.Length + additionalMatchers.Length);

        userMatchers.CopyTo(matchers, 0);
        additionalMatchers.CopyTo(matchers, userMatchers.Length);

        var result 
            = asset?.MatchPointers(matchers) ?? new int[0];

        ArrayPool<WWDatabaseAsset.Match>.Shared.Return(matchers);            
        return result;
    }

    public string[] GetPointedData(int pointer, params string[] columns) {
        if (asset == null) return null;

        return asset.GetData(pointer, columns);
    }

    public string GetLocalizedValue(string lang, string column) {
        return GetData(
            new WWDatabaseAsset.Match[] { new("Lang", lang) }, 
            new string[] { column }
        )[0];
    }

    public override string ToString() {
        var result = $"[{asset}; ";
        foreach (var match in userMatchers)
            result += $"{match.key}:{match.value},";
        return result + "]";
    } 

    // public override bool Equals(object other) {
    //     if (!(other is WWDatabaseSelector selectorOther)) return false;

    //     if (selectorOther.asset != asset) return false;

    //     var otherLen = selectorOther.userMatchers.Length;
    //     var len = userMatchers.Length;
    //     if (len != otherLen) return false;

    //     for (int i = 0; i < len; ++i) {
    //         if (userMatchers[i].key != selectorOther.userMatchers[i].key)
    //             return false;
    //         if (userMatchers[i].value != selectorOther.userMatchers[i].value)
    //             return false;
    //     }

    //     return true;
    // }

    // public override int GetHashCode() => 0;

    // public static bool operator==(WWDatabaseSelector x, WWDatabaseSelector y)
    //     => x.Equals(y);
    // public static bool operator!=(WWDatabaseSelector x, WWDatabaseSelector y)
    //     => !x.Equals(y);
}


#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(WWDatabaseSelector))]
public class WWDatabaseSelectorDrawer : PropertyDrawer {
    public override VisualElement CreatePropertyGUI(SerializedProperty prop) {
        var propAsset = prop.FindPropertyRelative("asset");
        // var propDefaultMatchers = prop.FindPropertyRelative("defaultMatchers");
        var propUserMatchers = prop.FindPropertyRelative("userMatchers");

        var root = new VisualElement();

        // var dataColumn = new VisualElement() 
        //     { style = { flexGrow = 1f } };

        var fieldAsset 
            = new PropertyField(propAsset) { label = preferredLabel };
        // dataColumn.Add(fieldAsset);
        root.Add(fieldAsset);

        for (int i = 0; i < propUserMatchers.arraySize; ++i) {
            var propMatcher = propUserMatchers.GetArrayElementAtIndex(i);
            var propKey = propMatcher.FindPropertyRelative("key");
            var propValue = propMatcher.FindPropertyRelative("value");

            // var matcherLine = new VisualElement() 
            //     { style = { flexDirection = FlexDirection.Row }};

            // matcherLine.Add(new Label() 
            //     { text = propKey.stringValue, style = { width = 80f } });
            root.Add(new PropertyField(propValue) { 
                label = propKey.stringValue, 
                style = { marginLeft = 20f, flexGrow = 1f }
            });

            // dataColumn.Add(matcherLine);
        }

        var labelEntries = new Label("Found 0 entries") { 
            style = { marginLeft = 25f, flexGrow = 1f } 
        };
        // dataColumn.Add(labelEntries);

        // root.Add(new Label() 
        //     { text = preferredLabel, style = { width = 100f } });
        // root.Add(dataColumn);
        root.Add(labelEntries);

        root.schedule.Execute(() => {
            labelEntries.style.backgroundColor = new Color(.2f, .2f, .0f, 1f);
            var target = prop.boxedValue as WWDatabaseSelector;
            if (target == null) return;
            if (target.userMatchers.Length == 0) return;
            if (target.asset == null) return;

            var ptrs = target.asset.MatchPointers(target.userMatchers);

            labelEntries.text = $"Found {ptrs.Length} entries";
            labelEntries.style.backgroundColor = ptrs.Length > 0 
                ? new Color(0f, .2f, 0f, 1f) : new Color(.2f, 0f, 0f, 1f);
        }).Every(1000).StartingIn(0);


        return root;
    }

    public override void 
    OnGUI(Rect pos, SerializedProperty prop, GUIContent label) {
        var propAsset = prop.FindPropertyRelative("asset");
        var propUserMatchers = prop.FindPropertyRelative("userMatchers");

        EditorGUI.BeginProperty(pos, label, prop);

        var line = pos.LineCut(out pos);
        var rectHeader = EditorGUI.PrefixLabel(line, label);
        EditorGUI.indentLevel++;

        var rectAsset = pos.LineCut(out pos);
        EditorGUI.PropertyField(rectAsset, propAsset);

        for (int i = 0; i < propUserMatchers.arraySize; ++i) {
            var propMatcher = propUserMatchers.GetArrayElementAtIndex(i);
            var propKey = propMatcher.FindPropertyRelative("key");
            var propValue = propMatcher.FindPropertyRelative("value");

            var cnt = new GUIContent(propKey.stringValue);
            var rect = pos.LineCut(out pos);
            EditorGUI.PropertyField(rect, propValue, cnt);
        }

        EditorGUI.indentLevel--;
        EditorGUI.EndProperty();
    }

    public override float 
    GetPropertyHeight(SerializedProperty prop, GUIContent label) {
        var propUserMatchers = prop.FindPropertyRelative("userMatchers");

        var pos = new Rect();
        var line = pos.LineCut(out pos);
        var rectAsset = pos.LineCut(out pos);

        for (int i = 0; i < propUserMatchers.arraySize; ++i) {
            var rect = pos.LineCut(out pos);
        }

        return -pos.height;
    }
}

#endif