using A3.Act;
using AutoFixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace A3.Arrange
{
    public sealed class ArrangeBuilder<TSut, TParameter>
    {
        private readonly List<Func<ArrangeContext, TSut>> factory = new List<Func<ArrangeContext, TSut>>();
        private readonly List<Mock> mocks = new List<Mock>();
        private readonly List<TParameter> parameter = new List<TParameter>();

        internal ArrangeBuilder(Func<ArrangeContext, TSut> factory, ArrangeOptions options)
        {
            this.factory.Add(factory);
            Fixture = new Fixture().Customize(new AutoFixtureCustomization(options.Scope));
        }

        public IFixture Fixture { get; }

        public Mock<TMock> Mock<TMock>(Action<Mock<TMock>> setup)
            where TMock : class
        {
            var mock = GetOrAddMock<TMock>();
            setup(mock);
            return mock;
        }

        public Mock<TMock> Mock<TMock>(Mock<TMock> mock)
            where TMock : class
        {
            var existing = mocks.FirstOrDefault(x => x is Mock<TMock>);

            if (!(existing is null))
            {
                mocks.Remove(existing);
            }

            mocks.Add(mock);
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

        public ArrangeBuilder<TSut, TParameter> Constructor(Func<Type, ConstructorInfo?> selector)
        {
            _ = selector ?? throw new ArgumentNullException(nameof(selector));
            factory.Add(context => ArrangeSutFactory<TSut>.Create(context, selector(typeof(TSut))));
            return this;
        }

        public ArrangeBuilder<TSut, TParameter> Sut(Func<ArrangeContext, TSut> factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            this.factory.Add(factory);
            return this;
        }

        public ArrangeBuilder<TSut, TParameter> Sut(Func<TSut> factory)
        {
            _ = factory ?? throw new ArgumentNullException(nameof(factory));
            this.factory.Add(_ => factory());
            return this;
        }

        public ArrangeBuilder<TSut, TParameter> Sut(TSut sut)
        {
            _ = sut ?? throw new ArgumentNullException(nameof(sut));
            factory.Add(_ => sut);
            return this;
        }

        public TSpecificParameter Parameter<TSpecificParameter>(TSpecificParameter parameter)
        {
            if (typeof(TParameter) != typeof(TSpecificParameter) && !typeof(TParameter).IsAssignableFrom(typeof(TSpecificParameter)))
            {
                throw new ArrangeException($"The specified parameter type {typeof(TSpecificParameter)} value cannot be used with parameters of type {typeof(TParameter)}.");
            }

            this.parameter.Add((TParameter)(object)parameter!);
            return parameter;
        }

        internal ActStep<TSut, TParameter> Build()
            => new ActStep<TSut, TParameter>(() => factory.Last().Invoke(new ArrangeContext(Fixture, mocks)), parameter.LastOrDefault(), mocks);
    }
}
