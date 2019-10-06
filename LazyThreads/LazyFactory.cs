using System;

namespace LazyThreads
{
    /// <summary>
    /// Class implementing abstract factory pattern for creating ILazy objects.
    /// </summary>
    public static class LazyFactory
    {
        /// <summary>
        /// Creates a new instance of ILazy object.
        /// </summary>
        /// <typeparam name="T">Type of value encapsulated by ILazy object.</typeparam>
        /// <param name="supplier">Function returning value which is encapsulated by ILazy object.</param>
        /// <returns>A new instance of ILazy object.</returns>
        public static ILazy<T> CreateLazy<T>(Func<T> supplier)
        {
            return new Lazy<T>(supplier);
        }

        /// <summary>
        /// Creates a new instance of thread-safe ILazy object.
        /// </summary>
        /// <typeparam name="T">Type of value encapsulated by ILazy object.</typeparam>
        /// <param name="supplier">Function returning value which is encapsulated by ILazy object.</param>
        /// <returns>A new instance of ILazy object.</returns>
        public static ILazy<T> CreateThreadSafeLazy<T>(Func<T> supplier)
        {
            return new ThreadSafeLazy<T>(supplier);
        }
    }
}