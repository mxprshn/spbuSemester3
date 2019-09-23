using System;
using System.Threading;

namespace LazyThreads
{
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        private bool isEvaluated = false;
        private readonly Func<T> supplier;
        private Object locker = new Object();
        private T value;

        public ThreadSafeLazy(Func<T> supplier) => this.supplier = supplier;

        public T Get()
        {
            if (Volatile.Read(ref isEvaluated))
            {
                return value;
            }

            lock (locker)
            {
                if (!isEvaluated)
                {
                    value = supplier();
                    Volatile.Write(ref isEvaluated, true);
                }
            }

            return value;
        }
    }
}