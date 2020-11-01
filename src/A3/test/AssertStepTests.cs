using A3.Assert;
using A3.Tests.Fixtures;
using FluentAssertions;
using Moq;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace A3.Tests
{
    public class AssertStepTests
    {
        [Fact]
        public void CanAssertResult()
            => A3<WidgetService>
            .Arrange(setup => setup.Mock<WidgetFactory>(m => m.Setup(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())).Returns(new Widget())))
            .Act(sut => sut.Execute())
            .Assert(result => result.Should().BeFalse());

        [Fact]
        public void CanAssertMockThatIsUnconfigured()
            => A3<WidgetService>
            .Arrange(setup => setup.Mock<WidgetFactory>())
            .Act(sut => { /* NOOP */ })
            .Assert(context => context.Mock<WidgetFactory>().Verify(x => x.Create(It.IsAny<string>(), It.IsAny<Widget>())));

        [Fact]
        public void CanAssertMockThatIsConfigured()
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
            .Assert(result => result.Should().BeFalse());

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
            .Assert(result => result.Should().CompleteWithinAsync(TimeSpan.FromSeconds(5)));

        [Fact]
        public void WhenMockDoesNotExistThenShouldThrowAssertException()
            => A3<AssertStep<Widget>>
            .Arrange(setup => setup.Sut(A3<Widget>.Arrange(_ => { }).Act(_ => { })))
            .Act(sut => sut.Invoking(x => x.Assert(context => context.Mock<WidgetFactory>())))
            .Assert(result => result.Should().Throw<AssertException>());
    }
}
