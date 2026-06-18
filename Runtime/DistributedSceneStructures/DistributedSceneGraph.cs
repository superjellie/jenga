using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jenga.Sparse;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Jenga {
    public class DistributedSceneGraph : MonoBehaviour {

        [SerializeField]
        LILDirectedMultiEdgeGraph<SceneGraphVertex, SceneGraphEdge>
            graph = new();

        // These methods can be used both in editor with correct Undo
        // and in runtime
        public bool TryGetVertex(int index, out SceneGraphVertex vertex) 
            => graph.TryGetVertex(index, out vertex);
        public bool TryGetEdge(int i, int j, int id, out SceneGraphEdge edge) {
            if (graph.TryGetEdge(i, j, out MultiEdge<SceneGraphEdge> me)) {
                return me.TryGetEdge(id, out edge);
            }
            edge = null;
            return false;
        }
        public bool TryGetMultiEdge(int i, int j, out MultiEdge<SceneGraphEdge> edge) 
            => graph.TryGetEdge(i, j, out edge);
        public bool HasVertex(int index) 
            => graph.HasVertex(index);
        public bool HasEdge(int i, int j, int id)
            => graph.TryGetEdge(i, j, out MultiEdge<SceneGraphEdge> multiEdge) 
            && multiEdge.HasEdgeAt(id);
        public bool HasAnyEdge(int i, int j) 
            => graph.HasEdge(i, j);

        public int AddVertex(SceneGraphVertex vertex) {
            if (vertex.ownerGraph != null) 
                vertex.ownerGraph.RemoveVertex(vertex.vertexIndex);   
        #if UNITY_EDITOR
            if (!Application.isPlaying) {
                var msg = $"Add vertex {vertex} to graph {name}";
                Undo.RecordObject(this, msg);
                Undo.RecordObject(vertex, msg);
            }
        #endif
            var index = graph.AddVertex(vertex);         
            vertex.vertexIndex = index;
            vertex.ownerGraph = this;
            return index;
        }

        public void RemoveVertex(int index) { 
            if (TryGetVertex(index, out var vertex)) {
            #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    var msg = $"Remove vertex {vertex} from graph {name}";
                    Undo.RecordObject(this, msg);
                    Undo.RecordObject(vertex, msg);
                }
            #endif
                vertex.vertexIndex = 0;
                vertex.ownerGraph = null;
                graph.RemoveVertex(index);
            } 
        }

        
        public void RemoveEdge(int i, int j, int id) {
            if (TryGetEdge(i, j, id, out var edge)) {
            #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    var msg = $"Remove edge {edge} from graph {name}";
                    Undo.RecordObject(this, msg);
                    Undo.RecordObject(edge, msg);
                }
            #endif
                edge.edgeIndex = Vector2Int.zero;
                edge.edgeId = 0;
                edge.ownerGraph = null;

                if (graph.TryGetEdge(i, j, out MultiEdge<SceneGraphEdge> me)) {
                    me.RemoveAt(id);
                    if (me.Count() == 0)
                        graph.RemoveEdge(i, j);
                }
            }
        }

        public void RemoveEdge(SceneGraphEdge edge) {
            RemoveEdge(edge.edgeIndex.x, edge.edgeIndex.y, edge.edgeId);
        }

        public void RemoveMultiEdge(int i, int j) { 
            if (TryGetMultiEdge(i, j, out var multiEdge)) {
            #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    var msg = $"Remove edge ({i}, {j}) from graph {name}";
                    Undo.RecordObject(this, msg);
                }
            #endif

                foreach (var edge in multiEdge.GetEdges()) {
                #if UNITY_EDITOR
                    if (!Application.isPlaying) {
                        var msg = $"Remove edge {edge} from graph {name}";
                        Undo.RecordObject(edge, msg);
                    }
                #endif
                    edge.edgeIndex = Vector2Int.zero;
                    edge.edgeId = 0;
                    edge.ownerGraph = null;
                }
                
                graph.RemoveEdge(i, j);
            }
        }

        public void SetEdge(int i, int j, int id, SceneGraphEdge edge) { 
            if (edge.ownerGraph != null) 
                edge.ownerGraph
                    .RemoveEdge(edge.edgeIndex.x, edge.edgeIndex.y, edge.edgeId);
            #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    var msg = $"Set edge ({i}, {j}):{id} on graph {name} to {edge}";
                    Undo.RecordObject(this, msg);
                    Undo.RecordObject(edge, msg);
                }
            #endif
            RemoveEdge(i, j, id);
            edge.edgeIndex = new(i, j);
            edge.edgeId = id;
            edge.ownerGraph = this;

            if (graph.TryGetEdge(i, j, out MultiEdge<SceneGraphEdge> me))
                me[id] = edge;
        }

        public int AddEdge(int i, int j, SceneGraphEdge edge) { 
            if (edge.ownerGraph != null) 
                edge.ownerGraph
                    .RemoveEdge(edge.edgeIndex.x, edge.edgeIndex.y, edge.edgeId);
            #if UNITY_EDITOR
                if (!Application.isPlaying) {
                    var msg = $"Add edge {edge} on graph {name}";
                    Undo.RecordObject(this, msg);
                    Undo.RecordObject(edge, msg);
                }
            #endif
            edge.ownerGraph = this;
            edge.edgeIndex = new(i, j);

            if (!graph.TryGetEdge(i, j, out MultiEdge<SceneGraphEdge> me)) {
                me = new();
                graph.SetEdge(i, j, me);
            }    
            edge.edgeId = me.Add(edge);
            return edge.edgeId;
        }


        public IEnumerable<SceneGraphEdge> GetIncidentEdges(int vertex) {
            foreach (var (i, me) in graph.GetIncidentEdges<
                LILMatrix<MultiEdge<SceneGraphEdge>>,
                SceneGraphVertex, MultiEdge<SceneGraphEdge>
            >(vertex))
            foreach (var e in me.GetEdges())
                yield return e;
        }

        public IEnumerable<SceneGraphEdge> 
        GetIncidentEdges(SceneGraphVertex vertex) 
            => GetIncidentEdges(vertex.vertexIndex);
    }
}
