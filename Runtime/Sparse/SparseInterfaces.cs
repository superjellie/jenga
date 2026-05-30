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


    public interface IRowIterableMatrix<T> {
        IEnumerable<IndexedValue<T>> GetRow(int rowIndex);
        IEnumerable<int> GetRowIndices(int rowIndex) {
            foreach (var (i, x) in GetRow(rowIndex))
                yield return i;
        }
    }

    public interface IColumnIterableMatrix<T> {
        IEnumerable<IndexedValue<T>> GetColumn(int columnIndex);
        IEnumerable<int> GetColumnIndices(int columnIndex) {
            foreach (var (i, x) in GetColumn(columnIndex))
                yield return i;
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

}
