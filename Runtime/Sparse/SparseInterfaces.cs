using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga.Sparse {

    [System.Serializable]
    public struct IndexedValue<T> {
        public int index;
        public T value;

        public IndexedValue(int index, T value) 
            { this.index = index; this.value = value; }
        public static implicit operator T(IndexedValue<T> x) => x.value;
        public static implicit operator IndexedValue<T>((int, T) pair) 
            => new(pair.Item1, pair.Item2);
        public void Deconstruct(out int index, out T value) 
            { index = this.index; value = this.value; }
    }

    [System.Serializable]
    public struct Indexed2Value<T> {
        public Vector2Int index;
        public T value;

        public Indexed2Value(int i, int j, T value) 
            { this.index = new(i, j); this.value = value; }
        public static implicit operator T(Indexed2Value<T> x) => x.value;
        public static implicit operator Indexed2Value<T>((int, int, T) pair) 
            => new(pair.Item1, pair.Item2, pair.Item3);
        public static implicit operator Indexed2Value<T>((Vector2Int, T) pair) 
            => new(pair.Item1.x, pair.Item1.x, pair.Item2);
        public void Deconstruct(out Vector2Int index, out T value) 
            { index = this.index; value = this.value; }
        public void Deconstruct(out int i, out int j, out T value) 
            { i = index.x; j = index.y; value = this.value; }
    }

    public interface IValueIterableMatrix<T> {
        IEnumerable<Indexed2Value<T>> GetValues();
        IEnumerable<Vector2Int> GetValueIndices() {
            foreach (var (v, x) in GetValues())
                yield return v;
        }
    }

    public interface IRowIterableMatrix<T> : IValueIterableMatrix<T> {
        IEnumerable<IndexedValue<T>> GetRow(int rowIndex);
        IEnumerable<int> GetRowIndices(int rowIndex) {
            foreach (var (i, x) in GetRow(rowIndex))
                yield return i;
        }
        IEnumerable<int> GetNonemptyRows();

        IEnumerable<Indexed2Value<T>> IValueIterableMatrix<T>.GetValues() {
            foreach (var i in GetNonemptyRows())
            foreach (var (j, x) in GetRow(i))
                yield return (i, j, x);
        }
    }

    public interface IColumnIterableMatrix<T> : IValueIterableMatrix<T> {
        IEnumerable<IndexedValue<T>> GetColumn(int columnIndex);
        IEnumerable<int> GetColumnIndices(int columnIndex) {
            foreach (var (i, x) in GetColumn(columnIndex))
                yield return i;
        }
        IEnumerable<int> GetNonemptyColumns();

        IEnumerable<Indexed2Value<T>> IValueIterableMatrix<T>.GetValues() {
            foreach (var j in GetNonemptyColumns())
            foreach (var (i, x) in GetColumn(j))
                yield return (i, j, x);
        }
    }

    public interface IRandomTestableMatrix {
        bool HasValueAt(int i, int k);
    }

    public interface IRandomReadableMatrix<T> : IRandomTestableMatrix {
        T this[int i, int k] { get; }
        bool IRandomTestableMatrix.HasValueAt(int i, int k) 
            => !object.Equals(this[i, k], default(T));
    }

    public interface IRandomClearableMatrix {
        void RemoveValueAt(int i, int k); // => this[i, k] = default(T);
    }

    public interface IRandomWritableMatrix<T> : IRandomClearableMatrix {
        T this[int i, int k] { set; }
        void IRandomClearableMatrix.RemoveValueAt(int i, int k) 
            => this[i, k] = default(T);
    }

    public interface IRandomReadableVector<T> {
        T this[int i] { get; }
        bool HasValueAt(int i) => !object.Equals(this[i], default(T));
    }

    public interface IRandomWritableVector<T> {
        T this[int i] { set; }
    }

    public interface IIterableIndexVector : IEnumerable<int> { 
        IEnumerable<int> GetIndices();
        IEnumerator<int> IEnumerable<int>.GetEnumerator() 
            => GetIndices().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() 
            => GetIndices().GetEnumerator();
    }

    public interface IIterableVector<T> : IEnumerable<IndexedValue<T>> { 
        IEnumerable<IndexedValue<T>> GetIndexedValues();
        IEnumerator<IndexedValue<T>> 
        IEnumerable<IndexedValue<T>>.GetEnumerator() 
            => GetIndexedValues().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() 
            => GetIndexedValues().GetEnumerator();
    }

    public static class SparseMehods {
        public static SparseVector<float> MatMul<M>(this M m, SparseVector<float> v)
        where M : IRowIterableMatrix<float> {
            var r = new SparseVector<float>();
            foreach (var row in m.GetNonemptyRows())
            foreach (var (column, value) in m.GetRow(row))
                r[row] = r[row] + value * v[column];
            return r;
        }

        public static SparseVector<float> 
        Add(this SparseVector<float> v, SparseVector<float> u) {
            var r = new SparseVector<float>();
            foreach (var (i, vi) in v.GetIndexedValues())
                r[i] = vi;
            foreach (var (j, uj) in u.GetIndexedValues())
                r[j] = r[j] + uj;
            return r;
        }

        public static SparseVector<float> 
        Scale(this SparseVector<float> v, float scalar) {
            var r = new SparseVector<float>();
            foreach (var (i, vi) in v.GetIndexedValues())
                r[i] = vi * scalar;
            return r;
        }
    }

}
