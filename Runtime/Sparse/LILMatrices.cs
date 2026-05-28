using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga.Sparse {

    // LIst of Lists format for few types of matrices
    // LIL format has following properties:
    // - O(log(length of row)) read
    // - O(length of row) write
    // - O(length of row) iteration over row
    // - O(n * log(length of biggest row)) iteration over column


    // Adjacency is matrix over booleans (only presense of value matters)
    [System.Serializable]
    public partial class LILAdjacencyMatrix {
        [SerializeField] List<IndexVector> rows = new();
    }


    public partial class LILAdjacencyMatrix 
    : IRandomReadableMatrix<bool>, IRandomWritableMatrix<bool> {
        public virtual bool this[int i, int j] {
            get => i < rows.Count && rows[i][j];
            set {
                if (!value) return;
                rows.EnsureCount(i + 1, new IndexVector());
                rows[i][j] = value;                 
            }
        }
    }

    public partial class LILAdjacencyMatrix : IRowIterableMatrix<bool> {
        public IEnumerable<int> GetRowIndices(int rowIndex) 
            => rows != null && rowIndex < rows.Count 
                ? rows[rowIndex] : Iterators.Empty<int>();
        public IEnumerable<IndexedValue<bool>> GetRow(int rowIndex) {
            foreach (var index in GetRowIndices(rowIndex))
                yield return (index, true);
        }
    }

    public partial class LILAdjacencyMatrix : IColumnIterableMatrix<bool> {
        public IEnumerable<int> GetColumnIndices(int columnIndex) {
            for (int rowIndex = 0; rowIndex < rows.Count; ++rowIndex) {
                if (!rows[rowIndex][columnIndex]) continue;
                yield return rowIndex;
            }
        }

        public IEnumerable<IndexedValue<bool>> GetColumn(int columnIndex) {
            foreach (var index in GetColumnIndices(columnIndex))
                yield return (index, true);
        }
    }

    // Regular LIL Matrix over generic type
    [System.Serializable]
    public partial class LILMatrix<T> {
        [SerializeField] List<SparseVector<T>> rows = new();
    }


    public partial class LILMatrix<T> 
    : IRandomReadableMatrix<T>, IRandomWritableMatrix<T> {
        public virtual T this[int i, int j] {
            get => i < rows.Count ? rows[i][j] : default(T);
            set {
                // if (value) return;
                rows.EnsureCount(i + 1, new SparseVector<T>());
                rows[i][j] = value;                 
            }
        }
    }

    public partial class LILMatrix<T> : IRowIterableMatrix<T> {
        public IEnumerable<int> GetRowIndices(int rowIndex) 
            => rows != null && rowIndex < rows.Count 
                ? rows[rowIndex].indices : Iterators.Empty<int>();
        public IEnumerable<IndexedValue<T>> GetRow(int rowIndex) 
            => rows != null && rowIndex < rows.Count 
                ? rows[rowIndex].GetIndexedValues() 
                : Iterators.Empty<IndexedValue<T>>();
    }

    public partial class LILMatrix<T> : IColumnIterableMatrix<T> {
        public IEnumerable<int> GetColumnIndices(int columnIndex) {
            for (int rowIndex = 0; rowIndex < rows.Count; ++rowIndex) {
                if (object.Equals(rows[rowIndex][columnIndex], default(T))) 
                    continue;
                yield return rowIndex;
            }
        }

        public IEnumerable<IndexedValue<T>> GetColumn(int columnIndex) {
            for (int rowIndex = 0; rowIndex < rows.Count; ++rowIndex) {
                var value = rows[rowIndex][columnIndex];
                if (object.Equals(value, default(T))) 
                    continue;
                yield return (rowIndex, value);
            }
        }
    }


    // Matrix with symmetric storage type
    // Any matrix can be symmetric, but this one is always symmetric
    // Writing into symmetric matrix, writes to both [i, j] and [j, i]
    [System.Serializable]
    public partial class LILSymMatrix<T> : LILMatrix<T> {
        public override T this[int i, int j] {
            get => base[i, j];
            set { base[i, j] = value; base[j, i] = value; }
        }
    }

    [System.Serializable]
    public partial class LILSymAdjacencyMatrix : LILAdjacencyMatrix {
        public override bool this[int i, int j] {
            get => base[i, j];
            set { base[i, j] = value; base[j, i] = value; }
        }
    }


}
