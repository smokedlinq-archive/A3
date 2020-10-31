﻿using AutoFixture.AutoMoq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace AutoFixture
{
    public class AutoFixtureCustomization : ICustomization
    {
        private static readonly Lazy<IEnumerable<Action<IFixture, string?>>> Customizations = new Lazy<IEnumerable<Action<IFixture, string?>>>(CreateCustomizeFixtureExpressions);
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

        private static IEnumerable<Action<IFixture, string?>> CreateCustomizeFixtureExpressions()
        {
            var targetAssembly = typeof(AutoFixtureCustomization).Assembly.GetName();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembliesThatReferenceA3 = assemblies.Where(x => x.GetReferencedAssemblies().Any(a => a.FullName == targetAssembly.FullName));
            var types = assembliesThatReferenceA3.SelectMany(x => x.GetTypes());
            var typesThatAreNotAbstract = types.Where(x => !x.IsAbstract && x.IsClass);
            var typesThatImplementICustomizeFixture = typesThatAreNotAbstract.Where(x => x.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICustomizeFixture<>)));
            var typesWithEmptyCtor = typesThatImplementICustomizeFixture.Where(x => !(x.GetConstructor(Type.EmptyTypes) is null));
            var customizers = typesWithEmptyCtor
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