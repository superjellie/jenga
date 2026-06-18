using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class SceneGraphVertex : MonoBehaviour {

        // Index in DistributedSceneGraph
        // Should be stable during lifetime of vertex
        // These values controlled by ownerGraph
        public int vertexIndex = 0;
        public DistributedSceneGraph ownerGraph;

    }
}
