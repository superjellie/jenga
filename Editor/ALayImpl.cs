#if UNITY_EDITOR
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine.Serialization;

namespace Jenga {

    [CustomPropertyDrawer(typeof(ALay.ILayoutMe), true)]
    public class ALayPropertyDrawer : PropertyDrawer {

        public override VisualElement CreatePropertyGUI(
            SerializedProperty property
        ) {
            var root = new VisualElement();
            Rebuild(root, property);
            return root;
        }

        public void Rebuild(VisualElement root, SerializedProperty property) {
            // Debug.Log(property.propertyPath);
            property.serializedObject.Update();

            root.Clear();

            var main = new FieldContainer() { label = preferredLabel };

            var flags 
                = BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static | BindingFlags.Instance;

            var myType = property.propertyType 
                == SerializedPropertyType.ManagedReference 
                    ? (SerializedPropertyUtility.GetManagedType(property)
                        ?? fieldInfo.FieldType)
                    : SerializedPropertyUtility.GetFieldType(fieldInfo); 

            foreach (var member in myType.GetMembers(flags)) {

                if (member.GetCustomAttributes(typeof(HideInInspector), false)
                        .Length > 0) continue;
                var ve = ALayLayouter.LayoutMember(
                    member, property, () => Rebuild(root, property)
                );

                if (ve != null)
                    main.content.Add(ve);
            }

            ALayLayouter.LayoutSelf(
                main, fieldInfo, property, () => Rebuild(root, property)
            );

            root.Add(main);
            root.Bind(property.serializedObject);
        }
    }

    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class CustomLayouterAttribute : System.Attribute {
        public System.Type type;

        public CustomLayouterAttribute(System.Type type)
            => this.type = type;
    }

    public class ALayContext {
        public string name;
        public string path;
        public MemberInfo memberInfo;
        public object attribute;
        public SerializedObject serializedObject;
        public System.Action refreshCallback;

        public SerializedProperty property;

        public ALayContext(
            MemberInfo memberInfo, string path,
            object attribute, SerializedObject so,
            System.Action refreshCallback
        ) {
            this.memberInfo = memberInfo;
            this.path = path;
            this.attribute = attribute;
            this.serializedObject = so;
            this.name = memberInfo.Name;
            this.refreshCallback = refreshCallback;

            if (memberInfo is FieldInfo fieldInfo) 
                property = so.FindProperty(path);
        }

        public T GetAttribute<T>() where T : System.Attribute
            => attribute as T; 
        public FieldInfo fieldInfo => memberInfo as FieldInfo;

        // public void Ref 
    }

    public class ALayLayouter {
        public virtual void Layout(VisualElement element, ALayContext ctx) { }

        static Dictionary<System.Type, System.Type> layouters = new();
        static ALayLayouter() {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    foreach (var attr in 
                        type.GetCustomAttributes
                            (typeof(CustomLayouterAttribute), false)) 
                        layouters[((CustomLayouterAttribute)attr).type] = type;
        }

        public static void LayoutSelf(
            VisualElement ve, FieldInfo fieldInfo, SerializedProperty property,
            System.Action refreshCallback
        ) {


            var myType = property.propertyType 
                == SerializedPropertyType.ManagedReference 
                    ? (SerializedPropertyUtility.GetManagedType(property)
                        ?? fieldInfo.FieldType)
                    : SerializedPropertyUtility.GetFieldType(fieldInfo); 

            foreach (var attr in myType.GetCustomAttributes(true)) {
                var layouter = ALayLayouter.Get(attr.GetType());
                if (layouter == null) continue; 

                var ctx = new ALayContext(
                    fieldInfo, property.propertyPath, attr,
                    property.serializedObject, refreshCallback
                );

                layouter.Layout(ve, ctx);
            }

        }

        public static VisualElement LayoutMember(
            MemberInfo member, SerializedProperty property, 
            System.Action refreshCallback
        ) {
            var wasLayouted = false;

            var memberProp = property.FindPropertyRelative(member.Name);

            var memberPath = memberProp != null
                ? memberProp.propertyPath
                : $"{property.propertyPath}+{member.Name}";

            var attrs = member.GetCustomAttributes(false);

            var ve = memberProp != null
                ? new PropertyField(memberProp) 
                    { name = $"ALay:{memberPath}" }
                : new VisualElement()
                    { name = $"ALay:{memberPath}" }; 

            // Debug.Log(attrs.Length);

            foreach (var attr in attrs) {
                var layouter = ALayLayouter.Get(attr.GetType());
                if (layouter == null) continue;

                var ctx = new ALayContext(
                    member, memberPath, attr,
                    property.serializedObject, refreshCallback
                );

                layouter.Layout(ve, ctx);
                wasLayouted = true;
            }

            return wasLayouted || memberProp != null ? ve : null;
        }

        public static ALayLayouter Get(System.Type type)
            => layouters.ContainsKey(type)
                ? System.Activator.CreateInstance(layouters[type])
                    as ALayLayouter
                : null;
    }
 
    [CustomLayouter(typeof(ALay.InlineAttribute))]
    public class ALayInlineLayouter : ALayLayouter {

        FieldContainer container;

        public override void Layout(VisualElement ve, ALayContext ctx) {
            if (ve is FieldContainer container) {
                container.inline = true;
                container.hideToggle = true;
                container.content.EnableInClassList(
                    SerializedPropertyUtility.ussNoLabelsInChildrenClassName,
                    true
                );
            }
        }
    }

    [CustomLayouter(typeof(ALay.StyleAttribute))]
    public class ALayStyleLayouter : ALayLayouter {

        public override void Layout(VisualElement ve, ALayContext ctx) {
            var attr = ctx.GetAttribute<ALay.StyleAttribute>();
            var v 
                = attr.applyToContent && ve is FieldContainer fc 
                    ? fc.content
                    : ve;
                    
            if (attr.hideCheckmark && ve is FieldContainer fc1)
                fc1.hideToggle = true;

            if (!float.IsNaN(attr.flexGrow))   v.style.flexGrow   = attr.flexGrow;
            if (!float.IsNaN(attr.flexShrink)) v.style.flexShrink = attr.flexShrink;
            if (!float.IsNaN(attr.minWidth))   v.style.minWidth   = attr.minWidth;
            if (!float.IsNaN(attr.minHeight))  v.style.minHeight  = attr.minHeight;
            if (!float.IsNaN(attr.maxWidth))   v.style.maxWidth   = attr.maxWidth;
            if (!float.IsNaN(attr.maxHeight))  v.style.maxHeight  = attr.maxHeight;
            if (!float.IsNaN(attr.width))      v.style.width      = attr.width;
            if (!float.IsNaN(attr.height))     v.style.height     = attr.height;

            if (!float.IsNaN(attr.marginLeft))      v.style.marginLeft     = attr.marginLeft;
            if (!float.IsNaN(attr.marginRight))     v.style.marginRight    = attr.marginRight;
            if (!float.IsNaN(attr.marginTop))       v.style.marginTop      = attr.marginTop;
            if (!float.IsNaN(attr.marginBottom))    v.style.marginBottom   = attr.marginBottom;
            // Debug.Log($"{ctx.path}, {v.style.marginLeft}");
        }
    }

    [CustomLayouter(typeof(ALay.HideHeaderAttribute))]
    public class ALayHideHeaderLayouter : ALayLayouter { 
        public override void Layout(VisualElement ve, ALayContext ctx) { 
            ve.schedule.Execute(() => {
                var container = ve.Q<FieldContainer>();
                if (container == null) return;

                container.hideHeader = true;
                container.removeContentMargins = true;
            }).StartingIn(100);
        }
    }


    [CustomLayouter(typeof(ALay.TypeSelectorAttribute))]
    public class ALayTypeSelectorLayouter : ALayLayouter {

        public const string ussInWindowClass = "jenga-in-window";

        public override void Layout(VisualElement ve, ALayContext ctx) {
            var attr = ctx.GetAttribute<ALay.TypeSelectorAttribute>();
            var prop = attr.path != null 
                ? ctx.property.FindPropertyRelative(attr.path)
                : ctx.property;
            if (ve is FieldContainer container && prop != null) {
                // container.schedule.Execute(() => {
                //     if (container.header.Query<TypeSelectorField>()
                //         .Where(x => x.parent == container.header).First()
                //         != null) return;
                    
                //     container.header.Add(new TypeSelectorField() {
                //         typeFamily = attr.typeFamily,
                //         currentType = SerializedPropertyUtility
                //             .GetManagedType(prop),
                //         property = prop,
                //         onSelect = (t) => {
                //             ctx.refreshCallback();
                //         }
                //     });
                // }).StartingIn(100);
                container.header.Add(new TypeSelectorField() {
                    typeFamily = attr.typeFamily,
                    currentType = SerializedPropertyUtility
                        .GetManagedType(prop),
                    property = prop,
                    onSelect = (t) => {
                        ctx.refreshCallback();
                    }
                });


                container.schedule.Execute(() => {
                    if (PropertyWindow.HasWindowFor(prop)) {
                        container.foldout.value = false;
                        container.EnableInClassList(ussInWindowClass, true);
                    } else 
                        container.EnableInClassList(ussInWindowClass, false);
                }).Every(300).StartingIn(100);
            }
        }
    }

    [CustomLayouter(typeof(ALay.ListViewAttribute))]
    public class ALayListViewLayouter : ALayLayouter {

        public override void Layout(VisualElement ve, ALayContext ctx) {
            var attr = ctx.GetAttribute<ALay.ListViewAttribute>();
            ve.schedule.Execute(() => {
                var view = ve.Q<ListView>();
                if (view == null) return;
                view.reorderable = attr.reorderable;
                view.showFoldoutHeader = attr.showFoldoutHeader;
                view.showAddRemoveFooter = attr.showAddRemoveFooter;
                view.showBoundCollectionSize = attr.showBoundCollectionSize;
                view.selectionType = SelectionType.Multiple;
                view.itemIndexChanged += (i, j) => view.Rebuild();
                view.itemsRemoved += (i) => view.Rebuild();

                view.itemsAdded += (indices) => {

                    foreach (var index in indices) {
                        var propItem = ctx.property
                            .GetArrayElementAtIndex(index);

                        var end = propItem.GetEndProperty();
                        for (var it = propItem.Copy(); 
                            !SerializedProperty.EqualContents(it, end); 
                            it.Next(true)
                        ) 
                            if (it.propertyType 
                                == SerializedPropertyType.ManagedReference) 
                                it.managedReferenceId 
                                    = ManagedReferenceUtility.RefIdNull;
                        
                        // if (attr.addItemCallback != null) {
                        //     var callback = field.FieldType.GetMethod(
                        //         attr.addItemCallback,
                        //         BindingFlags.Public | BindingFlags.NonPublic
                        //         | BindingFlags.Static
                        //     );
                            
                        //     if (callback != null) 
                        //         callback.Invoke(null, new object[] { propItem });
                        // }
                    } 

                    ctx.serializedObject.ApplyModifiedProperties();
                    
                    view.Rebuild();
                    view.Unbind();
                    view.Bind(ctx.serializedObject);

                };
            }).StartingIn(100);
        }
    }

    [CustomLayouter(typeof(ALay.OptionsAttribute))]
    public class ALayOptionsLayouter : ALayLayouter {

        public override void Layout(VisualElement ve, ALayContext ctx) {
            var attr = ctx.GetAttribute<ALay.OptionsAttribute>();

            // var type = ctx.property != null
            //     ? ctx.property.propertyType 
            //         == SerializedPropertyType.ManagedReference
            //         ? SerializedPropertyUtility.GetManagedType(ctx.property) 
            //         : SerializedPropertyUtility.GetFieldType(ctx.fieldInfo)
            //     : ctx.fieldInfo.FieldType;

            // var parentType = ctx.fieldInfo.DeclaringType;
            // var flags = BindingFlags.Public | BindingFlags.NonPublic
            //     | BindingFlags.Static;
            // var method = parentType?.GetMethod(attr.provider, flags);

            // if (method == null) return;


            var field = new PopupField<(object, string)>() { 
                style = { flexGrow = 1f }
            };

            // ve.Clear();
            ve.Add(field);
        }

        static void UpdateFiekd(MethodInfo method, PopupField<object> field) {
            // var map = new ALay.OptionsAttribute.Map(); 
            // method.Invoke(null, { ctx.property, map });
            
            // field.choices = map.options;
        }
    }

    [CustomLayouter(typeof(ALay.DelayAttributeAttribute))]
    public class ALayDelayAttributeLayouter : ALayLayouter {

        public override void Layout(VisualElement ve, ALayContext ctx) {
            var attr = ctx.GetAttribute<ALay.DelayAttributeAttribute>();
            var clss = attr.inClass  
                ? ctx.property != null
                    ? ctx.property.propertyType 
                        == SerializedPropertyType.ManagedReference
                        ? SerializedPropertyUtility.GetManagedType(ctx.property) 
                        : SerializedPropertyUtility.GetFieldType(ctx.fieldInfo)
                    : ctx.fieldInfo.FieldType
                : ctx.fieldInfo.DeclaringType;

            var flags = BindingFlags.Public | BindingFlags.NonPublic
                | BindingFlags.Static;
            var mthd = clss?.GetMethod(attr.name, flags);

            if (mthd != null) {
                var delayedAttr = mthd.Invoke(null, null);
                if (delayedAttr == null) return;

                var layouter = ALayLayouter.Get(delayedAttr.GetType());
                if (layouter == null) return;

                var delayedCtx = new ALayContext(
                    ctx.memberInfo, ctx.path, delayedAttr,
                    ctx.serializedObject, ctx.refreshCallback
                );

                layouter.Layout(ve, delayedCtx);
            }
        }
    }

    [CustomLayouter(typeof(ALay.MatchReferencesAttribute))]
    public class ALayMatchReferencesAttributeLayouter : ALayLayouter {

        public static string ussFixTextMarginClass = "jenga-fix-text-margin";



        // public static Dictionary<(Object, long), HashSet<FieldContainer>>
        //     visibleContainers = new();

        ALay.MatchReferencesAttribute attr;
        public override void Layout(VisualElement ve, ALayContext ctx) {
            attr = ctx.GetAttribute<ALay.MatchReferencesAttribute>();

            if (ctx.property == null) return;
            var propValue = ctx.property.FindPropertyRelative
                (attr.pathToSerializedReference);
            if (propValue == null) return;
            if (propValue.propertyType 
                    != SerializedPropertyType.ManagedReference) return;
            var propName = propValue.FindPropertyRelative(attr.pathToRefName);
            if (propName == null) return;
            var target = ctx.property.serializedObject.targetObject;

            var fieldType = SerializedPropertyUtility
                .GetFieldType(ctx.fieldInfo)
                ?.GetField(attr.pathToSerializedReference)
                ?.FieldType;

            if (fieldType == null) return;

            if (ve is FieldContainer container) {
                // container.schedule.Execute(() => {
                var myRoot = new VisualElement() { 
                    style = { flexDirection = FlexDirection.Row } 
                };

                var nameTextField = new TextField() {
                    maxLength = 31,
                    style = { width = 50f },
                    bindingPath = propName.propertyPath,
                    value = $"{propValue.managedReferenceId.ToString("X")}"
                };

                nameTextField.EnableInClassList(ussFixTextMarginClass, true);
                // nameTextField.RegisterCallback<ChangeEvent<string>>(evt => {
                //     SetColor(myRoot, ColorFromString(evt.newValue));
                // });

                var nameMenuButton = new Button() {
                    style = { width = 20f }
                };

                nameMenuButton.clicked += () => {
                    var menu = new GenericMenu();

                    foreach (var id in GetNamedRefs(target)) {
                        var value = ManagedReferenceUtility
                            .GetManagedReference(target, id);
                        var name = $"{GetRefName(value)}#{id.ToString("X")}";

                        if (!fieldType.IsAssignableFrom(value.GetType()))
                            continue;

                        menu.AddItem(
                            new GUIContent(name), 
                            id == propValue.managedReferenceId, 
                            () => {
                                propValue.managedReferenceId = id;
                                propValue.serializedObject
                                    .ApplyModifiedProperties();
                
                                SetColor(
                                    myRoot, 
                                    SerializedPropertyUtility
                                        .ColorFromId(id)
                                );

                                ctx.refreshCallback(); 
                            }
                        );
                    }

                    menu.AddSeparator("");
                    menu.AddItem(
                        new GUIContent("Remove"), false, 
                        () => {
                            propValue.managedReferenceValue = null;
                            propValue.serializedObject
                                .ApplyModifiedProperties();
                            ctx.refreshCallback();
                        }
                    );

                    menu.AddItem(
                        new GUIContent("Remove"), false, 
                        () => {
                            propValue.managedReferenceValue = null;
                            propValue.serializedObject
                                .ApplyModifiedProperties();
                            ctx.refreshCallback();
                        }
                    );

                    menu.ShowAsContext();
                };

                myRoot.Add(nameTextField);
                myRoot.Add(nameMenuButton);
                
                SetColor(
                    myRoot, 
                    SerializedPropertyUtility
                        .ColorFromId(propValue.managedReferenceId)
                );


                container.header.Add(myRoot);


                // container.foldout.RegisterCallback<ChangeEvent<bool>>((evt) => {
                //     var key = (target, propValue.managedReferenceId);
                //     if (visibleContainers.ContainsKey(key))
                //         container.foldout.value = true;
                // });

                // if (propValue.managedReferenceValue != null) {
                //     var key = (target, propValue.managedReferenceId);
                //     if (!visibleContainers.ContainsKey(key))
                //         visibleContainers.Add(key, new());
                //     visibleContainers[key].Add(container);
                // }
            }
        }

        // public Color ColorFromString(string name) 
        //     => string.IsNullOrEmpty(name) 
        //         ? Color.clear
        //         : colors[Mathx.Mod(name.GetHashCode(), colors.Length)];

        public void SetColor(VisualElement myRoot, Color color) {
            myRoot.style.borderBottomColor  = color;
            myRoot.style.borderLeftColor    = color;
            myRoot.style.borderRightColor   = color;
            myRoot.style.borderTopColor     = color;

            myRoot.style.borderBottomWidth  = 2f;
            myRoot.style.borderLeftWidth    = 2f;
            myRoot.style.borderRightWidth   = 2f;
            myRoot.style.borderTopWidth     = 2f;

            myRoot.style.borderBottomLeftRadius     = 2f;
            myRoot.style.borderBottomRightRadius    = 2f;
            myRoot.style.borderTopRightRadius       = 2f;
            myRoot.style.borderTopLeftRadius        = 2f;
        }

        public List<long> GetNamedRefs(Object target) {
            var list  = new List<long>();
            var ids = ManagedReferenceUtility.GetManagedReferenceIds(target);

            foreach (var id in ids) {
                var value = ManagedReferenceUtility
                    .GetManagedReference(target, id);
                var name = GetRefName(value);

                if (!string.IsNullOrEmpty(name))
                    list.Add(id);
            }

            return list;
        }

        public string GetRefName(object value) {
            if (value == null) return null;

            var type = value.GetType();
            var field = type.GetField(attr.pathToRefName);
            // Debug.Log($"value: {value}, field: {type}");
            var refName = field.GetValue(value);

            return (string)refName;
        }
    }

    [CustomLayouter(typeof(ALay.ScenePreviewAttribute))]
    public class ALayScenePreviewLayouter : ALayLayouter {

        public static string ussEyeToggleClass = "jenga-eye-toggle";
        public static string ussScenePreviewClass = "jenga-scene-preview";

        ALay.ScenePreviewAttribute attr;

        public struct PropertyDescriptor {
            public Object target;
            public string path;

            public PropertyDescriptor(SerializedProperty prop) {
                target = prop.serializedObject.targetObject;
                path = prop.propertyPath;
            }
        }

        static HashSet<PropertyDescriptor> previews = new();

        public static bool HasPreview(SerializedProperty self) 
            => previews.Contains(new(self));

        public override void Layout(VisualElement ve, ALayContext ctx) {
            attr = ctx.GetAttribute<ALay.ScenePreviewAttribute>();

            if (ve is FieldContainer fc) {
                var previewToggle = new Toggle() { 
                    name = $"PreviewToggle:{ctx.path}", 
                    value = false 
                };

                previewToggle.EnableInClassList(ussEyeToggleClass, true);
                previewToggle.EnableInClassList(ussScenePreviewClass, true);

                previewToggle.RegisterCallback<ChangeEvent<bool>>((evt) => {
                    if (evt.newValue)
                        previews.Add(new(ctx.property));
                    else
                        previews.Remove(new(ctx.property));

                    var toggles = fc.content
                        .Query<Toggle>(className: ussScenePreviewClass)
                        .ToList();
                    // Debug.Log(toggles.Count);
                    foreach (var toggle in toggles)
                        toggle.value = evt.newValue;
                });

                fc.header.Add(previewToggle);
            }
        }

    }

}
#endif