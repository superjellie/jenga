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
                m.SetColumn(0, Mathx.Vec4(axisX.x, axisX.y, axisX.z, 0f));
                m.SetColumn(1, Mathx.Vec4(axisY.x, axisY.y, axisY.z, 0f));
                m.SetColumn(2, Mathx.Vec4(axisZ.x, axisZ.y, axisZ.z, 0f));
                m.SetColumn(3, Mathx.Vec4(origin.x, origin.y, origin.z, 1f));
                return m;
            }}

            public Matrix4x4 worldToLocal => localToWorld.inverse;
        }


        // Coordinate transforms
        public static Vector3Int WorldToTile(Basis basis, Vector3 point) 
            => Mathx.FloorToInt((Vector3)(basis.worldToLocal * point));

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
            => Mathx.Abs(a.x - b.x) 
            + Mathx.Abs(a.y - b.y) 
            + Mathx.Abs(a.z - b.z);

        public static 
        float DistanceWorld(Basis basis, Vector3Int a, Vector3Int b)
            => Mathx.Distance(TileCenter(basis, a), TileCenter(basis, b));

        // Snapping
        public static Vector3 SnapToTileCenter(Basis basis, Vector3 point)
            => TileCenter(basis, WorldToTile(basis, point));
        public static Vector3 SnapToTileOrigin(Basis basis, Vector3 point)
            => TileOrigin(basis, WorldToTile(basis, point));
        public static Vector3 SnapToAxes(Basis basis, Vector3 direction)
            => direction.magnitude * AQRY.MinBy(
                ArrayView.Params(
                    basis.axisX, -basis.axisX,
                    basis.axisY, -basis.axisY,
                    basis.axisZ, -basis.axisZ
                ),
                (x, i) => Mathx.Distance(x, direction)
            );

        // Chunk support
        public static 
        Vector3Int TileChunk(Vector3Int chunkSize, Vector3Int tile) 
            => Mathx.Int3(
                Mathx.Div(tile.x, chunkSize.x), 
                Mathx.Div(tile.y, chunkSize.y), 
                Mathx.Div(tile.z, chunkSize.z)
            ); 

        public static 
        Vector3Int ChunkOrigin(Vector3Int chunkSize, Vector3Int chunk) 
            => Mathx.Int3(
                chunk.x * chunkSize.x, 
                chunk.y * chunkSize.y, 
                chunk.z * chunkSize.z
            );

        public static 
        Vector3Int TileInChunkIndex(Vector3Int chunkSize, Vector3Int tile) 
            => Mathx.Int3(
                Mathx.Mod(tile.x, chunkSize.x), 
                Mathx.Mod(tile.y, chunkSize.y), 
                Mathx.Mod(tile.z, chunkSize.z)
            );

        public static bool AreTilesInOneChunks(
            Vector3Int chunkSize, Vector3Int oldTile, Vector3Int newTile
        ) => TileChunk(chunkSize, oldTile) == TileChunk(chunkSize, newTile);
    }
}
