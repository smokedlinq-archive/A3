using AutoFixture.AutoMoq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoFixture
{
    public class AutoFixtureCustomization : ICustomization
    {
        private static readonly Lazy<IEnumerable<Action<IFixture, string?>>> Customizations = new Lazy<IEnumerable<Action<IFixture, string?>>>(CreateCustomizeExpressions);
        private readonly string? scope;

        public AutoFixtureCustomization(string? scope = null)
            => this.scope = scope;

        public void Customize(IFixture fixture)
        {
            fixture.Customize(new AutoMoqCustomization());
            foreach (var customization in Customizations.Value)
            {
                customization(fixture, scope);
            }
        }

        private static IEnumerable<Action<IFixture, string?>> CreateCustomizeExpressions()
            => CustomizeFixtureExpressionFactory.Create().Union(CustomizeFixtureOfTExpressionFactory.Create());
    }
}
