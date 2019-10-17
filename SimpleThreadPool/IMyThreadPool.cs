using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleThreadPool
{
    public interface IMyThreadPool
    {
        int ActiveThreadCount { get; }
        IMyTask<TResult> QueueTask<TResult>(Func<TResult> task);
        void Shutdown();
    }
}
