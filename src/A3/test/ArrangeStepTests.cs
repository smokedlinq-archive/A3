using A3.Act;
using A3.Arrange;
using A3.Tests.Fixtures;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace A3.Tests
{
    public class ArrangeStepTests
    {
        [Fact]
        public void CanSpecifySutConstructor()
            => A3<Widget>
            .Arrange(setup => setup.Constructor(type => type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null)))
            .Act(sut => sut.Name)
            .Assert((result, context) => result.Should().NotBeNullOrEmpty());

        [Fact]
        public void CanSpecifySutWithFactory()
            => A3<Widget>
            .Arrange(setup => setup.Sut(new Widget()))
            .Act(sut => sut)
            .Assert(result => result.Should().NotBeNull());

        [Fact]
        public void CanSpecifySutWithValue()
            => A3<Widget>
            .Arrange(setup => setup.Sut(new Widget()))
            .Act(sut => sut)
            .Assert(result => result.Should().NotBeNull());

        [Fact]
        public void CanUseAutoFixtureToCreateSut()
            => A3<WidgetService>
            .Arrange(setup => setup.Fixture.Register(() => new WidgetFactory()))
            .Act(sut => sut.Execute())
            .Assert(result => result.Should().BeTrue());

        [Fact]
        public void CanUseAutoFixtureToCreateMockForSut()
            => A3<Widget>
            .Arrange(setup =>
            {
                setup.Fixture.Register(() =>
                {
                    var mock = new Mock<Widget>();
                    mock.Setup(x => x.Execute()).Throws<Exception>();
                    return mock.Object;
                });

                setup.Sut(context => context.Fixture.Create<Widget>());
            })
            .Act(sut => sut.Invoking(x => x.Execute()))
            .Assert(result => result.Should().Throw<Exception>());

        [Fact]
        public void CanPassParameterToAct()
            => A3<Widget>
            .Arrange(setup => setup.Parameter(nameof(Widget)))
            .Act((Widget sut, string parameter) => { parameter.Should().NotBeNullOrEmpty(); })
            .Assert(context => true.Should().BeTrue());

        [Fact]
        public void CanPassParameterToActFromReturnValue()
            => A3<Widget>
            .Arrange<string>(_ => nameof(Widget))
            .Act((sut, parameter) => { parameter.Should().NotBeNullOrEmpty(); })
            .Assert(context => true.Should().BeTrue());

        [Fact]
        public void CanPassParameterToActWithResult()
            => A3<Widget>
            .Arrange(setup => setup.Parameter(nameof(Widget)))
            .Act((Widget sut, string parameter) => sut.Name == parameter)
            .Assert(result => result.Should().BeTrue());

        [Fact]
        public void CanPassParameterFromReturnValueToActWithResult()
            => A3<Widget>
            .Arrange<string>(_ => nameof(Widget))
            .Act((sut, parameter) => sut.Name == parameter)
            .Assert(result => result.Should().BeTrue());

        [Fact]
        public Task CanPassParameterToActAsync()
            => A3<Widget>
            .Arrange(setup => setup.Parameter(nameof(Widget)))
            .Act((Widget sut, string parameter) => { Task.FromResult(parameter.Should().NotBeNullOrEmpty()); })
            .Assert(context => Task.CompletedTask);

        [Fact]
        public Task CanPassParameterFromReturnValueToActAsync()
            => A3<Widget>
            .Arrange<string>(_ => nameof(Widget))
            .Act((sut, parameter) => Task.FromResult(parameter.Should().NotBeNullOrEmpty()))
            .Assert(context => Task.CompletedTask);

        [Fact]
        public Task CanPassParameterToActWithResultAsync()
            => A3<Widget>
            .Arrange(setup => setup.Parameter(nameof(Widget)))
            .Act((Widget sut, string parameter) => Task.FromResult(sut.Name == parameter))
            .Assert(result => result.Should().BeTrue());

        [Fact]
        public Task CanPassParameterWithReturnValueToActWithResultAsync()
            => A3<Widget>
            .Arrange<string>(_ => nameof(Widget))
            .Act((sut, parameter) => Task.FromResult(sut.Name == parameter))
            .Assert(result => result.Should().BeTrue());

        [Theory]
        [AutoData]
        public void CanProviderCustomSutFactory(string widgetName, Widget parent)
            => A3<Widget>
            .Arrange(setup => setup.Sut(context => new Widget(widgetName, parent)))
            .Act(sut => sut.Name)
            .Assert(result => result.Should().Be($"{parent.Name}.{widgetName}"));

        [Fact]
        public void WhenArrangeThrowsThenShouldThrowArrangeException()
            => A3<Widget>
            .Arrange(setup => { })
            .Act(sut => new Action(() => A3<object>.Arrange(setup => { throw new Exception(); })))
            .Assert(result => result.Should().Throw<ArrangeException>());

        [Fact]
        public void WhenTIsInterfaceArrangeDoesNotThrow()
            => A3<Func<ActStep<IWidget, object>>>
            .Arrange(setup => setup.Sut(this.Invoking(_ => A3<IWidget>.Arrange(setup => { }))))
            .Act(sut => sut)
            .Assert(result => result.Should().NotThrow());

        [Fact]
        public void WhenTIsAbstractArrangeDoesNotThrow()
            => A3<Func<ActStep<WidgetBase, object>>>
            .Arrange(setup => setup.Sut(this.Invoking(_ => A3<WidgetBase>.Arrange(setup => { }))))
            .Act(sut => sut)
            .Assert(result => result.Should().NotThrow());
    }
}
