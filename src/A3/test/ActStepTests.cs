using A3.Act;
using A3.Arrange;
using A3.Tests.Fixtures;
using FluentAssertions;
using System;
using Xunit;

namespace A3.Tests
{
    public class ActStepTests
    {
        [Fact]
        public void WhenTIsInterfaceAndSutIsExplicitlyDefinedInArrangeThenActDoesNotThrow()
            => A3<ActStep<IWidget, object>>
            .Arrange(setup => setup.Sut(A3<IWidget>.Arrange(setup => setup.Sut(_ => new Widget()))))
            .Act(sut => sut.Invoking(x => x.Act(_ => { })))
            .Assert(result => result.Should().NotThrow());

        [Fact]
        public void WhenTIsInterfaceAndSutIsNotExplicitedDefinedInArrangeThenActShouldThrowArrangeException()
            => A3<ActStep<IWidget, object>>
            .Arrange(setup => setup.Sut(A3<IWidget>.Arrange(setup => { })))
            .Act(sut => sut.Invoking(x => x.Act(_ => { })))
            .Assert(result => result.Should().Throw<ArrangeException>());

        [Fact]
        public void WhenTIsAbstractAndSutIsExplicitlyDefinedInArrangeThenActDoesNotThrow()
            => A3<ActStep<WidgetBase, object>>
            .Arrange(setup => setup.Sut(A3<WidgetBase>.Arrange(setup => setup.Sut(_ => new Widget()))))
            .Act(sut => sut.Invoking(x => x.Act(_ => { })))
            .Assert(result => result.Should().NotThrow());

        [Fact]
        public void WhenTIsAbstractAndSutIsNotExplicitedDefinedInArrangeThenActShouldThrowArrangeException()
            => A3<ActStep<WidgetBase, object>>
            .Arrange(setup => setup.Sut(A3<WidgetBase>.Arrange(setup => { })))
            .Act(sut => sut.Invoking(x => x.Act(_ => { })))
            .Assert(result => result.Should().Throw<ArrangeException>());

        [Fact]
        public void WhenActThrowsThenShouldThrowActException()
            => A3<Widget>
            .Arrange(setup => { })
            .Act(sut => new Action(() => A3<object>.Arrange(_ => { }).Act(_ => { throw new Exception(); })))
            .Assert(result => result.Should().Throw<ActException>());

        [Fact]
        public void WhenActSutFactoryThrowsThenShouldThrowActException()
            => A3<Widget>
            .Arrange(setup => { })
            .Act(sut => new Action(() => A3<object>.Arrange(setup => setup.Sut((Func<ArrangeContext, object>)(context => { throw new Exception(); }))).Act(_ => { })))
            .Assert(result => result.Should().Throw<ActException>());
    }
}
