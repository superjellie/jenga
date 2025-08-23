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


    public WWDatabaseSelector(string key) {
        this.userMatchers = new WWDatabaseAsset.Match[] { new(key, "") };
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
}

#endif