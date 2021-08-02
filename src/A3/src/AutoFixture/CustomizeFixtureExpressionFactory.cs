using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace AutoFixture
{
    internal static class CustomizeFixtureExpressionFactory
    {
        public static IEnumerable<Action<IFixture, string?>> Create()
        {
            var customizers = CustomizeFixtureTypeDiscovery.FindTypesThatImplementInterface(typeof(ICustomizeFixture));
            var fixtureParameter = Expression.Parameter(typeof(IFixture));
            var scopeParameter = Expression.Parameter(typeof(string));

            foreach (var customizer in customizers)
            {
                var instance = Expression.New(customizer);
                var customizerType = typeof(ICustomizeFixture);
                var cutomizeMethod = customizerType.GetMethod(nameof(ICustomizeFixture.Customize))!;

                var call = Expression.Call(instance, cutomizeMethod, fixtureParameter);

                var shouldCustomizeMethod = customizerType.GetMethod(nameof(ICustomizeFixture.ShouldCustomize))!;

                var shouldCustomize = Expression.Call(instance, shouldCustomizeMethod, scopeParameter);

                var ifShouldCustomize = Expression.IfThen(shouldCustomize, call);

                var lambda = Expression.Lambda<Action<IFixture, string?>>(ifShouldCustomize, fixtureParameter, scopeParameter).Compile();

                yield return lambda;
            }
        }
    }
}