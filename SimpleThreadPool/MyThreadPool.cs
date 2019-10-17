using System;
using System.Threading;
using System.Collections.Generic;

// добавили новый таск в очередь -- потоки должны узнать, что можно будет его достать
// деструктор???

namespace SimpleThreadPool
{
    public class MyThreadPool : IMyThreadPool
    {
        private class MyTask<TResult> : IMyTask<TResult>
        {
            private Action<bool> continuation;
            private MyThreadPool threadPool;
            private Func<TResult> supplier;
            private TResult result;
            private AggregateException aggregateException;
            private ManualResetEvent isResultReadyEvent = new ManualResetEvent(false);
            private bool isCancelled = false;
            private Object isCompletedLocker = new Object();

            public bool IsCompleted { get; private set; } = false;

            public MyTask(Func<TResult> supplier, MyThreadPool threadPool)
            {
                this.threadPool = threadPool;
                this.supplier = supplier;
            }

            public TResult Result
            {
                get
                {
                    isResultReadyEvent.WaitOne();

                    if (isCancelled)
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

            public Action<bool> TaskStarter => (isCancelled) => 
            {
                try
                {
                    if (isCancelled)
                    {
                        this.isCancelled = true;
                        return;
                    }

                    result = supplier();
                    IsCompleted = true;
                    supplier = null;

                    lock (isCompletedLocker)
                    {
                        if (continuation != null)
                        {
                            threadPool.EnqueueAction(continuation);
                            continuation = null;
                        }
                    }
                }
                catch (Exception exception)
                {
                    aggregateException = new AggregateException(exception);
                }
                finally
                {
                    isResultReadyEvent.Set();
                }
            };

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newSupplier)
            {
                if (threadPool.cancellationTokenSource.IsCancellationRequested)
                {
                    throw new ThreadPoolShutdownException();
                }

                var task = new MyTask<TNewResult>(() => newSupplier(result), threadPool);

                lock (isCompletedLocker)
                {
                    if (IsCompleted)
                    {
                        threadPool.EnqueueAction(task.TaskStarter);
                    }
                    else
                    {
                        continuation = task.TaskStarter;
                    }
                }

                return task;
            }
        }

        private Thread[] threads;
        private Queue<Action<bool>> actions = new Queue<Action<bool>>();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private Object actionQueueLocker = new Object();

        public int ThreadCount => threads.Length;

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
        }

        private void DoTasks()
        {
            while (true)
            {
                Action<bool> nextAction = null;

                lock (actionQueueLocker)
                {
                    while (actions.Count == 0)
                    {
                        Monitor.Wait(actionQueueLocker);
                    }

                    nextAction = actions.Dequeue();
                }

                nextAction.Invoke(false);

                if (cancellationTokenSource.IsCancellationRequested)
                {
                    lock (actionQueueLocker)
                    {
                        while (actions.Count != 0)
                        {
                            actions.Dequeue().Invoke(true);
                        }
                    }

                    return;
                }
            }
        }

        private void EnqueueAction(Action<bool> action)
        {
            lock (actionQueueLocker)
            {
                actions.Enqueue(action);
                Monitor.Pulse(actionQueueLocker);
            }
        }

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

        public void Shutdown()
        {
            cancellationTokenSource.Cancel();
        }
    }
}