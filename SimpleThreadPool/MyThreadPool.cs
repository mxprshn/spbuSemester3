using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;

namespace SimpleThreadPool
{
    class MyThreadPool : IMyThreadPool
    {
        private Thread[] threads;
        private ConcurrentQueue<Action> tasks;
        private Semaphore semaphore;
        private CancellationTokenSource tokenSource = new CancellationTokenSource();

        public int ThreadCount => threads.Length;

        public MyThreadPool(int threadCount)
        {
            semaphore = new Semaphore(0, threadCount);
            threads = new Thread[threadCount];

            for (var i = 0; i < threadCount; ++i)
            {
                threads[i] = new Thread(DoTasks(tokenSource.Token));
                threads[i].Start();
            }
        }

        private void DoTasks(CancellationToken cancellationToken)
        {
            while (true)
            {
                semaphore.WaitOne();
                // Add more concurrency
                if (tasks.TryDequeue(out var action))
                {
                    action.Invoke();
                }
                semaphore.Release();
            }
        }

        public MyTask<TResult> QueueTask<TResult>(Func<TResult> supplier)
        {
            var task = new MyTask<TResult>(supplier);
            tasks.Enqueue(new Action(task));
            return null;
        }

        public void Shutdown()
        {

        }
    }

}
