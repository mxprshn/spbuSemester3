using System;

namespace LazyThreads
{
    public class Lazy<T> : ILazy<T>
    {
        private bool isEvaluated = false;
        private readonly Func<T> supplier;
        private T value;

        public Lazy(Func<T> supplier) => this.supplier = supplier;

        public T Get()
        {
            if (!isEvaluated)
            {
                value = supplier();
                isEvaluated = true;
            }

            return value;
        }
    }
}