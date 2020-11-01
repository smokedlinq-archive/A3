using A3.Act;
using AutoFixture;
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
        private readonly List<SutParameter> parameter = new List<SutParameter>();

        internal ArrangeBuilder(Func<ArrangeContext, T> factory, ArrangeOptions options)
        {
            this.factory.Add(factory);
            Fixture = new Fixture().Customize(new AutoFixtureCustomization(scope: options.AutoFixtureCustomizationScope));
        }

        public IFixture Fixture { get; }

        public Mock<TMock> Mock<TMock>(Action<Mock<TMock>> setup)
            where TMock : class
        {
            var mock = GetOrAddMock<TMock>();
            setup(mock);
            return mock;
        }

        public Mock<TMock> Mock<TMock>()
            where TMock : class
            => GetOrAddMock<TMock>();

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

        public ArrangeBuilder<T> Sut(Func<Type, ConstructorInfo?> selector)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            factory.Add(context => ArrangeSutFactory<T>.Create(context, selector(typeof(T))));
            return this;
        }

        public ArrangeBuilder<T> Sut(Func<ArrangeContext, T> factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            this.factory.Add(factory);
            return this;
        }

        public ArrangeBuilder<T> Sut(Func<T> factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            this.factory.Add(_ => factory());
            return this;
        }

        public ArrangeBuilder<T> Sut(T sut)
        {
            _ = sut ?? throw new ArgumentNullException(nameof(sut));
            this.factory.Add(_ => sut);
            return this;
        }

        public ArrangeBuilder<T> Parameter<TParameter>(TParameter parameter)
        {
            this.parameter.Add(new SutParameter(typeof(TParameter), parameter));
            return this;
        }

        internal ActStep<T> Build()
            => new ActStep<T>(() => factory.Last().Invoke(new ArrangeContext(Fixture, mocks)), parameter.LastOrDefault(), mocks);
    }
}
