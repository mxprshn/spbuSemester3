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
            // добавить многопоточности
            private Action continuation;
            private MyThreadPool threadPool;
            private Func<TResult> supplier;
            private TResult result;
            private AggregateException aggregateException;
            private ManualResetEvent resetEvent = new ManualResetEvent(false);

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
                    resetEvent.WaitOne();

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

            public Action TaskStarter => () => {

                try
                {
                    result = supplier();
                    IsCompleted = true;

                    if (continuation != null)
                    {
                        threadPool.EnqueueAction(continuation);
                    }
                }
                catch (Exception exception)
                {
                    aggregateException = new AggregateException(exception);
                }
                finally
                {
                    resetEvent.Set();
                }
            };

            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask)
            {
                var task = new MyTask<TNewResult>(() => newTask(result), threadPool);
                continuation = task.TaskStarter;

                // синхронизация
                if (IsCompleted)
                {
                    threadPool.EnqueueAction(continuation);
                }

                return task;
            }
        }

        private Thread[] threads;
        private Queue<Action> tasks = new Queue<Action>();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private Object locker = new Object();

        public int ThreadCount => threads.Length;

        public MyThreadPool(int threadCount)
        {
            // проверка на отрицательные значения threadCount
            threads = new Thread[threadCount];
            

            for (var i = 0; i < threadCount; ++i)
            {
                // как-то передавать токен сюда
                threads[i] = new Thread(() => DoTasks(cancellationTokenSource.Token));
                threads[i].Start();
            }
        }

        private void DoTasks(CancellationToken cancellationToken)
        {
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    return;
                }

                Action action = null;

                lock (locker)
                {
                    while (tasks.Count == 0)
                    {
                        Monitor.Wait(locker);
                    }

                    action = tasks.Dequeue();
                }

                action.Invoke();
            }
        }

        private void EnqueueAction(Action action)
        {
            lock (locker)
            {
                tasks.Enqueue(action);
                Monitor.Pulse(locker);
            }
        }

        public IMyTask<TResult> QueueTask<TResult>(Func<TResult> supplier)
        {
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
