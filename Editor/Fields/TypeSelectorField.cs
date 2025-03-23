#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class TypeSelectorField : VisualElement {
    
    //
	public const string ussClassName = "jenga-type-selector";

	public delegate void OnSelectDelegate(System.Type type);

	//
	public System.Type typeFamily;
	public System.Type currentType;
	public OnSelectDelegate onSelect = (t) => { };

	//
	CategorizedPopupSelector selector;

	public TypeSelectorField() {
		selector = new CategorizedPopupSelector();
	
		Add(selector);
		EnableInClassList(ussClassName, true);

		RegisterCallback<AttachToPanelEvent>(
			OnAttachToPanel, TrickleDown.TrickleDown
		);

		this.AddManipulator(new ContextualMenuManipulator((evt) => {
            evt.menu.AppendAction(
            	"[TODO] Edit Script", 
            	(x) => OpenScriptAsset(), 
            	DropdownMenuAction.AlwaysEnabled
            );
        }));
	}

	void OnAttachToPanel(AttachToPanelEvent evt) {
		var registry = AddTypeMenuAttribute.registries[typeFamily];

        var myIndex = registry.FindIndex((reg) => reg.type == currentType);

        selector.selectedIndex = myIndex;

		selector.onSelect = (i) => {
        	if (i < 0 || i >= registry.Count) return;
			onSelect(registry[i].type);
			currentType = registry[i].type;
		};

        selector.getItemCategory = (i) => {
        	if (i < 0 || i >= registry.Count) return "[None]";
            var split = registry[i].path.Split('/'); 
            if (split.Length == 0) return "Default";
            return split[0];
        };

        selector.getItemName = (i) => {
        	if (i < 0 || i >= registry.Count) return "[None]";
            var split = registry[i].path.Split('/'); 
            if (split.Length == 0) return registry[i].path;
            return split[split.Length - 1];
        };

        selector.itemsCount = registry.Count;
	}

	void OpenScriptAsset() {
		// var filter = $"t:MonoScript {currentType?.Name ?? typeFamily.Name}";
		// var paths = AssetDatabase.FindAssets(filter);
		// if (paths.Length == 0) return;
		// AssetDatabase.LoadAssetAtPath(paths[0], );
	}
}
#endif