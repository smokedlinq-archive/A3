using System;
using System.Runtime.Serialization;

namespace A3.Assert
{
    [Serializable]
    public class AssertException : Exception
    {
        public AssertException() { }
        public AssertException(string message) : base(message) { }
        public AssertException(string message, Exception inner) : base(message, inner) { }
        protected AssertException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
