using A3.Act;
using A3.Arrange;
using A3.Assert;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace A3.Tests
{
    public class A3Tests
    {
        [Fact]
        public void WhenTIsInterfaceArrangeDoesNotThrow()
        {
            // Arrange
            // Act
            var result = this.Invoking(_ => A3<IWidget>.Arrange(setup => { }));
            // Assert
            result.Should().NotThrow();
        }

        [Fact]
        public void WhenTIsInterfaceAndSutIsExplicitlyDefinedInArrangeThenActDoesNotThrow()
        {
            // Arrange
            // Act
            var result = this.Invoking(_ => A3<IWidget>.Arrange(setup => setup.Sut(_ => new Widget())).Act(_ => { }));
            // Assert
            result.Should().NotThrow();
        }

        [Fact]
        public void WhenTIsInterfaceAndSutIsNotExplicitedDefinedInArrangeThenActShouldThrowArrangeException()
        {
            // Arrange
            // Act
            var result = this.Invoking(_ => A3<IWidget>.Arrange(setup => { }).Act(_ => { }));
            // Assert
            result.Should().Throw<ArrangeException>();
        }

        [Fact]
        public void WhenTIsAbstractArrangeDoesNotThrow()
        {
            // Arrange
            // Act
            var result = this.Invoking(_ => A3<WidgetBase>.Arrange(setup => { }));
            // Assert
            result.Should().NotThrow();
        }

        [Fact]
        public void WhenTIsAbstractAndSutIsExplicitlyDefinedInArrangeThenActDoesNotThrow()
        {
            // Arrange
            // Act
            var result = this.Invoking(_ => A3<WidgetBase>.Arrange(setup => setup.Sut(_ => new Widget())).Act(_ => { }));
            // Assert
            result.Should().NotThrow();
        }

        [Fact]
        public void WhenTIsAbstractAndSutIsNotExplicitedDefinedInArrangeThenActShouldThrowArrangeException()
        {
            // Arrange
            // Act
            var result = this.Invoking(_ => A3<WidgetBase>.Arrange(setup => { }).Act(_ => { }));
            // Assert
            result.Should().Throw<ArrangeException>();
        }

        [Fact]
        public void CanArrangeAndAssertMockWithoutSetup()
            => A3<WidgetService>
            .Arrange(setup => setup.Mock<WidgetFactory>())
            .Act(sut => { /* NOOP */ })
            .Assert(context => context.Mock<WidgetFactory>().Verify(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())));

        [Fact]
        public void CanAssertResult()
            => A3<WidgetService>
            .Arrange(setup => setup.Mock<WidgetFactory>(m => m.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())).Returns(new Widget())))
            .Act(sut => sut.Execute())
            .Assert((context, result) => result.Should().BeFalse());

        [Fact]
        public void CanAssertMock()
            => A3<WidgetService>
            .Arrange(setup => setup.Mock<WidgetFactory>(m => m.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())).Returns(new Widget())))
            .Act(sut =>
            {
                _ = sut.Execute();
            })
            .Assert(context => context.Mock<WidgetFactory>().Verify(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>()), Times.Once));

        [Fact]
        public Task CanAssertResultAsync()
            => A3<WidgetService>
            .Arrange(setup => setup.Mock<WidgetFactory>(m => m.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())).Returns(new Widget())))
            .Act(sut => sut.ExecuteAsync())
            .Assert((context, result) => result.Should().BeFalse());

        [Fact]
        public Task CanAssertMockAsync()
            => A3<WidgetService>
            .Arrange(setup => setup.Mock<WidgetFactory>(m => m.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())).Returns(new Widget())))
            .Act(sut => (Task)sut.ExecuteAsync())
            .Assert(context => context.Mock<WidgetFactory>().Verify(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>()), Times.Once));

        [Fact]
        public Task CanAssertTaskOfTAsync()
            => A3<Widget>
            .Arrange(setup => { })
            .Act(sut => sut.Awaiting(x => x.EnumerateAsync().ToListAsync()))
            .Assert((context, result) => result.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)));

        [Fact]
        public void CanUseAutoFixtureToCreateSut()
            => A3<WidgetService>
            .Arrange(setup => setup.Fixture.Register(() => new WidgetFactory()))
            .Act(sut => sut.Execute())
            .Assert((context, result) => result.Should().BeTrue());

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
            .Assert((context, result) => result.Should().Throw<Exception>());

        [Fact]
        public void CanUseSpecificSutConstructor()
            => A3<Widget>
            .Constructor(type => type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null))
            .Arrange(setup => { })
            .Act(sut => sut.Name)
            .Assert((context, result) => result.Should().NotBeNullOrEmpty());

        [Theory]
        [AutoData]
        public void CanProviderCustomSutFactory(string widgetName, Widget parent)
            => A3<Widget>
            .Arrange(setup => setup.Sut(context => new Widget(widgetName, parent)))
            .Act(sut => sut.Name)
            .Assert((context, result) => result.Should().Be($"{parent.Name}.{widgetName}"));

        [Fact]
        public void WhenArrangeThrowsThenShouldThrowArrangeException()
            => A3<Widget>
            .Arrange(setup => { })
            .Act(sut => new Action(() => A3<object>.Arrange(setup => { throw new Exception(); })))
            .Assert((context, result) => result.Should().Throw<ArrangeException>());

        [Fact]
        public void WhenActThrowsThenShouldThrowActException()
            => A3<Widget>
            .Arrange(setup => { })
            .Act(sut => new Action(() => A3<object>.Arrange(_ => { }).Act(_ => { throw new Exception(); })))
            .Assert((context, result) => result.Should().Throw<ActException>());

        [Fact]
        public void WhenActSutFactoryThrowsThenShouldThrowActException()
            => A3<Widget>
            .Arrange(setup => { })
            .Act(sut => new Action(() => A3<object>.Arrange(setup => setup.Sut((Func<ArrangeContext, object>)(context => { throw new Exception(); }))).Act(_ => { })))
            .Assert((context, result) => result.Should().Throw<ActException>());

        [Fact]
        public void WhenMockDoesNotExistThenShouldThrowAssertException()
            => A3<Widget>
            .Arrange(setup => { })
            .Act(sut => new Action(() => A3<object>.Arrange(_ => { }).Act(_ => { }).Assert(context => context.Mock<WidgetFactory>())))
            .Assert((context, result) => result.Should().Throw<AssertException>());

        [Fact]
        public void CanPassParameterToAct()
            => A3<Widget>
            .Arrange(setup => setup.Parameter(nameof(Widget)))
            .Act((Widget sut, string parameter) => { parameter.Should().NotBeNullOrEmpty(); })
            .Assert(context => true.Should().BeTrue());

        [Fact]
        public void CanPassParameterToActWithResult()
            => A3<Widget>
            .Arrange(setup => setup.Parameter(nameof(Widget)))
            .Act((Widget sut, string parameter) => sut.Name == parameter)
            .Assert((context, result) => result.Should().BeTrue());

        [Fact]
        public Task CanPassParameterToActAsync()
            => A3<Widget>
            .Arrange(setup => setup.Parameter(nameof(Widget)))
            .Act((Widget sut, string parameter) => { Task.FromResult(parameter.Should().NotBeNullOrEmpty()); })
            .Assert(context => Task.CompletedTask);

        [Fact]
        public Task CanPassParameterToActWithResultAsync()
            => A3<Widget>
            .Arrange(setup => setup.Parameter(nameof(Widget)))
            .Act((Widget sut, string parameter) => Task.FromResult(sut.Name == parameter))
            .Assert((context, result) => result.Should().BeTrue());
    }
}
