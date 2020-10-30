using System;

namespace A3
{
    public class A3Options
    {
        public string? AutoFixtureCustomizationScope { get; set; }

        public static A3Options WithAutoFixtureCustomizationScope(string scope)
            => new A3Options { AutoFixtureCustomizationScope = scope ?? throw new ArgumentNullException(nameof(scope)) };
    }
}