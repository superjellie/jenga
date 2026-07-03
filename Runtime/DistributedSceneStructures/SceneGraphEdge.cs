using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {
    public class SceneGraphEdge : MonoBehaviour {

        // Index in DistributedSceneGraph
        // Should be stable during lifetime of edge
        // These values controlled by ownerGraph
        [ReadOnly] public Vector2Int edgeIndex = Vector2Int.zero;
        [ReadOnly] public int edgeId = 0;
        [ReadOnly] public DistributedSceneGraph ownerGraph;


        //        
        public bool TryGetStartVertex(out SceneGraphVertex vertex) {
            vertex = null;
            return ownerGraph != null 
                ? ownerGraph.TryGetVertex(edgeIndex.x, out vertex)
                : false;
        }

        public bool TryGetEndVertex(out SceneGraphVertex vertex) {
            vertex = null;
            return ownerGraph != null  
                ? ownerGraph.TryGetVertex(edgeIndex.y, out vertex)
                : false;
        }
        public Vector3 GetStartPosition() 
            => ownerGraph.TryGetVertex(edgeIndex.x, out var vertex) 
                ? vertex.transform.position 
                : Vector3.zero;
        public Vector3 GetEndPosition() 
            => ownerGraph.TryGetVertex(edgeIndex.y, out var vertex) 
                ? vertex.transform.position 
                : Vector3.forward;
    }
}
