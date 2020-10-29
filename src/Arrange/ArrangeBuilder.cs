using A3.Act;
using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Kernel;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace A3.Arrange
{
    public sealed class ArrangeBuilder<T>
        where T : class
    {
        private readonly List<Func<ArrangeContext, T>> factory = new List<Func<ArrangeContext, T>>();
        private readonly List<Mock> mocks = new List<Mock>();

        internal ArrangeBuilder(ConstructorInfo constructor)
            => factory.Add(CreateConstructorSutFactory(constructor));

        public IFixture Fixture { get; } = new Fixture().Customize(new AutoMoqCustomization());

        public ArrangeBuilder<T> AddMock<TMock>(Action<Mock<TMock>> setup)
            where TMock : class
        {
            var mock = GetOrAddMock<TMock>();
            setup(mock);
            return this;
        }

        public ArrangeBuilder<T> AddMock<TMock>()
            where TMock : class
        {
            _ = GetOrAddMock<TMock>();
            return this;
        }

        private Mock<TMock> GetOrAddMock<TMock>()
            where TMock : class
        {
            var mock = (Mock<TMock>)mocks.FirstOrDefault(x => x is Mock<TMock>);

            if (mock is null)
            {
                mock = Fixture.Create<Mock<TMock>>();
                mocks.Add(mock);
            }

            return mock;
        }

        public ArrangeBuilder<T> UseSutConstructor(Func<Type, ConstructorInfo> selector)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            factory.Add(CreateConstructorSutFactory(selector(typeof(T))));
            return this;
        }

        public ArrangeBuilder<T> UseSut(Func<ArrangeContext, T> factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            this.factory.Add(factory);
            return this;
        }

        internal ActStep<T> Build()
            => new ActStep<T>(() => factory.Last().Invoke(new ArrangeContext(Fixture, mocks)), mocks);

        private static Func<ArrangeContext, T> CreateConstructorSutFactory(ConstructorInfo constructor)
        {
            _ = constructor ?? throw new ArgumentNullException(nameof(constructor));

            return new Func<ArrangeContext, T>(context =>
            {
                var parameters = constructor.GetParameters();
                var args = new object[parameters.Length];
                var fixture = new SpecimenContext(context.Fixture);

                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];

                    if (context.TryGetMock(parameter.ParameterType, out var mock))
                    {
                        args[i] = mock.Object;
                    }
                    else
                    {
                        try
                        {
                            args[i] = fixture.Resolve(parameter.ParameterType);
                        }
                        catch (ObjectCreationException)
                        {
                            args[i] = ((Mock)fixture.Resolve(typeof(Mock<>).MakeGenericType(parameter.ParameterType))).Object;
                        }
                    }
                }

                return (T)constructor.Invoke(args);
            });
        }
    }
}
