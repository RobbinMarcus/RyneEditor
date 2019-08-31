using System;
using System.Collections.Concurrent;
using System.Threading;

namespace Ryne.Utility.Collections
{
    /// <summary>
    /// Array that will loop around when adding values over the maximum size
    /// </summary>
    public class CircularArray<T>
    {
        private readonly ConcurrentQueue<T> Data;
        private readonly ReaderWriterLockSlim Lock = new ReaderWriterLockSlim();
        private readonly int Size;

        public CircularArray(int size)
        {
            if (size < 1)
            {
                throw new ArgumentException($"{nameof(size)} cannot be negative or zero");
            }
            Data = new ConcurrentQueue<T>();
            Size = size;
        }

        public T[] ToArray()
        {
            return Data.ToArray();
        }

        public void Add(T t)
        {
            Lock.EnterWriteLock();
            try
            {
                if (Data.Count == Size)
                {
                    Data.TryDequeue(out _);
                }

                Data.Enqueue(t);
            }
            finally
            {
                Lock.ExitWriteLock();
            }
        }
    }
}
