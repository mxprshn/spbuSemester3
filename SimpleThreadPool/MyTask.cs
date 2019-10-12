using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleThreadPool
{
    class MyTask<TResult> : IMyTask<TResult>
    {
        private Func<TResult> supplier;

        public MyTask(Func<TResult> supplier)
        {
            this.supplier = supplier;
        }

        public bool IsCompleted { get; private set; } = false;

        // Property?
        // Also thread-safe?
        public TResult Result
        {
            get
            {
                var result = supplier();
                IsCompleted = true;
                return result;
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask)
        {
            throw new NotImplementedException();
        }
    }
}