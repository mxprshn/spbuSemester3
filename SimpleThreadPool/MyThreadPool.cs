using System;
using System.Threading;
using System.Collections.Generic;

// добавили новый таск в очередь -- потоки должны узнать, что можно будет его достать

namespace SimpleThreadPool
{
    class MyThreadPool : IMyThreadPool
    {
        private Thread[] threads;
        // отдавать сюда что-то из MyTask
        private Queue<Action> tasks;
        private AutoResetEvent resetEvent = new AutoResetEvent(false);
        private CancellationTokenSource tokenSource = new CancellationTokenSource();
        private Object locker = new Object();

        public int ThreadCount => threads.Length;

        public MyThreadPool(int threadCount)
        {
            threads = new Thread[threadCount];

            for (var i = 0; i < threadCount; ++i)
            {
                // как-то передавать токен сюда
                threads[i] = new Thread(DoTasks);
                threads[i].Start();
            }
        }

        private void DoTasks()
        {
            while (true)
            {
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

        public MyTask<TResult> QueueTask<TResult>(Func<TResult> supplier)
        {
            var task = new MyTask<TResult>(supplier);

            lock (locker)
            {
                tasks.Enqueue(task.TaskStarter);
                Monitor.Pulse(locker);
            }
            
            return task;
        }

        public void Shutdown()
        {

        }
    }

}
