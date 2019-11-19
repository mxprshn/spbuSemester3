using System;
using System.Collections.Generic;
using System.Text;

namespace FTPClient
{
    [Serializable]
    public class ConnectionToServerException : Exception
    {
        public ConnectionToServerException() { }
        public ConnectionToServerException(string message) : base(message) { }
        public ConnectionToServerException(string message, Exception inner) : base(message, inner) { }
        protected ConnectionToServerException(System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}