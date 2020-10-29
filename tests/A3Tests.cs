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
        public void CanAssertResult()
            => A3<WidgetService>
            .Arrange(setup => setup.AddMock<WidgetFactory>(m => m.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())).Returns(new Widget())))
            .Act(sut => sut.Execute())
            .Assert((context, result) => result.Should().BeFalse());

        [Fact]
        public void CanAssertMock()
            => A3<WidgetService>
            .Arrange(setup => setup.AddMock<WidgetFactory>(m => m.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())).Returns(new Widget())))
            .Act(sut =>
            {
                _ = sut.Execute();
            })
            .Assert(context => context.Mock<WidgetFactory>().Verify(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>()), Times.Once));

        [Fact]
        public Task CanAssertResultAsync()
            => A3<WidgetService>
            .Arrange(setup => setup.AddMock<WidgetFactory>(m => m.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())).Returns(new Widget())))
            .Act(sut => sut.ExecuteAsync())
            .Assert((context, result) => result.Should().BeFalse());

        [Fact]
        public Task CanAssertMockAsync()
            => A3<WidgetService>
            .Arrange(setup => setup.AddMock<WidgetFactory>(m => m.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())).Returns(new Widget())))
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

                setup.UseSut(context => context.Fixture.Create<Widget>());
            })
            .Act(sut => sut.Invoking(x => x.Execute()))
            .Assert((context, result) => result.Should().Throw<Exception>());

        [Fact]
        public void CanUseSpecificSutConstructor()
            => A3<Widget>
            .Constructor(type => type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { typeof(string) }, null))
            .Arrange(setup => { })
            .Act(sut => sut.Name)
            .Assert((context, result) => result.Should().NotBeNullOrEmpty());

        [Theory]
        [AutoData]
        public void CanProviderCustomSutFactory(string widgetName, Widget parent)
            => A3<Widget>
            .Arrange(setup => setup.UseSut(context => new Widget(widgetName, parent)))
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
            .Act(sut => new Action(() => A3<object>.Arrange(setup => setup.UseSut(context => { throw new Exception(); })).Act(_ => { })))
            .Assert((context, result) => result.Should().Throw<ActException>());

        [Fact]
        public void WhenMockDoesNotExistThenShouldThrowAssertException()
            => A3<Widget>
            .Arrange(setup => { })
            .Act(sut => new Action(() => A3<object>.Arrange(_ => { }).Act(_ => { }).Assert(context => context.Mock<WidgetFactory>())))
            .Assert((context, result) => result.Should().Throw<AssertException>());
    }
}
