using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleThreadPool
{
    public interface IMyTask<TResult>
    {
        bool IsCompleted { get; }
        TResult Result { get; }
        Action TaskStarter { get; }
        IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> newTask);
    }
}