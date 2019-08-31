using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Ryne.Utility.Collections
{
    [DebuggerDisplay("Length = {Length}")]
    public class DynamicArray<T> : IEnumerable<T>, IEquatable<DynamicArray<T>>
    {
        internal T[] Elements;

        public int Length { get; private set; }

        private int Allocated => Elements.Length;

        public DynamicArray(int size = 0, int allocated = 1)
        {
            allocated = System.Math.Max(size, allocated);

            Length = size;
            Elements = new T[allocated];
        }

        public ref T this[int index] => ref Elements[index];

        public void Resize(int maxSize)
        {
            if (Allocated <= maxSize)
            {
                var newSize = System.Math.Max(Length * 2, maxSize);
                Array.Resize(ref Elements, newSize);
            }
        }

        public void SetSize(int size)
        {
            if (Allocated <= size)
            {
                Resize(size);
            }

            Length = size;
        }

        public void Add(T item)
        {
            Resize(Length + 1);
            Elements[Length++] = item;
        }

        public T[] ToArray()
        {
            var array = new T[Length];
            Copy(this, 0, array, 0, Length);
            return array;
        }

        public void Clear()
        {
            Elements = new T[0];
            Length = 0;
        }

        public void RemoveLast()
        {
            if (Length > 0)
            {
                Length--;
            }
        }


        public bool Equals(DynamicArray<T> other)
        {
            if (other == null)
            {
                return false;
            }

            return Length == other.Length && Elements == other.Elements;
        }


        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public struct Enumerator : IEnumerator<T>
        {
            readonly DynamicArray<T> Array;
            readonly int Length;
            int Index;

            public Enumerator(DynamicArray<T> array)
            {
                Array = array;
                Length = array.Length;
                Index = -1;
            }

            public void Dispose()
            {
            }

            public bool MoveNext()
            {
                Index++;
                return Index < Length;
            }

            public void Reset()
            {
                Index = -1;
            }

            public T Current => Array[Index];

            object IEnumerator.Current => Current;
        }


        public void CopyTo(DynamicArray<T> dst)
        {
            dst.SetSize(Length);
            Copy(this, dst);
        }

        public static void Copy(DynamicArray<T> src, DynamicArray<T> dst)
        {
            if (src.Length != dst.Length)
            {
                throw new ArgumentException("source and destination length must be the same");
            }

            Copy(src, 0, dst, 0, src.Length);
        }

        public static void Copy(DynamicArray<T> src, int srcIndex, DynamicArray<T> dst, int dstIndex, int length)
        {
            System.Array.ConstrainedCopy(src.Elements, srcIndex, dst.Elements, dstIndex, length);
        }

        public static void Copy(T[] src, int srcIndex, DynamicArray<T> dst, int dstIndex, int length)
        {
            System.Array.ConstrainedCopy(src, srcIndex, dst.Elements, dstIndex, length);
        }

        public static void Copy(DynamicArray<T> src, int srcIndex, T[] dst, int dstIndex, int length)
        {
            System.Array.ConstrainedCopy(src.Elements, srcIndex, dst, dstIndex, length);
        }
    }
}
