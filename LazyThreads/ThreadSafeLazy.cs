using System;
using System.Threading;

namespace LazyThreads
{
    /// <summary>
    /// Thread-safe ILazy implementation.
    /// </summary>
    /// <typeparam name="T">Type of encapsulated value.</typeparam>
    public class ThreadSafeLazy<T> : ILazy<T>
    {
        private bool isEvaluated = false;
        private readonly Func<T> supplier;
        private Object locker = new Object();
        private T value;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="supplier">Function returning value which is encapsulated by the object.</param>
        public ThreadSafeLazy(Func<T> supplier) => this.supplier = supplier;

        /// <summary>
        /// Evaluates encapsulated expression if it has not been done before and returns it.
        /// </summary>
        /// <returns>Value of the expression.</returns>
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