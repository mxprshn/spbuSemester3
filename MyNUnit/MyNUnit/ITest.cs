using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyNUnit
{
    public interface ITest
    {
        void Run();
        string ClassName { get; }
        string Name { get; }
        bool? IsPassed { get; }
        bool IsIgnored { get; }
        string IgnoreReason { get; }
        TimeSpan RunTime { get; }
    }
}