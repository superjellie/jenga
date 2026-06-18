using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class SceneGraphVertex : MonoBehaviour {

        // Index in DistributedSceneGraph
        // Should be stable during lifetime of vertex
        // These values controlled by ownerGraph
        [ReadOnly] public int vertexIndex = 0;
        [ReadOnly] public DistributedSceneGraph ownerGraph;

    }
}
