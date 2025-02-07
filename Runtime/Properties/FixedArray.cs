using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

	[System.Serializable]
	public struct FixedArray<T> {
		[SerializeField] T[] array;

		public ArrayView<T> view     => array;
		public ArrayView<T> viewCopy => view.Copy();
		public int          length   => view.length;

		public T this[int x] => array[x];

		public FixedArray(int size) { array = new T[size]; }
	} 

}