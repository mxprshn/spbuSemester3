using System;
using System.Threading;

namespace LazyThreads
{
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        private bool isInitial = true;
        private Func<T> supplier;
        private Object locker = new Object();
        private T value;

        public ThreadSafeLazy(Func<T> supplier) => this.supplier = supplier;

        public T Get()
        {
            if (!isInitial)
            {
                return value;
            }

            lock (locker)
            {
                if (isInitial)
                {
                    value = supplier();
                    isInitial = false;
                }
            }

            return value;
        }
    }
}
