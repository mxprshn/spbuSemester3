using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleThreadPool
{

    [Serializable]
    public class ThreadPoolShutdownException : Exception
    {
        public ThreadPoolShutdownException() { }
        public ThreadPoolShutdownException(string message) : base(message) { }
        public ThreadPoolShutdownException(string message, Exception inner) : base(message, inner) { }
        protected ThreadPoolShutdownException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
