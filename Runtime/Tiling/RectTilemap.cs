using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [AddComponentMenu("Jenga/RectTilemap", 101)]
    public class RectTilemap : MonoBehaviour {
        public RectTiling.Basis basis;
        public ADT.Map<Vector3Int, RectTile> tiles 
            = new ADT.Map<Vector3Int, RectTile>();

        public RectTile TileAt(Vector3Int position) => this.tiles[position];
        public bool HasTileAt(Vector3Int position) 
            => this.tiles[position] != null;        
        public T GetComponentInTile<T>(Vector3Int position) where T : Component
            => this.tiles[position] != null ? 
                this.tiles[position].GetComponent<T>()
                : null;

        [Message] void Awake() => this.RebuildMap();

        [ContextMenu("Rebuild Map")]
        public void RebuildMap() {
        #if UNITY_EDITOR
            UnityEditor.Undo.RecordObject(this, "Rebuild Map");
        #endif
            
            var children = this.GetComponentsInChildren<RectTile>();
            this.tiles.Clear();
            foreach (var tile in children)
                this.tiles[tile.position] = tile;
        }   
    #if UNITY_EDITOR
        [Header("Debug")]
        public bool autoRebuildMapOnTileMoves = true;
        [Message] void OnDrawGizmos() { }
    #endif
    }
}
