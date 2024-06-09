using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Jenga {
    internal static class TilingGizmos {

        [InitializeOnLoadMethod]
        static void Init() {
            // EditorCallbacks.CallOnSceneGUI<RectTile>(RectTileOnMove);
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

        // static void RectTileOnMove(RectTile self, SceneView view) {
        //     var parent = self.transform.parent;
        //     var position = self.transform.position;
        //     var map = self.tilemap;
        //     var mapTransform = map != null ? map.transform : null;
        //     var name = self.gameObject.name;
        //     var mapName = map != null ? map.gameObject.name : "";
        //     var oldTilePos = self.position;

        //     if (parent != mapTransform) {
        //         Undo.SetTransformParent(self.transform, mapTransform, 
        //             map != null ? $"Snap tile {name} to {mapName}"
        //             : $"Deparent tile {name}"
        //         );

        //         var oldMap = parent.GetComponent<RectTilemap>();

        //         if (oldMap != null) {
        //             oldMap.RemoveTile(self);
        //             oldMap.ReturnDuplicates(oldTilePos);
        //         }

        //         if (map != null) {
        //             var oldTile = map.GetTile(newTilePos);
        //             if (oldTile != null) map.AddDuplicate(oldTile);
        //             map.SetTile(newTilePos, self);
        //         }
        //     }

        //     if (map == null) return;

        //     var basis = map.basis;
        //     var newTilePos = RectTiling.WorldToTile(basis, position); 

        //     if (newTilePos != oldTilePos) {
        //         map.RemoveTile(self);
        //         map.ReturnDuplicates(oldTilePos);
        //         var oldTile = map.GetTile(position);
        //         if (oldTile != null) map.AddDuplicate(oldTile);
        //         map.SetTile(position, self);
        //     }
        // }

        static void RectTilemapOnGizmos(RectTilemap self, SceneView view) {
            if (!Gizmo.ShouldDrawGizmos<RectTilemap>()) return;
            Gizmo.DrawGrid(
                self.basis.origin, self.basis.axisX, 
                view.in2DMode ? self.basis.axisY : self.basis.axisZ
            );

            if (self.collectSetTimes) 
            foreach (var (chunkPos, chunk) in self.chunks.Items())
            for(int x = 0; x < RectTilemap.chunkSizeX; ++x) 
            // for(int y = 0; y < RectTilemap.chunkSizeY; ++y)  
            for(int z = 0; z < RectTilemap.chunkSizeZ; ++z) { 
                // Debug.Log($"{chunkPos}, {chunk}");
                var chunkIndex = Math.Int3(x, 0, z);
                var currentTime = (float)EditorApplication.timeSinceStartup;
                var time = (currentTime - chunk.times[x, 0, z]) / 10f;
                var alpha = 1f - 2 / Math.PI * Math.Atan(time);
                var tile 
                    = RectTiling.ChunkOrigin(RectTilemap.chunkSize, chunkPos)
                    + chunkIndex;
                var pos = RectTiling.TileOrigin(self.basis, tile);

                var p00 = pos;
                var p01 = pos + self.basis.axisZ; 
                var p11 = pos + self.basis.axisX + self.basis.axisZ;
                var p10 = pos + self.basis.axisX;

                var col = Math.Lerp(
                    new Color(0f, 0f, 0f, .2f), 
                    new Color(0f, 1f, 0f, .2f), 
                    alpha
                );
                Handles.DrawSolidRectangleWithOutline(
                    new Vector3[] { p00, p01, p11, p10 },
                    col, Color.black
                );
            }
        }

    }
}
