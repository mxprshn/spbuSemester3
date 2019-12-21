using System;

namespace MyNUnit
{
    /// <summary>
    /// Exception thrown in case of running invalid MyNUnit methods.
    /// </summary>
    [Serializable]
    public class TestRunnerException : Exception
    {
        public TestRunnerException() { }
        public TestRunnerException(string message) : base(message) { }
        public TestRunnerException(string message, Exception inner) : base(message, inner) { }
        protected TestRunnerException(System.Runtime.Serialization.SerializationInfo info,
                System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}