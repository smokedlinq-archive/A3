using A3.Tests.Fixtures;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace A3.Tests
{
    public class A3Tests
    {
        [Fact]
        public void CanSpecifySutWithFactory()
            => A3<Widget>
            .Sut(_ => new Widget())
            .Arrange(setup => { })
            .Act(sut => sut)
            .Assert(result => result.Should().NotBeNull());

        [Fact]
        public void CanSpecifySutWithValue()
            => A3<Widget>
            .Sut(new Widget())
            .Arrange(setup => { })
            .Act(sut => sut)
            .Assert(result => result.Should().NotBeNull());

        [Fact]
        public void CanSpecifySutConstructor()
            => A3<Widget>
            .Constructor(type => type.GetConstructor(BindingFlags.Public | BindingFlags.Instance, null, new[] { typeof(string) }, null))
            .Arrange(setup => { })
            .Act(sut => sut.Name)
            .Assert((result, context) => result.Should().NotBeNullOrEmpty());
    }
}
