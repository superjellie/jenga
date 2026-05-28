using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga.Sparse {

    // Sparse graph (meaning its adjacency matrix is sparse)
    // - Matrix is type of underlying adjacency matrix
    // - Vertex is type of label attached to vertex
    // - Vertices has stable indices
    // Graph is basicaly a matrix with label attached to each row/column
    // Methods on graph are extension method that available whenever
    // underlying matrix has some properties
    [System.Serializable]
    public class SparseGraph<Matrix, Vertex> where Matrix : new() {
        public Matrix matrix = new();
        public MagicList<Vertex> vertices = new();
    }

    // Types for some cases 
    // - Directed: Matrix is not symmetric
    // - Not directed: Matrix is symmetric
    // - Adjacency: Matrix is over booleans (no labels on edges)
    // - Not adjacency: has labels on edges
    // - LIL: Matrix using LIL (LIst of Lists) sparse format 
    [System.Serializable]
    public class LILDirectedAdjacencyGraph<Vertex> 
    : SparseGraph<LILAdjacencyMatrix, Vertex> { }

    [System.Serializable]
    public class LILDirectedGraph<Vertex, Edge> 
    : SparseGraph<LILMatrix<Edge>, Vertex> { }

    [System.Serializable]
    public class LILAdjacencyGraph<Vertex> 
    : SparseGraph<LILSymAdjacencyMatrix, Vertex> { }

    [System.Serializable]
    public class LILGraph<Vertex, Edge> 
    : SparseGraph<LILSymMatrix<Edge>, Vertex> { }

    [System.Serializable]
    public class LILDirectedMultiEdgeGraph<Vertex, Edge> 
    : SparseGraph<LILMatrix<MultiEdge<Edge>>, Vertex> { }

    [System.Serializable]
    public class LILMultiEdgeGraph<Vertex, Edge> 
    : SparseGraph<LILSymMatrix<MultiEdge<Edge>>, Vertex> { }



    // General graph manipulation methods
    public static class SparseGraphExtensions {

        public static bool TryGetVertex<M, V>
        (this SparseGraph<M, V> graph, int vertexIndex, out V vertex) 
        where M : new() {
            if (graph.vertices.HasItemAt(vertexIndex)) { 
                vertex = graph.vertices[vertexIndex]; 
                return true; 
            }
            vertex = default(V);
            return false;
        }

        public static bool TrySetVertex<M, V, E>
        (this SparseGraph<M, V> graph, int vertexIndex, V vertex) 
        where M : new() {
            if (graph.vertices.HasItemAt(vertexIndex)) { 
                graph.vertices[vertexIndex] = vertex; 
                return true; 
            }
            return false;
        }

        public static int 
        AddVertex<M, V>(this SparseGraph<M, V> graph, V vertex)
        where M : new() {
            return graph.vertices.Add(vertex);
        }
        public static bool 
        HasVertex<M, V>(this SparseGraph<M, V> graph, int index)
        where M : new() {
            return graph.vertices.HasItemAt(index);
        }

        public static void 
        RemoveVertex<M, V>(this SparseGraph<M, V> graph, int index)
        where M : new() {
            graph.vertices.RemoveAt(index);
        }

        public static void 
        SetEdge<M, V, E>(this SparseGraph<M, V> graph, int i, int j, E edge) 
        where M : IRandomWritableMatrix<E>, new() 
            { graph.matrix[i, j] = edge; }
        public static void 
        RemoveEdge<M, V>(this SparseGraph<M, V> graph, int i, int j) 
        where M : IRandomClearableMatrix, new() 
            { graph.matrix.RemoveValueAt(i, j); }
        
        public static E 
        GetEdge<M, V, E>(this SparseGraph<M, V> graph, int i, int j) 
        where M : IRandomReadableMatrix<E>, new() { return graph.matrix[i, j]; }

        public static bool 
        HasEdge<M, V>(this SparseGraph<M, V> graph, int i, int j) 
        where M : IRandomTestableMatrix, new() { 
            if (!graph.HasVertex(i) || !graph.HasVertex(j)) return false;
            return graph.matrix.HasValueAt(i, j); 
        }

        public static bool TryGetEdge<M, V, E>
        (this SparseGraph<M, V> graph, int i, int j, out E edge) 
        where M : IRandomReadableMatrix<E>, new() {
            if (graph.HasEdge(i, j)) {
                edge = graph.GetEdge<M, V, E>(i, j);
                return true;
            }

            edge = default(E);
            return false;
        }

        public static IEnumerable<IndexedValue<E>> 
        GetNeighbours<M, V, E>(this SparseGraph<M, V> graph, int i) 
        where M : IRowIterableMatrix<E>, new() { 
            return graph.matrix.GetRow(i); 
        }

        public static IEnumerable<int> 
        GetNeighbourIndices<M, V, E>(this SparseGraph<M, V> graph, int i) 
        where M : IRowIterableMatrix<E>, new() { 
            return graph.matrix.GetRowIndices(i); 
        }
        
    }

    // Multi-edges
    // Have stable indices for edges
    [System.Serializable]
    public class MultiEdge<T> { 
        [SerializeField] MagicList<T> edges = new();
        
        public T this[int i] { 
            get => edges[i]; 
            set => edges[i] = value;
        }

        public int Count() => edges.Count();
        public bool HasEdgeAt(int id) => edges.HasItemAt(id);
        public bool TryGetEdge(int id, out T edge) 
            => edges.TryGetValue(id, out edge);
        public int Add(T edge) => edges.Add(edge);
        public void RemoveAt(int id) => edges.RemoveAt(id);
        public IEnumerable<T> GetEdges() => edges;
    }

}
