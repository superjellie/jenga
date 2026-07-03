using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class SceneGraphEdge : MonoBehaviour {

        // Index in DistributedSceneGraph
        // Should be stable during lifetime of edge
        // These values controlled by ownerGraph
        [HideInInspector] public Vector2Int edgeIndex = Vector2Int.zero;
        [HideInInspector] public int edgeId = 0;
        [HideInInspector] public DistributedSceneGraph ownerGraph;
    }
}
