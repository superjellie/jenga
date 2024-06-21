using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [AddComponentMenu("Jenga/RectTilemap", 101)]
    public class RectTilemap : MonoBehaviour {

        public const int chunkSizeX = 4;
        public const int chunkSizeY = 1;
        public const int chunkSizeZ = 4;
        public static Vector3Int chunkSize 
            = Mathx.Int3(chunkSizeX, chunkSizeY, chunkSizeZ);

        [System.Serializable]
        public class Chunk {
            public RectTile[,,] tiles 
                = new RectTile[chunkSizeX, chunkSizeY, chunkSizeZ];

            public RectTile this[Vector3Int pos] {
                get => this.tiles[pos.x, pos.y, pos.z];
                set => this.tiles[pos.x, pos.y, pos.z] = value;
            }
        #if UNITY_EDITOR
            public float[,,] times 
                = new float[chunkSizeX, chunkSizeY, chunkSizeZ];
        #endif
        }

        public RectTiling.Basis basis;
        public ADT.Map<Vector3Int, Chunk> chunks 
            = new ADT.Map<Vector3Int, Chunk>();

        public RectTile TileAt(Vector3Int position)  {
            var chunk = this.chunks[RectTiling.TileChunk(chunkSize, position)];
            if (chunk == null) return null;
            var index = RectTiling.TileInChunkIndex(chunkSize, position);
            return chunk[index];
        }
        public bool HasTileAt(Vector3Int position) 
            => this.TileAt(position) != null;        
        public T GetComponentInTile<T>(Vector3Int position) where T : Component
            => this.TileAt(position) != null ? 
                this.TileAt(position).GetComponent<T>()
                : null;
        public void SetTile(Vector3Int position, RectTile tile) {
            var chunkCoord = RectTiling.TileChunk(chunkSize, position);
            var chunk = chunks[chunkCoord];
            if (chunk == null) {
                chunk = new Chunk();
                chunks[chunkCoord] = chunk;
            }

            var chunkIndex = RectTiling.TileInChunkIndex(chunkSize, position);
            // Debug.Log($"{position}, {chunkCoord}, {chunkIndex}"); 
            chunk[chunkIndex] = tile;
        #if UNITY_EDITOR
            if (collectSetTimes)
                chunk.times[chunkIndex.x, chunkIndex.y, chunkIndex.z] 
                    = (float)UnityEditor.EditorApplication.timeSinceStartup;
        #endif
        }

        [Message] void Awake() => this.RebuildMap();

        [ContextMenu("Rebuild Map")]
        public void RebuildMap() {
        #if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Rebuild Map");
        #endif
            
            var children = this.GetComponentsInChildren<RectTile>();
            this.chunks.Clear();
            foreach (var tile in children)
                if (tile.tilemap == this)
                    this.SetTile(tile.position, tile);
        }   

        public void RebuildChunk(Vector3Int chunk) {
        #if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Rebuild Map Chunk");
        #endif
            // chunks[]
        }

    #if UNITY_EDITOR
        [Header("Debug")]
        public bool autoRebuildMapOnTileMoves = true;
        public bool collectSetTimes = false;
        [Message] void OnDrawGizmos() { }
    #endif
    }
}
