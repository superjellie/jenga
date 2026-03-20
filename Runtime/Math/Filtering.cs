using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jenga {

    public class MovingAverageFilter<T> {
        T[] buffer;
        int currentPosition;

        public MovingAverageFilter(int bufferSize) {
            buffer = new T[bufferSize];
            currentPosition = 0;
        }

        public void Init(T value) {
            for (int i = 0; i < buffer.Length; ++i)
                buffer[i] = value;
            currentPosition = 0;
        }

        public T Next(T signal) {
            currentPosition = (currentPosition + 1) % buffer.Length;
            buffer[currentPosition] = signal;
            var sum = LinAlgf.Add(buffer);
            return LinAlgf.RScale(sum, (float)buffer.Length);
        }

        public T Current() => buffer[currentPosition];
    }

    public class MovingOffsetFilter<T> {
        T[] buffer;
        int currentPosition;

        public MovingOffsetFilter(int bufferSize) {
            buffer = new T[bufferSize];
            currentPosition = 0;
        }

        public void Init(T value) {
            for (int i = 0; i < buffer.Length; ++i)
                buffer[i] = value;
            currentPosition = 0;
        }

        public T Next(T signal) {
            currentPosition = (currentPosition + 1) % buffer.Length;
            var oldSignal = buffer[currentPosition];
            buffer[currentPosition] = signal;
            return oldSignal;


        }

        public T Current() => buffer[currentPosition];
    }


}
