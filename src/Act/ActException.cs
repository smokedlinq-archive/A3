using System;
using System.Runtime.Serialization;

namespace A3.Act
{
    [Serializable]
    public class ActException : Exception
    {
        public ActException() { }
        public ActException(string message) : base(message) { }
        public ActException(string message, Exception inner) : base(message, inner) { }
        protected ActException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
