using System;
using System.Runtime.Serialization;

namespace A3.Arrange
{
    [Serializable]
    public class ArrangeException : Exception
    {
        public ArrangeException() { }
        public ArrangeException(string message) : base(message) { }
        public ArrangeException(string message, Exception inner) : base(message, inner) { }
        protected ArrangeException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
