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
            private Action continuation;
            private MyThreadPool threadPool;
            private Func<TResult> supplier;
            private TResult result;
            private AggregateException aggregateException;
            private ManualResetEvent isResultReadyEvent = new ManualResetEvent(false);
            private bool isCancelled = false; // можно сделать свойством в интерфейсе
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

            public Action TaskStarter => () => 
            {
                try
                {
                    if (threadPool.cancellationTokenSource.IsCancellationRequested)
                    {
                        isCancelled = true;
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

                    lock (isCompletedLocker)
                    {
                        if (continuation != null)
                        {
                            threadPool.EnqueueAction(continuation);
                            continuation = null;
                        }
                    }
                }
            };

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newSupplier)
            {
                if (threadPool.cancellationTokenSource.IsCancellationRequested)
                {
                    throw new ThreadPoolShutdownException();
                }

                var task = new MyTask<TNewResult>(() => newSupplier(Result), threadPool);

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