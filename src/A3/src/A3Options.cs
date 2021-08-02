using A3.Arrange;
using System;

namespace A3
{
    public class A3Options
    {
        internal ArrangeOptions Arrange { get; } = new();

        public static A3Options Scope(string scope)
        {
            var options = new A3Options();
            options.Arrange.Scope = scope ?? throw new ArgumentNullException(nameof(scope));
            return options;
        }
    }
}