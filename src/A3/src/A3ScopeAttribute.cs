using System;

namespace A3
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Assembly, AllowMultiple = false, Inherited = true)]
    public class A3ScopeAttribute : Attribute
    {
        public A3ScopeAttribute(string name)
            => Name = name ?? throw new ArgumentNullException(nameof(name));

        public string Name { get; }
    }
}
