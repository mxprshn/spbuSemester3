using System;

namespace LazyThreads
{
    public class Lazy<T> : ILazy<T>
    {
        private bool isInitial = true;
        private Func<T> supplier;
        private T value;

        public Lazy(Func<T> supplier) => this.supplier = supplier;

        public T Get()
        {
            if (isInitial)
            {
                value = supplier();
                isInitial = false;
            }

            return value;
        }
    }
}