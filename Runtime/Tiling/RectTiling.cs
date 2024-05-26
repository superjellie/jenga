using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public static class RectTiling {

        [System.Serializable]
        public class Basis {
            public Vector3 origin = Vector3.zero;
            public Vector3 axisX = Vector3.right;
            public Vector3 axisY = Vector3.up;
            public Vector3 axisZ = Vector3.forward;

            public Matrix4x4 localToWorld { get {
                var m = new Matrix4x4();
                m.SetColumn(0, Math.Vec4(axisX.x, axisX.y, axisX.z, 0f));
                m.SetColumn(1, Math.Vec4(axisY.x, axisY.y, axisY.z, 0f));
                m.SetColumn(2, Math.Vec4(axisZ.x, axisZ.y, axisZ.z, 0f));
                m.SetColumn(3, Math.Vec4(origin.x, origin.y, origin.z, 1f));
                return m;
            }}

            public Matrix4x4 worldToLocal => localToWorld.inverse;
        }


        // Coordinate transforms
        public static Vector3Int WorldToTile(Basis basis, Vector3 point) 
            => Math.FloorToInt((Vector3)(basis.worldToLocal * point));

        public static Vector3 TileCenter(Basis basis, Vector3Int tile) 
            => basis.origin 
            + (tile.x + .5f) * basis.axisX
            + (tile.y + .5f) * basis.axisY
            + (tile.z + .5f) * basis.axisZ;

        public static Vector3 TileOrigin(Basis basis, Vector3Int tile) 
            => basis.origin 
            + tile.x * basis.axisX
            + tile.y * basis.axisY
            + tile.z * basis.axisZ;

        public static Vector3Int[] NeighbourTiles(Vector3Int tile) 
            => new Vector3Int[] { 
                tile + Vector3Int.right,
                tile + Vector3Int.left,
                tile + Vector3Int.up,
                tile + Vector3Int.down,
                tile + Vector3Int.forward,
                tile + Vector3Int.back,
            };

        // Distance on grid
        public static int Distance(Vector3Int a, Vector3Int b)
            => Math.Abs(a.x - b.x) 
            + Math.Abs(a.y - b.y) 
            + Math.Abs(a.z - b.z);

        public static 
        float DistanceWorld(Basis basis, Vector3Int a, Vector3Int b)
            => Math.Distance(TileCenter(basis, a), TileCenter(basis, b));

        // Snapping
        public static Vector3 SnapToTileCenter(Basis basis, Vector3 point)
            => TileCenter(basis, WorldToTile(basis, point));
        public static Vector3 SnapToTileOrigin(Basis basis, Vector3 point)
            => TileOrigin(basis, WorldToTile(basis, point));
        public static Vector3 SnapToAxes(Basis basis, Vector3 direction)
            => direction.magnitude * AQRY.MinBy(
                new Vector3[] {
                    basis.axisX, -basis.axisX,
                    basis.axisY, -basis.axisY,
                    basis.axisZ, -basis.axisZ,
                },
                x => Math.Distance(x, direction)
            );

        // Tile transform
    }
}