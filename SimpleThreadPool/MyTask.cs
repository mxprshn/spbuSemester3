using System;
using System.Threading;

namespace SimpleThreadPool
{
    class MyTask<TResult> : IMyTask<TResult>
    {
        private Func<TResult> supplier;
        private TResult result;
        private AggregateException aggregateException;
        private Mutex mutex = new Mutex();

        public TResult Result
        {
            get
            {
                try
                {
                    mutex.WaitOne();

                    if (aggregateException != null)
                    {
                        throw aggregateException;
                    }

                    return result;
                }
                finally
                {
                    mutex.ReleaseMutex();
                }
            }

            private set
            {
                result = value;
            }
        }

        public Action TaskStarter => () => {

            try
            {
                mutex.WaitOne();
                result = supplier();
                IsCompleted = true;
            }
            catch (Exception exception)
            {
                aggregateException = new AggregateException(exception);
            }
            finally
            {
                mutex.ReleaseMutex();
            }

        };

        public MyTask(Func<TResult> supplier)
        {
            this.supplier = supplier;
        }

        public bool IsCompleted { get; private set; } = false;
        
        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask)
        {
            throw new NotImplementedException();
        }
    }
}