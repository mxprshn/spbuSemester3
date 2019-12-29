using System;

namespace SimpleThreadPool
{
    /// <summary>
    /// Interface representing thread pool.
    /// </summary>
    public interface IMyThreadPool
    {
        /// <summary>
        /// Amount of active threads in the pool.
        /// </summary>
        int ActiveThreadCount { get; }

        /// <summary>
        /// Creates a new task which result is evaluated by the thread pool.
        /// </summary>
        /// <typeparam name="TResult">Task result type.</typeparam>
        /// <param name="supplier">Function to be encapsulated into the new task.</param>
        /// <returns>Created task.</returns>
        IMyTask<TResult> QueueTask<TResult>(Func<TResult> supplier);

        /// <summary>
        /// Stops the thread pool work.
        /// </summary>
        void Shutdown();
    }
}