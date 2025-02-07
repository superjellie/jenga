#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Serialization;

namespace Jenga {

    [CustomPropertyDrawer(typeof(ALay.ILayoutMe), true)]
    [CustomPropertyDrawer(typeof(ALay.LayoutFieldAttribute))]
    public class ALayPropertyDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI(
            SerializedProperty prop
        ) {
            var type 
                = prop.propertyType == SerializedPropertyType.ManagedReference
                    ? SerializedPropertyUtility.GetManagedType(prop)
                    : SerializedPropertyUtility.GetFieldType(fieldInfo);

            var label = preferredLabel;
            var container = new FieldContainer();
            container.content.Add(ALayImpl.Layout(type, prop));

            ALayImpl.SetStyle(
                container, type.GetCustomAttributes(false), ref label
            );

            container.labelText = label;

            return container;
        }
    }


    public static class ALayImpl {

        // public static string ussLabel

        public static bool HasAttribute<T>(MemberInfo info) {
            var arr = info.GetCustomAttributes(typeof(T), false);
            return arr.Length > 0;
        }

        public static bool HasAttribute<T>(MemberInfo info, out T attribute) {
            var arr = info.GetCustomAttributes(typeof(T), false);
            attribute = arr.Length > 0 ? (T)arr[0] : default(T);
            return arr.Length > 0;
        }

        public static VisualElement Layout(
            System.Type type, 
            SerializedProperty property
        ) {
            var root = new VisualElement();
            VisualElement veLine = null;

            var flags = BindingFlags.Public 
                | BindingFlags.NonPublic
                | BindingFlags.Instance
                | BindingFlags.Static; 

            foreach (var member in type.GetMembers(flags)) {
                if (member is FieldInfo field) { 

                    var myProp = property.FindPropertyRelative(field.Name);

                    if (!field.IsPublic 
                        && !HasAttribute<SerializeField>(field)
                        // || HasAttribute<ALay.SkipAttribute>(field)
                        || HasAttribute<System.NonSerializedAttribute>(field)
                        || HasAttribute<HideInInspector>(field)
                        || field.IsStatic
                        || myProp == null
                    ) continue;
 
                    var ve = LayoutField(myProp, field);

                    if (HasAttribute<ALay.StartLineAttribute>(field)) {
                        if (veLine != null) root.Add(veLine);

                        veLine = new VisualElement() { 
                            style = { flexDirection = FlexDirection.Row }
                        };
                    }

                    if (veLine != null) veLine.Add(ve);
                    else root.Add(ve);
                    
                    if (HasAttribute<ALay.EndLineAttribute>(field)) {
                        if (veLine != null) root.Add(veLine);
                        veLine = null;
                    }

                } else if (member is MethodInfo method) {
                    if (!method.IsPublic || !method.IsStatic) continue;

                    if (HasAttribute<ALay.ButtonAttribute>(
                        method, out var attr
                    )) {
                        var btn = new Button() {
                            text = attr.label ?? method.Name
                        };
                        btn.clicked += () => method.Invoke(null, null);
                        root.Add(btn);
                    }
                }
            }

            if (veLine != null)
                root.Add(veLine);

            return root;
        }

        public static VisualElement FindHeader(VisualElement ve) {
            var fieldContainer = ve.Q<FieldContainer>();
            Debug.Log(fieldContainer);
            if (fieldContainer != null) return fieldContainer.header;

            var foldout = ve.Q<Foldout>();
            if (foldout != null) return foldout.hierarchy[0];

            // Debug.Log(ve)
            // var field = ve.Q<VisualElement>(className: "");
            return ve;
        }

        public static VisualElement LayoutField(
            SerializedProperty prop, FieldInfo field
        ) {
            var ve = HasAttribute<ALay.ListViewAttribute>(
                    field, out var listView
                ) 
                ? MakeListView(field, listView, prop)
                : new PropertyField() { bindingPath = prop.propertyPath }
                    as VisualElement;
           
            string label = null;
            SetStyle(ve, field.GetCustomAttributes(false), ref label);

            if (label != null && ve is PropertyField pf) {
                pf.label = label;
                if (label == "")
                    pf.EnableInClassList(
                        SerializedPropertyUtility
                            .ussNoLabelPropertyClassName,
                        true
                    );
            }

            if (label != null && ve is ListView lv)
                lv.headerTitle = label; 

            if (HasAttribute<ALay.UsageToggleAttribute>(
                field, out var usageAttr
            )) {
                var row = new VisualElement() { 
                    style = { flexDirection = FlexDirection.Row }
                };

                var index = prop.propertyPath.LastIndexOf(".");

                var parentPath = index > 0 
                    ? prop.propertyPath.Remove(index)
                    : "";

                var usageBoolPath = index > 0
                    ? $"{parentPath}.{usageAttr.path}"
                    : usageAttr.path;

                var toggle = new Toggle() {
                    bindingPath = usageBoolPath,
                    label = null,
                    style = { minWidth = 20f }
                };

                toggle.RegisterValueChangedCallback((evt) => {
                    ve.SetEnabled(evt.newValue);
                });

                row.Add(toggle);
                ve.style.flexGrow = 1f;

                var propBool = prop.serializedObject
                    .FindProperty(usageBoolPath);
                ve.SetEnabled(propBool?.boolValue ?? false);

                row.Add(ve);
                return row;
            }

            return ve;
        }

        public static void SetStyle(
            VisualElement ve, object[] attrs, ref string label
        ) {
            foreach (var attr in attrs)
                if (attr is ALay.LabelAttribute labelAttribute)
                    label = labelAttribute.value;
                else if (attr is ALay.HideLabelAttribute)
                    label = "";
                else if (attr is ALay.FlexGrowAttribute flexGrow)
                    ve.style.flexGrow = flexGrow.value;
                else if (attr is ALay.MinWidthAttribute minWidth)
                    ve.style.minWidth = minWidth.value;
                else if (attr is ALay.MaxWidthAttribute maxWidth)
                    ve.style.maxWidth = maxWidth.value;
        }

        public static ListView MakeListView(
            FieldInfo field, ALay.ListViewAttribute attr, 
            SerializedProperty prop
        ) {
            var lv = new ListView() {
                reorderable = attr.reorderable,
                showFoldoutHeader = attr.showFoldoutHeader,
                showAddRemoveFooter = attr.showAddRemoveFooter,
                showBoundCollectionSize 
                    = attr.showBoundCollectionSize,
                bindingPath = prop.propertyPath,
                makeItem = () => new PropertyField() { label = "" },
                bindItem = (ve, i) => ve.Q<PropertyField>()
                    .BindProperty(prop.GetArrayElementAtIndex(i)),

                unbindItem = (ve, i) => ve.Q<PropertyField>()
                    .Unbind(),
                virtualizationMethod 
                    = CollectionVirtualizationMethod.DynamicHeight,
                selectionType = SelectionType.Single
            };

            lv.itemIndexChanged += (i, j) => lv.Rebuild();
            lv.itemsRemoved += (i) => lv.Rebuild();

            lv.itemsAdded += (indices) => {
                foreach (var index in indices) {
                    var propItem = prop.GetArrayElementAtIndex(index);
                    var end = propItem.GetEndProperty();
                    for (var it = propItem.Copy(); 
                        !SerializedProperty.EqualContents(it, end); 
                        it.Next(true)
                    ) 
                        if (it.propertyType 
                            == SerializedPropertyType.ManagedReference) 
                            it.managedReferenceId 
                                = ManagedReferenceUtility.RefIdNull;
                    
                    if (attr.addItemCallback != null) {
                        var callback = field.FieldType.GetMethod(
                            attr.addItemCallback,
                            BindingFlags.Public | BindingFlags.NonPublic
                            | BindingFlags.Static
                        );
                        
                        if (callback != null) 
                            callback.Invoke(null, new object[] { propItem });
                    }
                } 

                prop.serializedObject.ApplyModifiedProperties();
                
                lv.Rebuild();
                lv.Unbind();
                lv.Bind(prop.serializedObject);
            };
            return lv;
        }
    } 

}
#endif