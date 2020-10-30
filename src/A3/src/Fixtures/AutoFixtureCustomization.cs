using AutoFixture;
using AutoFixture.AutoMoq;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace A3.Fixtures
{
    public class AutoFixtureCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize(new AutoMoqCustomization());

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

            foreach (var customizer in customizers)
            {
                var instance = Expression.New(customizer.Type);

                foreach (var fixtureType in customizer.FixtureTypes)
                {
                    var method = typeof(ICustomizeFixture<>).MakeGenericType(fixtureType).GetMethod(nameof(ICustomizeFixture<object>.Customize));

                    var call = Expression.Call(instance, method, fixtureParameter);

                    var registerFixture = Expression.Call(registerFixtureMethod.MakeGenericMethod(fixtureType), fixtureParameter, call);

                    var lambda = Expression.Lambda<Action<IFixture>>(registerFixture, fixtureParameter).Compile();

                    lambda(fixture);
                }
            }
        }
    }
}
