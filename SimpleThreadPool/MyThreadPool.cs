using System;
using System.Threading;
using System.Collections.Generic;

namespace SimpleThreadPool
{
    /// <summary>
    /// Class implementing thread pool interface.
    /// </summary>
    public class MyThreadPool : IMyThreadPool
    {
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private Queue<Action> continuations = new Queue<Action>();
            private MyThreadPool threadPool;
            private Func<TResult> supplier;
            private TResult result;
            private AggregateException aggregateException;
            private ManualResetEvent isResultReadyEvent = new ManualResetEvent(false);
            private Object continuationQueueLocker = new Object();

            public bool IsCancelled { get; private set; } = false;
            public bool IsCompleted { get; private set; } = false;

            /// <summary>
            /// Returns result of the task. If it has not been calculated, the current thread waits for it.
            /// </summary>
            /// <exception cref="ThreadPoolShutdownException">Thrown if the thread pool was shutdown and result was not evaluated.</exception>
            /// <exception cref="AggregateException">Thrown if another exception was thrown during task evaluation.</exception>
            public TResult Result
            {
                get
                {
                    isResultReadyEvent.WaitOne();

                    if (IsCancelled)
                    {
                        throw new ThreadPoolShutdownException();
                    }

                    if (aggregateException != null)
                    {
                        throw aggregateException;
                    }

                    return result;
                }

                private set
                {
                    result = value;
                }
            }

            public Action TaskStarter => () => 
            {
                try
                {
                    if (threadPool.cancellationTokenSource.IsCancellationRequested)
                    {
                        IsCancelled = true;
                        return;
                    }

                    result = supplier();
                    IsCompleted = true;
                    supplier = null;
                }
                catch (Exception exception)
                {
                    aggregateException = new AggregateException(exception);
                }
                finally
                {
                    isResultReadyEvent.Set();

                    lock (continuationQueueLocker)
                    {
                        while (continuations.Count != 0)
                        {
                            lock (threadPool.actionQueueLocker)
                            {
                                threadPool.actions.Enqueue(continuations.Dequeue());
                                Monitor.Pulse(threadPool.actionQueueLocker);                         
                            }
                        }
                    }
                }
            };

            public MyTask(Func<TResult> supplier, MyThreadPool threadPool)
            {
                this.threadPool = threadPool;
                this.supplier = supplier;
            }

            /// <summary>
            /// Creates a new task which is applied to the result of another one.
            /// </summary>
            /// <typeparam name="TNewResult">New task result type.</typeparam>
            /// <param name="newSupplier">Function to be encapsulated into the new task.</param>
            /// <returns>Created task.</returns>
            /// <exception cref = "ThreadPoolShutdownException" > Thrown if the thread pool was shutdown.</exception>
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newSupplier)
            {
                if (threadPool.cancellationTokenSource.IsCancellationRequested)
                {
                    throw new ThreadPoolShutdownException();
                }

                var task = new MyTask<TNewResult>(() => newSupplier(Result), threadPool);

                lock (continuationQueueLocker)
                {
                    if (IsCompleted)
                    {
                        threadPool.EnqueueAction(task.TaskStarter);
                    }
                    else
                    {
                        continuations.Enqueue(task.TaskStarter);
                    }
                }

                return task;
            }
        }

        private Thread[] threads;
        private Queue<Action> actions = new Queue<Action>();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private Object actionQueueLocker = new Object();

        public int ActiveThreadCount { get; private set; }

        public MyThreadPool(int threadCount)
        {
            if (threadCount <= 0)
            {
                throw new ArgumentOutOfRangeException("Not positive amount of threads in MyThreadPool constructor.");
            }

            threads = new Thread[threadCount]; 

            for (var i = 0; i < threadCount; ++i)
            {
                threads[i] = new Thread(DoTasks);
                threads[i].Start();
            }

            ActiveThreadCount = threadCount;
        }

        private void DoTasks()
        {
            while (true)
            {
                Action nextAction = null;

                lock (actionQueueLocker)
                {
                    while (actions.Count == 0)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            --ActiveThreadCount;
                            return;
                        }

                        Monitor.Wait(actionQueueLocker);
                    }

                    nextAction = actions.Dequeue();
                }

                nextAction.Invoke();
            }
        }

        private void EnqueueAction(Action action)
        {
            lock (actionQueueLocker)
            {
                if (cancellationTokenSource.IsCancellationRequested)
                {
                    throw new ThreadPoolShutdownException();
                }

                actions.Enqueue(action);
                Monitor.Pulse(actionQueueLocker);
            }
        }

        /// <summary>
        /// Creates a new task which result is evaluated by the thread pool.
        /// </summary>
        /// <typeparam name="TResult">Task result type.</typeparam>
        /// <param name="supplier">Function to be encapsulated into the new task.</param>
        /// <returns>Created task.</returns>
        /// <exception cref = "ThreadPoolShutdownException" > Thrown if the thread pool was shutdown.</exception>
        public IMyTask<TResult> QueueTask<TResult>(Func<TResult> supplier)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolShutdownException();
            }

            var task = new MyTask<TResult>(supplier, this);
            EnqueueAction(task.TaskStarter);
            return task;
        }

        /// <summary>
        /// Stops the thread pool work. If there are queued tasks left, they throw ThreadPoolShutdownException to the waiting threads.
        /// </summary>
        /// <exception cref = "ThreadPoolShutdownException" > Thrown if the thread pool was already shutdown.</exception>
        public void Shutdown()
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolShutdownException();
            }

            cancellationTokenSource.Cancel();

            lock (actionQueueLocker)
            {
                Monitor.PulseAll(actionQueueLocker);
            }
        }
    }
}