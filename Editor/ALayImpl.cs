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
            var root = new FieldContainer() { label = preferredLabel };

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

                var wasLayouted = false;
                foreach (var attr in attrs) {
                    var layouter = ALayLayouter.Get(attr.GetType());
                    if (layouter == null) continue;

                    var ctx = new ALayContext(
                        member, memberPath, attr,
                        property.serializedObject
                    );

                    layouter.Layout(ve, ctx);
                    wasLayouted = true;
                }

                if (memberProp == null && !wasLayouted)
                    continue;

                root.content.Add(ve);
            }

            foreach (var attr in myType.GetCustomAttributes(true)) {
                var layouter = ALayLayouter.Get(attr.GetType());
                if (layouter == null) continue; 

                var ctx = new ALayContext(
                    fieldInfo, property.propertyPath, attr,
                    property.serializedObject
                );

                layouter.Layout(root, ctx);
            }

            return root;
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

        public SerializedProperty property;

        public ALayContext(
            MemberInfo memberInfo, string path,
            object attribute, SerializedObject so
        ) {
            this.memberInfo = memberInfo;
            this.path = path;
            this.attribute = attribute;
            this.serializedObject = so;
            this.name = memberInfo.Name;

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

            if (!float.IsNaN(attr.flexGrow))   ve.style.flexGrow   = attr.flexGrow;
            if (!float.IsNaN(attr.flexShrink)) ve.style.flexShrink = attr.flexShrink;
            if (!float.IsNaN(attr.minWidth))   ve.style.minWidth   = attr.minWidth;
            if (!float.IsNaN(attr.minHeight))  ve.style.minHeight  = attr.minHeight;
            if (!float.IsNaN(attr.maxWidth))   ve.style.maxWidth   = attr.maxWidth;
            if (!float.IsNaN(attr.maxHeight))  ve.style.maxHeight  = attr.maxHeight;
            if (!float.IsNaN(attr.width))      ve.style.width      = attr.width;
            if (!float.IsNaN(attr.height))     ve.style.height     = attr.height;
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

        public override void Layout(VisualElement ve, ALayContext ctx) {
            var attr = ctx.GetAttribute<ALay.TypeSelectorAttribute>();
            var prop = ctx.property.FindPropertyRelative(attr.path);
            if (ve is FieldContainer container && prop != null) {
                container.header.Add(new TypeSelectorField() {
                    typeFamily = attr.typeFamily,
                    currentType = SerializedPropertyUtility
                        .GetManagedType(prop),
                    onSelect = (t) => {
                        SerializedPropertyUtility.SetManagedReference(prop, t);
                        prop.serializedObject.ApplyModifiedProperties();
                    }
                });
            }
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
                    ctx.serializedObject
                );

                layouter.Layout(ve, delayedCtx);
            }
        }
    }

}
#endif