using AutoFixture;
using AutoFixture.Kernel;
using Moq;
using System.Reflection;

namespace A3.Arrange
{
    internal static class ArrangeSutFactory<T>
        where T : class
    {
        public static T Create(ArrangeContext context, ConstructorInfo? constructor)
        {
            _ = constructor ?? throw new ArrangeException($"Could not instantiate {typeof(T)}; verify it is not an interface or abstract, or that the Sut is explicitly configured during the arrange step.");

            var parameters = constructor.GetParameters();
            var args = new object[parameters.Length];
            var fixture = new SpecimenContext(context.Fixture);

            for (var i = 0; i < parameters.Length; i++)
            {
                var ctorParameter = parameters[i];

                if (context.TryGetMock(ctorParameter.ParameterType, out var mock))
                {
                    args[i] = mock.Object;
                }
                else
                {
                    try
                    {
                        args[i] = fixture.Resolve(ctorParameter.ParameterType);
                    }
                    catch (ObjectCreationException)
                    {
                        args[i] = ((Mock)fixture.Resolve(typeof(Mock<>).MakeGenericType(ctorParameter.ParameterType))).Object;
                    }
                }
            }

            return (T)constructor.Invoke(args);
        }
    }
}
