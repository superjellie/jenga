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
            SerializedProperty prop
        ) {
            var root = new VisualElement()
                { name = $"Jenga.ALay:{prop.propertyPath}" };

            var type 
                = prop.propertyType == SerializedPropertyType.ManagedReference
                    ? SerializedPropertyUtility.GetManagedType(prop)
                    : SerializedPropertyUtility.GetFieldType(fieldInfo);

            if (type == null)
                type = SerializedPropertyUtility.GetFieldType(fieldInfo);

            var container = new FieldContainer() { labelText = preferredLabel };

            var groupStack = new Stack<VisualElement>();
            groupStack.Push(container.content);

            var bindings 
                = BindingFlags.Static | BindingFlags.Instance 
                | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (var memberInfo in type.GetMembers(bindings)) {
                var name = memberInfo.Name;
                var propItem = prop.FindPropertyRelative(name);

                if (memberInfo
                        .GetCustomAttributes(typeof(HideInInspector), false)
                            .Length > 0) continue;

                var element = propItem != null 
                    ? new PropertyField(propItem)
                    : new VisualElement();

                if (memberInfo.DeclaringType    
                        .IsSubclassOf(typeof(ALay.ILayoutMe)))
                    continue;

                element.name 
                    = propItem != null 
                        ? $"Jenga.ALay:{propItem.propertyPath}"
                        : $"Jenga.ALay:{prop.propertyPath}+{name}";

                ALayImpl.LayoutElement(
                    element, memberInfo, memberInfo.GetCustomAttributes(false),
                    propItem, groupStack,
                    preferredLabel
                );
            }

            var attrs = new List<object>(type.GetCustomAttributes(true));
            attrs.AddRange(fieldInfo.GetCustomAttributes(false));

            // Debug.Log($"{prop.propertyPath}: {string.Concat(attrs.Select(x => x.GetType().Name + " "))}");
            var rootStack = new Stack<VisualElement>();
            rootStack.Push(root);

            ALayImpl.LayoutElement(
                container, fieldInfo,
                attrs.ToArray(), 
                prop, rootStack,
                preferredLabel
            );

            return root;
        }
    }

    // [CustomPropertyDrawer(typeof(ALay.LayoutMeAttribute))]
    // public class ALaySinglePropertyDrawer : PropertyDrawer {

    //     public override VisualElement CreatePropertyGUI(
    //         SerializedProperty prop
    //     ) {
    //         // var root = new VisualElement() 
    //         //     { name = "Jenga.ALay:layout-me-root"};
    //         // var groupStack = new Stack<VisualElement>();
    //         // groupStack.Push(root);

    //         // var field = new PropertyField(prop);

    //         // ALayImpl.LayoutElement(
    //         //     field, fieldInfo,
    //         //     fieldInfo.GetCustomAttributes(false),  
    //         //     prop, groupStack,
    //         //     preferredLabel
    //         // );

    //         return new PropertyField(prop);
    //     }

    // }


    public static class ALayImpl {

        public class Layouter {
            public object attribute;
            public MemberInfo memberInfo;
            public SerializedProperty property;
            public VisualElement element;
            public Stack<VisualElement> groupStack;
            public string preferredParentLabel;

            public virtual void OnLayout() { }
            public virtual void OnAfterLayout() { }
        }

        [System.AttributeUsage(
            System.AttributeTargets.Class, AllowMultiple = true
        )]
        public class CustomLayouterAttribute : System.Attribute {
            public System.Type attributeType;
            public CustomLayouterAttribute(System.Type type)
                => attributeType = type;
        }

        public static Dictionary<System.Type, System.Type> 
            attrLayouters = new();

        static ALayImpl() {
            var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
                foreach (var type in assembly.GetTypes())
                    foreach (var attr in type.GetCustomAttributes(
                        typeof(CustomLayouterAttribute), false
                    )) attrLayouters
                        [((CustomLayouterAttribute)attr).attributeType] = type;
        }

        public static void AddLayouter<T, Q>()
            where T : System.Attribute 
            where Q : Layouter {
            attrLayouters[typeof(T)] = typeof(Q);
            // Debug.Log($"{typeof(T).Name}: {typeof(T).Name}");
        }

        public static Layouter MakeLayouter(System.Type type)
            => attrLayouters.ContainsKey(type) 
                ? (Layouter)System.Activator.CreateInstance(attrLayouters[type])
                : null;

        public static void LayoutElement(
            VisualElement element,
            MemberInfo memberInfo, 
            IEnumerable<object> attributes, 
            SerializedProperty propItem,
            Stack<VisualElement> groupStack,
            string preferredParentLabel
        ) {
            var layouters = new List<ALayImpl.Layouter>();

            foreach (var attr in attributes) {
                var layouter = MakeLayouter(attr.GetType());
                if (layouter == null) continue;

                layouter.property = propItem;
                layouter.element = element;
                layouter.memberInfo = memberInfo;
                layouter.groupStack = groupStack;
                layouter.attribute = attr;
                layouter.preferredParentLabel = preferredParentLabel;
                layouters.Add(layouter);

                layouter.OnLayout();
            }

            if (propItem != null || layouters.Count > 0)
                if (groupStack.TryPeek(out var parent))
                    parent.Add(element);

            foreach (var layouter in layouters)
                layouter.OnAfterLayout();
        }

        // Queries
        public static VisualElement QueryHeader(VisualElement root) 
            => root?.Query<VisualElement>()
                .Where(ve => 
                    ve.ClassListContains("custom-field-container__header")
                    || ve.ClassListContains("unity-foldout__toggle")
                    || ve.ClassListContains("unity-base-field")
                ).First();

        public static VisualElement QueryContent(VisualElement root) 
            => root?.Query<VisualElement>()
                .Where(ve => 
                    ve.ClassListContains("custom-field-container__content")
                    || ve.ClassListContains("unity-foldout__content")
                ).First();

        public static VisualElement FindParent(
            VisualElement element, string path
        ) {
            var ve = element;
            for (;
                ve != null && ve.name != $"Jenga.ALay:{path}"
                && ve.name != $"PropertyField:{path}";
                ve = ve.parent);

            return ve;
        }

        // Style layouters
        [CustomLayouterAttribute(typeof(ALay.FlexGrowAttribute))]
        public class FlexGrowLayouter : Layouter {
            public override void OnLayout()
                => element.style.flexGrow 
                    = ((ALay.FlexGrowAttribute)attribute).value;
        }

        [CustomLayouterAttribute(typeof(ALay.FlexShrinkAttribute))]
        public class FlexShrinkLayouter : Layouter {
            public override void OnLayout()
                => element.style.flexShrink
                    = ((ALay.FlexShrinkAttribute)attribute).value;
        }

        [CustomLayouterAttribute(typeof(ALay.MinWidthAttribute))]
        public class MinWidthLayouter : Layouter {
            public override void OnLayout()
                => element.style.minWidth
                    = ((ALay.MinWidthAttribute)attribute).value;
        }

        [CustomLayouterAttribute(typeof(ALay.MaxWidthAttribute))]
        public class MaxWidthLayouter : Layouter {
            public override void OnLayout()
                => element.style.maxWidth
                    = ((ALay.MaxWidthAttribute)attribute).value;
        }

        [CustomLayouterAttribute(typeof(ALay.ListViewAttribute))]
        public class ListViewLayouter : Layouter {
            public override void OnLayout()
                => element.schedule.Execute(OnSchedule).StartingIn(100);

            void OnSchedule() {
                var attr = (ALay.ListViewAttribute)attribute;
                var lv = element.Q<ListView>();
                if (lv == null) return; 

                lv.reorderable = attr.reorderable;
                lv.showFoldoutHeader = attr.showFoldoutHeader;
                lv.showAddRemoveFooter = attr.showAddRemoveFooter;
                lv.showBoundCollectionSize = attr.showBoundCollectionSize;
                lv.virtualizationMethod 
                    = CollectionVirtualizationMethod.DynamicHeight;

                lv.itemsAdded += (indices) => {
                    foreach (var index in indices) {
                        var propItem = property.GetArrayElementAtIndex(index);
                        var endProp = propItem.GetEndProperty(true);

                        while (propItem.Next(true) 
                            && !SerializedProperty
                                .EqualContents(property, endProp)
                        ) if (propItem.propertyType 
                            == SerializedPropertyType.ManagedReference)
                            propItem.managedReferenceId 
                                = ManagedReferenceUtility.RefIdNull;
                    }

                    property.serializedObject.ApplyModifiedProperties();
                };

                lv.Rebuild();
            }
        }

        // Groups & Placement
        [CustomLayouterAttribute(typeof(ALay.BeginRowGroupAttribute))]
        public class BeginRowGroupLayouter : Layouter {
            public override void OnLayout() {
                if (groupStack.TryPeek(out var parent)) {
                    var newParent = new VisualElement() {
                        style = { flexDirection = FlexDirection.Row },
                        name = $"Jenga.ALay:group-row--{groupStack.Count}"
                    };

                    parent.Add(newParent);
                    
                    groupStack.Push(newParent);
                }
            }
        }

        [CustomLayouterAttribute(typeof(ALay.EndGroupAttribute))]
        public class EndGroupLayouter : Layouter {
            public override void OnAfterLayout() => groupStack.Pop();
        }

        [CustomLayouterAttribute(typeof(ALay.PlaceInHeaderAttribute))]
        public class PlaceInHeaderLayouter : Layouter {

            public override void OnLayout() {
                element.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            }

            void OnAttachToPanel(AttachToPanelEvent evt) {
                element.UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
                var root = FindParent(element, property.propertyPath);
                var header = QueryHeader(root); 
                var attr = (ALay.PlaceInHeaderAttribute)attribute;

                if (header != null) {
                    element.RemoveFromHierarchy();
                    header.Insert(attr.pos, element);
                }
            }
        }

        [CustomLayouterAttribute(typeof(ALay.LabelAttribute))]
        public class LabelLayouter : Layouter {

            public override void OnLayout() {
                element.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
            }

            void OnAttachToPanel(AttachToPanelEvent evt) {
                element.UnregisterCallback<AttachToPanelEvent>(OnAttachToPanel);
                var root = FindParent(element, property.propertyPath);
                var header = QueryHeader(root); 
                var attr = (ALay.LabelAttribute)attribute;
                var label = header?.Q<Label>();

                if (label != null)
                    label.text = attr.value;
            }
        }

        // [CustomLayouterAttribute(typeof(ALay.EmitFieldAttribute))]
        // public class EmitFieldLayouter : Layouter {

        //     public override void OnLayout() {
        //         var methodInfo = memberInfo as MethodInfo;
        //         var res = methodInfo.Invoke(null, null);

        //         if (res is ALay.FieldAttribute[] attrs) 


        //     }
        // }

        [CustomLayouterAttribute(typeof(ALay.HideLabelAttribute))]
        public class HideLabelLayouter : Layouter {

            public override void OnLayout() {
                // element.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
                element.schedule.Execute(OnSchedule).StartingIn(100);
            }

            void OnSchedule() {
                // var root = FindParent(element, property.propertyPath);
                var header = QueryHeader(element); 
                var label = header?.Q<Label>();
                var attr = (ALay.HideLabelAttribute)attribute;
                if (label != null)
                    label.style.display = DisplayStyle.None;
            }
        }

        [CustomLayouterAttribute(typeof(ALay.UseRootLabelAttribute))]
        public class UseRootLabelLayouter : Layouter {

            public override void OnLayout() {
                // element.RegisterCallback<AttachToPanelEvent>(OnAttachToPanel);
                element.schedule.Execute(OnSchedule).StartingIn(100);
            }
 
            void OnSchedule() {
                var root = FindParent(element, property.propertyPath);
                var header = QueryHeader(element)?.Q<Label>();

                if (header != null) 
                    header.text = preferredParentLabel; 
                
            }
        }

        [CustomLayouterAttribute(typeof(ALay.DelayAttributeAttribute))]
        public class DelayAttributeLayouter : Layouter {
            
            Layouter layouter;

            public override void OnLayout() {
                var attr = (ALay.DelayAttributeAttribute)attribute;
                var flags 
                    = BindingFlags.Static 
                    | BindingFlags.NonPublic
                    | BindingFlags.Public;

                var myType 
                    = SerializedPropertyUtility
                        .GetFieldType(memberInfo as FieldInfo);

                var methodInfo 
                    = attr.inClass 
                        ? (myType?.GetMethod(attr.name, flags))
                        : memberInfo.DeclaringType.GetMethod(attr.name, flags);

                if (methodInfo == null) return;

                var newAttr = methodInfo.Invoke(null, null);
                if (newAttr == null) return;

                layouter = MakeLayouter(newAttr.GetType());
                
                layouter.property = property;
                layouter.element = element;
                layouter.memberInfo = memberInfo;
                layouter.groupStack = groupStack;
                layouter.attribute = newAttr;
                layouter.preferredParentLabel = preferredParentLabel;

                layouter.OnLayout();
            }

            public override void OnAfterLayout() {
                if (layouter != null)
                    layouter.OnAfterLayout();
            }
        }

        // [CustomLayouterAttribute(typeof(ALay.OnChangeCallbackAttribute))]
        // public class OnChangeCallbackLayouter : Layouter {
            
        //     public override void OnLayout() {
        //         var attr = (ALay.OnChangeCallbackAttribute)attribute;
        //         var flags 
        //             = BindingFlags.Static 
        //             | BindingFlags.NonPublic
        //             | BindingFlags.Public;

        //         var myType 
        //             = SerializedPropertyUtility
        //                 .GetFieldType(memberInfo as FieldInfo);

        //         var methodInfo 
        //             = attr.inClass 
        //                 ? (myType?.GetMethod(attr.name, flags))
        //                 : memberInfo.DeclaringType.GetMethod(attr.name, flags);

        //         if (methodInfo == null) return;

        //         element.schedule.Execute(() => {
                    
        //         })
        //         methodInfo.Invoke(null, new object[] { property });


        //     }
        // }

        [CustomLayouterAttribute(typeof(ALay.HideHeaderAttribute))]
        public class HideHeaderLayouter : Layouter {

            public override void OnLayout() {
                // Debug.Log("Hide Header");
                element.schedule.Execute(OnSchedule).StartingIn(100);
            }

            void OnSchedule() {
                // var root = FindParent(element, property.propertyPath);
                var header = QueryHeader(element); 
                var content = QueryContent(element); 
                var attr = (ALay.HideHeaderAttribute)attribute;

                var toggle = header?.Q<Toggle>();

                if (toggle != null)
                    toggle.value = true;
                if (header != null)
                    header.style.display = DisplayStyle.None;
                if (content != null)
                    content.style.marginLeft = 0f;
            }
        }

        // Visuals
        [CustomLayouterAttribute(typeof(ALay.UsageToggleAttribute))]
        public class UsageToggleLayouter : Layouter {

            public override void OnLayout() {
                var group = new VisualElement() {
                    name = $"Jenga.ALay:group-usage--{groupStack.Count}",
                    style = { 
                        flexDirection = FlexDirection.Row, 
                        alignItems = Align.Center,
                        flexGrow = 1f
                    },
                };

                if (groupStack.TryPeek(out var parent))
                    parent.Add(group);

                var toggle = new Toggle() {
                    label = "",
                    bindingPath = ((ALay.UsageToggleAttribute)attribute).path,
                    style = {
                        minWidth = 20f,
                        flexShrink = 1f
                    }
                };

                element.style.flexGrow = 1f;
                element.SetEnabled(toggle.value);

                toggle.RegisterValueChangedCallback(
                    evt => element.SetEnabled(evt.newValue)
                );
                
                group.Add(toggle);
                groupStack.Push(group);
            }

            public override void OnAfterLayout() => groupStack.Pop();
        }

        [CustomLayouterAttribute(typeof(ALay.TypeSelectorAttribute))]
        public class TypeSelectorLayouter : Layouter {

            public override void OnLayout() {
                element.schedule.Execute(OnSchedule).StartingIn(100);
            }

            void OnSchedule() {
                var root = FindParent(element, property.propertyPath);
                var header = QueryHeader(root); 
                var attr = (ALay.TypeSelectorAttribute)attribute;
                var prop = attr.path != null 
                    ? property.FindPropertyRelative(attr.path) 
                    : property;

                if (header == null) return;

                var typeSelector = new TypeSelectorField() {
                    currentType 
                        = SerializedPropertyUtility.GetManagedType(prop),
                    typeFamily = attr.typeFamily,
                    onSelect = (type) => {
                        element.Unbind();
                        SerializedPropertyUtility
                            .SetManagedReference(prop, type);
                        prop.serializedObject.ApplyModifiedProperties();
                        prop.serializedObject.Update();
                        element.Bind(prop.serializedObject);
                    },
                    style = { 
                        minWidth = 150f, maxWidth = 150f
                    }
                };

                header.Insert(header.childCount, typeSelector);
            }
        }

    }

}
#endif