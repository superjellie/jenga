using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEditor;

namespace Jenga {
    public static partial class JengaEditorGUI {

        static Dictionary<SerializedReferenceLink, List<Vector3>> 
            drawnLinkPlugs = new();

        static bool drawFlags = false;
        static int cablePlugsCount = 0;
        static SerializedReferenceLink draggedLink
            = SerializedReferenceLink.Null;

        public static void BeginCablePlugsGroup() {
            drawFlags = true;
            drawnLinkPlugs.Clear();
            cablePlugsCount = 0;
            draggedLink = SerializedReferenceLink.Null;
        }
        
        public static void EndCablePlugsGroup() {
            drawFlags = false;
            // // var draggedFieldType = draggedProperty?.GetFieldType();

            // foreach (var (link, centers) in drawnLinkPlugs) {
            //     centers.Sort((c1, c2) => c1.x.CompareTo(c2.x));
            //     if (link == draggedLink) continue;

            //     for (int i = 0; i < centers.Count - 1; ++i) {
            //         var w1 = GUIUtility.ScreenToGUIPoint(centers[i]);
            //         var w2 = GUIUtility.ScreenToGUIPoint(centers[i + 1]);
            //         var length = (w2 - w1).magnitude * 1.05f;

            //         Catenary(w1, w2, length, 
            //             Color.white, JengaAssets.texThread);
            //     }

            // }

            // foreach (var (link, centers) in drawnLinkPlugs) {
            //     centers.Sort((c1, c2) => c1.x.CompareTo(c2.x));
            //     for (int i = 0; i < centers.Count; ++i) {
            //         var center = GUIUtility.ScreenToGUIPoint(centers[i]);

            //         var isDragged = link == draggedLink;
            //         var linkCount = link.GetLinkedPropertiesCount();

            //         GUI.DrawTexture(
            //             new Rect(center.x - 6f, center.y - 13f, 20f, 20f),
            //             linkCount > 1 
            //                 ? JengaAssets.texPinFull : JengaAssets.texPinEmpty, 
            //             ScaleMode.ScaleToFit, true, 0f,
            //             isDragged 
            //                 ? Color.yellow
            //                 : new Color(.8f, .8f, .8f, 1f), 
            //             0f, 0f
            //         );
            //     }
            // }

            // if (draggedLink != SerializedReferenceLink.Null)
            // if (drawnLinkPlugs.TryGetValue(draggedLink, out var centers)) {
            //     centers.Sort((c1, c2) => c1.x.CompareTo(c2.x));

            //     for (int i = 0; i < centers.Count - 1; ++i) {
            //         var w1 = GUIUtility.ScreenToGUIPoint(centers[i]);
            //         var w2 = GUIUtility.ScreenToGUIPoint(centers[i + 1]);
            //         var length = (w2 - w1).magnitude * 1.05f;

            //         Catenary(w1, w2, length, 
            //             Color.yellow, JengaAssets.texThread);
            //     }

            // }
        }

        public static void LinkCablePlug(
            Rect rect, SerializedProperty property
        ) {
            if (!drawFlags) return;
            if (!property.IsManagedReference()) return;
            if (property.managedReferenceValue == null) return;

            var key = property.GetLink();
            var center = rect.center;
            var linkCount = key.GetLinkedPropertiesCount();

            // var wCenter = GUIUtility.GUIToScreenPoint(center)
            //     - new Vector2(2f, 2f);


            // if (drawnLinkPlugs.TryGetValue(key, out var list))
            //     list.Add(wCenter);
            // else
            //     drawnLinkPlugs.Add(key, new List<Vector3>() { wCenter }); 

            GUI.DrawTexture(
                //new Rect(center.x - 3f, center.y - 10f, 18f, 18f),
                rect,
                linkCount > 1 
                    ? JengaAssets.texPinFull : JengaAssets.texPinEmpty, 
                ScaleMode.ScaleToFit, true, 0f,
                new Color(.8f, .8f, .8f, 1f), 
                0f, 0f
            );


            var evt = Event.current;
            if (evt.type == EventType.ContextClick 
                && rect.Contains(evt.mousePosition)
            ) {
                var menu = new GenericMenu();
                var so = property.serializedObject;
                var id = property.managedReferenceId;

                foreach (var prop in SerializedReferenceUtility
                    .GetAllLinkedProperties(so, id)
                ) {
                    var path = property.propertyPath == prop.propertyPath 
                        ? $"=> {prop.propertyPath.Replace(".Array.data", "")}"
                        : prop.propertyPath.Replace(".Array.data", "");

                    var cntUnlink = new GUIContent($"{path}/Unlink");

                    menu.AddItem(cntUnlink, false,
                        () => {
                            prop.managedReferenceId 
                                = ManagedReferenceUtility.RefIdNull;
                            prop.serializedObject.ApplyModifiedProperties();
                        }
                    );
                }

                menu.DropDown(rect);
                evt.Use();
            }

            // if (evt.button == 0) 
            // if (DragArea(rect, $"plug:{cablePlugsCount++}", MouseCursor.Pan)) {
            //     if (drawnLinkPlugs.TryGetValue(key, out var list1)) {
            //         list1.Add(GUIUtility
            //             .GUIToScreenPoint(Event.current.mousePosition));
            //         draggedLink = key;
            //     }
            // }

            // if (drawnLinkPlugs.Count > 1000) {
            //     Debug.LogError("Too many plugs!");
            //     drawnLinkPlugs.Clear();
            // }

            // GUI.Toggle(rect, false, "", EditorStyles.radioButton);
            
            
        }

    }
}
