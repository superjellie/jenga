using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    // No length checks!!!
    public class ArrayView<T> {
        public T[] array;
        public int firstIndex;
        public int length;


        public T[] Copy() {
            var copy = new T[length];
            for (int i = 0; i < length; ++i)
                copy[i] = this[i];
            return copy;
        }

        public void Swap(int i, int j) {
            var x = array[firstIndex + i];
            array[firstIndex + i] = array[firstIndex + j];
            array[firstIndex + j] = x;
        }

        public static implicit operator ArrayView<T>(T[] array) 
            => new ArrayView<T>() {
                array = array,
                firstIndex = 0,
                length = array.Length
            };


        public T this[int i] {
            get => array[i + firstIndex];
            set => array[i + firstIndex] = value;
        }
    }

    public static class ArrayView {
        public static ArrayView<T> Params<T>(params T[] array) => array;
        public static ArrayView<T> Array<T>(T[] array) => array;
        public static ArrayView<T> Slice<T>(
            ArrayView<T> array, int start, int endExclusive
        ) => new ArrayView<T>() {
            array = array.array,
            firstIndex = start + array.firstIndex,
            length = endExclusive - (start + array.firstIndex)
        };
    }

}
