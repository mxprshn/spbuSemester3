using System;

namespace LazyThreads
{
    /// <summary>
    /// ILazy implementation (thread-safety not guaranteed).
    /// </summary>
    /// <typeparam name="T">Type of encapsulated value.</typeparam>
    public class Lazy<T> : ILazy<T>
    {
        private bool isEvaluated = false;
        private readonly Func<T> supplier;
        private T value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="supplier">Function returning value which is encapsulated by the object.</param>
        public Lazy(Func<T> supplier) => this.supplier = supplier;

        /// <summary>
        /// Evaluates encapsulated expression if it has not been done before and returns it.
        /// </summary>
        /// <returns>Value of the expression.</returns>
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