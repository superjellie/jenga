#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;

public class GenericPopupContent : PopupWindowContent {

    public Vector2 size = new Vector2(200, 100);
    public VisualElement root = new Label("Hello, Popups!");

    public override Vector2 GetWindowSize() => size;

    public override void OnGUI(Rect rect) { }

    public override void OnOpen() {
        editorWindow.rootVisualElement.Add(root);
    }
}

public class CategorizedPopupSelector : VisualElement {

    public delegate void OnSelectDelegate(int index);
    public delegate string ItemInfoDelegate(int index);

    // public string label = "";
    public float  categoryPaneDefaultWidth = 100f;
    public Vector2 popupSize = new Vector2(250f, 200f);

    public OnSelectDelegate onSelect = (index) => { };
    public ItemInfoDelegate getItemCategory = (index) => "Default";
    public ItemInfoDelegate getItemName     = (index) => "Name";
    public ItemInfoDelegate formatSelectedName;

    public int itemsCount = 0;
    public int selectedIndex = 0;

    List<string> categories;
    // List<int> visibleNames;
    Button button;

    string DefaultFormatSelected(int index) 
        => index >= 0 && index < itemsCount 
            ? getItemName(index) 
            : "[None]";

    public CategorizedPopupSelector() {
        button = new Button() { style = { minWidth = 100f } };
        formatSelectedName = DefaultFormatSelected;

        button.clicked += MakePopup;
        RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);

        Add(button);
    }

    void OnAttachToPanel(AttachToPanelEvent attachEvent) {
        button.text = formatSelectedName(selectedIndex);
    }

    void RecalculateCategories() {
        var catSet = new HashSet<string>();
        for (var i = 0; i < itemsCount; ++i)
            catSet.Add(getItemCategory(i));

        categories = new List<string>(catSet);
    }

    void MakePopup() {
        var twoPaneView = new TwoPaneSplitView() {
            fixedPaneIndex = 0,
            fixedPaneInitialDimension = categoryPaneDefaultWidth,
            orientation = TwoPaneSplitViewOrientation.Horizontal
        };

        var scrollLeft = new ScrollView();
        var scrollRight = new ScrollView();

        RecalculateCategories();

        var categoryList = new ListView() { 
            itemsSource = categories,
            makeItem = () => {
                var box = new Box();
                box.Add(new Label() { 
                    style = { unityTextAlign = TextAnchor.MiddleCenter }
                });
                return box; 
            },
            bindItem = (ve, i) => ve.Q<Label>().text = categories[i]
        };
        
        var nameSource = new List<int>();
        
        var nameList = new ListView() {
            itemsSource = nameSource,
            makeItem = () => {
                var box = new Box();
                box.Add(new Label());
                return box; 
            },
            bindItem = 
                (ve, i) => ve.Q<Label>().text = getItemName(nameSource[i]),
        };

        System.Action rebuldNamesList = () => {
            if (categories.Count == 0)
                return;
                
            var cat = categories[categoryList.selectedIndex];
            nameSource.Clear();

            for (int i = 0; i < itemsCount; ++i)
                if (getItemCategory(i) == cat) 
                    nameSource.Add(i);

            nameList.Rebuild();
        };

        categoryList.selectedIndicesChanged 
            += (indicesBad) => rebuldNamesList();

        nameList.selectedIndicesChanged += (indicesBad) => {
            var indices = indicesBad.ToArray();
            if (indices.Length == 0) return;

            var index = nameSource[indices[0]];

            selectedIndex = index;
            button.text = formatSelectedName(selectedIndex);
            onSelect(index);
        };

        categoryList.selectedIndex 
            = selectedIndex >= 0 && selectedIndex < itemsCount
                ? categories.IndexOf(getItemCategory(selectedIndex))
                : 0;
        
        rebuldNamesList();

        scrollLeft.Add(categoryList);
        scrollRight.Add(nameList);

        twoPaneView.Add(scrollLeft);
        twoPaneView.Add(scrollRight);

        UnityEditor.PopupWindow.Show(
            button.worldBound, 
            new GenericPopupContent() { 
                size = popupSize,
                root = twoPaneView
            }
        );
    }
}

#endif