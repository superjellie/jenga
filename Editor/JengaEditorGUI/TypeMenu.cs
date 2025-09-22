using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {

        public static void TypeMenu(
            Rect rect, PathTree<TypeMenuEntry> tree, 
            System.Type currentType, System.Action<System.Type> setNewType
        ) {
            var name = currentType != null ? "[UNKNOWN]" : "[NULL]";

            if (currentType.TryGetAttribute<AddTypeMenuAttribute>(out var atm)) 
                name = atm.path.Remove(0, atm.path.LastIndexOf('/') + 1);

            var content = new GUIContent(name);
            
            if (EditorGUI.DropdownButton(rect, content, FocusType.Passive)) {
                var menu = new GenericMenu();
                
                GenericMenu.MenuFunction2 action 
                    = o => setNewType(o as System.Type);

                foreach (var (path, reg) in tree.Walk()) {
                    var entryContent = new GUIContent(path);

                    menu.AddItem(
                        entryContent, reg.type == currentType,
                        action, reg.type
                    );
                }

                menu.DropDown(rect);
            } 
        }

        public static void TypeMenu(Rect rect, SerializedProperty property) {
            var family = property.GetFieldType();
            var foundFamily = AddTypeMenuAttribute.registry.TryFindSubtree(
                reg => reg.type == family, out var familyTree
            );

            if (!foundFamily) {
                GUI.Label(rect, "No Family");
                return;
            }

            TypeMenu(
                rect, familyTree, property.GetCurrentType(), 
                (type) => property.TrySetType(type)
            );

            TypeMenuContext(rect, property);
        }

        public static void TypeMenuContext(
            Rect rect, SerializedProperty property
        ) {


            var evt = Event.current;
            if (evt.type == EventType.ContextClick 
                && rect.Contains(evt.mousePosition)
            ) {
                var family = property.GetFieldType();
                var foundFamily = AddTypeMenuAttribute.registry.TryFindSubtree(
                    reg => reg.type == family, out var familyTree
                );
                var menu = new GenericMenu();

                var cntCopy = new GUIContent("Copy");
                var cntPaste = new GUIContent("Paste");
                var cntEdit = new GUIContent("Edit Script");
                var cntTree = new GUIContent("Tree View");

                var pasteValue = SerializedPropertyExtensions
                    .ParseFromClipboard(property.GetFieldType());

                menu.AddItem(cntCopy, false, () => property.ClipCopy());

                if (pasteValue != null)
                    menu.AddItem(cntPaste, false, 
                        () => property.FillFromValue(pasteValue));


                foreach (var (path, reg) in familyTree.GetWrappers()) {
                    var cntWrap = new GUIContent($"Wrap With/{path}");
                    menu.AddItem(
                        cntWrap, false, 
                        () => property.WrapWith(reg.type)
                    );
                }

                if (property.IsWrapper()) {
                    var cntReplace = new GUIContent("Replace With Child");
                    menu.AddItem(
                        cntReplace, false, 
                        () => property.ReplaceWithWrapped()
                    );
                }

                menu.AddItem(
                    cntTree, false,
                    () => PropertyTreeWindow.Edit(property)
                );

                menu.AddItem(
                    cntEdit, false, 
                    () => property.EditScript()
                );

                menu.DropDown(rect);
                evt.Use();
            }
        }

    }
}
