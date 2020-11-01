using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoFixture
{
    internal static class CustomizeFixtureTypeDiscovery
    {
        public static IEnumerable<Type> FindTypesThatImplementInterface(Type type)
        {
            var targetAssembly = typeof(AutoFixtureCustomization).Assembly.GetName();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var assembliesThatReferenceA3 = assemblies.Where(x => x.GetReferencedAssemblies().Any(a => a.FullName == targetAssembly.FullName));
            var types = assembliesThatReferenceA3.SelectMany(x => x.GetTypes());
            var typesThatAreNotAbstract = types.Where(x => !x.IsAbstract && x.IsClass);
            var typesThatImplementICustomizeFixture = typesThatAreNotAbstract.Where(x => x.GetInterfaces().Any(i =>
            {
                if (type.IsGenericType && type.IsGenericTypeDefinition)
                {
                    return i.IsGenericType && i.GetGenericTypeDefinition() == type;
                }

                return i == type;
            }));
            var typesWithEmptyCtor = typesThatImplementICustomizeFixture.Where(x => !(x.GetConstructor(Type.EmptyTypes) is null));

            return typesWithEmptyCtor;
        }
    }
}
