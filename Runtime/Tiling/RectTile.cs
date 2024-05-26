using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    [AddComponentMenu("Jenga/RectTile", 100)]
    public class RectTile : MonoBehaviour {
        [EnumStyle] public RectTilemap tilemap;
        [ReadOnly] public Vector3Int position;
    }
}
