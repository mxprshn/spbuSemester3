using System;

namespace LazyThreads
{
    public static class LazyFactory
    {
        public static ILazy<T> CreateLazy<T>(Func<T> supplier)
        {
            return new Lazy<T>(supplier);
        }

        public static ILazy<T> CreateThreadSafeLazy<T>(Func<T> supplier)
        {
            return new ThreadSafeLazy<T>(supplier);
        }
    }
}