using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    internal static class TilingGizmos {

        [InitializeOnLoadMethod]
        static void Init() {
            EditorCallbacks.CallOnSceneGUI<RectTile>(RectTileOnMove);
            // EditorCallbacks.CallOnDestroyObject(RectTileOnDestroy);
            EditorCallbacks.CallOnSceneGUI<RectTilemap>(RectTilemapOnGizmos);
        }

        // static void RectTileOnDestroy(DestroyGameObjectHierarchyEventArgs arg) {
        //     var id = arg.parentInstanceId;
        //     var go = EditorUtility.InstanceIDToObject(id) as GameObject;
        //     Debug.Log($"{go}");
        //     var tm = go != null ? go.GetComponent<RectTilemap>() : null;
        //     if (tm != null && tm.autoRebuildMapOnTileMoves)
        //         tm.RebuildMap();
        // }

        static void RectTileOnMove(RectTile self, SceneView view) {
            if (self.tilemap == null) return;

            var pos = self.transform.position;
            var basis = self.tilemap.basis;

            var tile = RectTiling.WorldToTile(basis, pos);
            var newWorld = RectTiling.TileCenter(basis, tile);

            if (Math.Distance(pos, newWorld) > Math.TINY 
                || self.position != tile) {
                // Debug.Log($"pos = {pos}, tile = {tile}, newWorld = {newWorld}");
                Undo.RecordObject(
                    self.transform, 
                    $"Snap {self.gameObject.name} to Tilemap"
                );
                self.transform.position = newWorld;
                var oldPosition = self.position;
                self.position = tile;
                if (self.tilemap.autoRebuildMapOnTileMoves)
                    if (oldPosition != tile)
                        self.tilemap.RebuildMap();
            }

            if (self.transform.parent != self.tilemap.transform) {
                var parentTilemap = 
                    self.transform.parent != null ?
                        self.transform.parent.GetComponent<RectTilemap>()
                    : null;
                Undo.SetTransformParent(
                    self.transform, self.tilemap.transform,
                    $"Snap {self.gameObject.name} to Tilemap"
                );

                if (parentTilemap != null) 
                    if (self.tilemap.autoRebuildMapOnTileMoves)
                        parentTilemap.RebuildMap();
                if (self.tilemap.autoRebuildMapOnTileMoves)
                    self.tilemap.RebuildMap();
            }
        }

        static void RectTilemapOnGizmos(RectTilemap self, SceneView view) {
            Gizmo.DrawGrid(
                self.basis.origin, self.basis.axisX, 
                view.in2DMode ? self.basis.axisY : self.basis.axisZ
            );
        }

    }
}
