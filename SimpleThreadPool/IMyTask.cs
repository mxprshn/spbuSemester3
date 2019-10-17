using System;

namespace SimpleThreadPool
{
    /// <summary>
    /// Interface representing a task queued in the thread pool.
    /// </summary>
    /// <typeparam name="TResult">Task result type.</typeparam>
    public interface IMyTask<TResult>
    {
        /// <summary>
        /// Determines if the task's result is already calculated.
        /// </summary>
        bool IsCompleted { get; }

        /// <summary>
        /// Determines if the task was cancelled.
        /// </summary>
        bool IsCancelled { get; }

        /// <summary>
        /// Returns result of the task. If it has not been calculated, the current thread waits for it.
        /// </summary>
        TResult Result { get; }

        /// <summary>
        /// Creates a new task which is applied to the result of another one.
        /// </summary>
        /// <typeparam name="TNewResult">New task result type.</typeparam>
        /// <param name="newSupplier">Function to be encapsulated into the new task.</param>
        /// <returns>Created task.</returns>
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newSupplier);
    }
}