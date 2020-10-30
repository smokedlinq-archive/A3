using System;

namespace A3
{
    internal class SutParameter
    {
        public SutParameter(Type type, object? value)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            Value = value;
        }

        public Type Type { get; }
        public object? Value { get; }
    }
}
