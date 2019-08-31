//using System;

//namespace Ryne.Utility.Collections
//{
//    ref struct ContiguousArray<T> where T : struct
//    {
//        private Span<T> Elements;

//        public int Length { get; private set; }

//        private int Allocated => Elements.Length;

//        public ContiguousArray(int size = 0)
//        {
//            Length = size;
//            Elements = new Span<T>(new T[size]);
//        }

//        public ref T this[int index] => ref Elements[index];

//        public void Resize(int maxSize)
//        {
//            if (Allocated <= maxSize)
//            {
//                var newSize = System.Math.Size(Length * 2, maxSize);
//                Span<T> newSpan = new Span<T>(new T[newSize]);
//                Elements.CopyTo(newSpan);
//                Elements = newSpan;
//            }
//        }

//        public void Add(T item)
//        {
//            if (Length >= Allocated)
//            {
//                Resize(Length);
//            }

//            Elements[Length++] = item;
//        }
//    }
//}
