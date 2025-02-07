#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

using UnityEngine.UIElements;
using UnityEditor.UIElements;

public class TypeSelectorField : VisualElement {
    
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

		RegisterCallback<AttachToPanelEvent>(
			OnAttachToPanel, TrickleDown.TrickleDown
		);
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
}
#endif