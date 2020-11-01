using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AutoFixture
{
    internal static class CustomizeFixtureOfTExpressionFactory
    {
        public static IEnumerable<Action<IFixture, string?>> Create()
        {
            var customizers = CustomizeFixtureTypeDiscovery.FindTypesThatImplementInterface(typeof(ICustomizeFixture<>))
                .Select(x => new
                {
                    Type = x,
                    FixtureTypes = x
                        .GetInterfaces()
                        .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICustomizeFixture<>))
                        .Select(i => i.GetGenericArguments()[0])
                });
            var fixtureParameter = Expression.Parameter(typeof(IFixture));
            var registerFixtureMethod = typeof(FixtureRegistrar).GetMethods().First(x => x.Name == nameof(FixtureRegistrar.Inject) && x.IsGenericMethod);
            var scopeParameter = Expression.Parameter(typeof(string));

            foreach (var customizer in customizers)
            {
                var instance = Expression.New(customizer.Type);

                foreach (var fixtureType in customizer.FixtureTypes)
                {
                    var customizeFixtureType = typeof(ICustomizeFixture<>).MakeGenericType(fixtureType);
                    var cutomizeMethod = customizeFixtureType.GetMethod(nameof(ICustomizeFixture<object>.Customize));

                    var call = Expression.Call(instance, cutomizeMethod, fixtureParameter);

                    var registerFixture = Expression.Call(registerFixtureMethod.MakeGenericMethod(fixtureType), fixtureParameter, call);

                    var shouldCustomizeMethod = customizeFixtureType.GetMethod(nameof(ICustomizeFixture<object>.ShouldCustomize));

                    var shouldCustomize = Expression.Call(instance, shouldCustomizeMethod, scopeParameter);

                    var ifShouldCustomize = Expression.IfThen(shouldCustomize, registerFixture);

                    var lambda = Expression.Lambda<Action<IFixture, string?>>(ifShouldCustomize, fixtureParameter, scopeParameter).Compile();

                    yield return lambda;
                }
            }
        }
    }
}
