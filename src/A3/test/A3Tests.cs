using A3.Tests.Fixtures;
using AutoFixture;
using FluentAssertions;
using System.Reflection;
using Xunit;

namespace A3.Tests
{
    [A3Scope(nameof(A3TestsFixtureCustomization))]
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

        [Fact]
        [A3Scope(nameof(A3TestMethodFixtureCustomization))]
        public void UsesScopeFromA3ScopeAttributeOnMethodOverClass()
            => A3<string>
            .Arrange(setup => setup.Sut(setup.Fixture.Create<string>()))
            .Act(sut => sut)
            .Assert(result => result.Should().Be(nameof(A3TestMethodFixtureCustomization)));

        [Fact]
        public void UsesScopeFromA3ScopeAttributeOnClass()
            => A3<string>
            .Arrange(setup => setup.Sut(setup.Fixture.Create<string>()))
            .Act(sut => sut)
            .Assert(result => result.Should().Be(nameof(A3TestsFixtureCustomization)));

        public class A3TestMethodFixtureCustomization : ICustomizeFixture<string>
        {
            public bool ShouldCustomize(string? scope)
                => scope == nameof(A3TestMethodFixtureCustomization);

            public string Customize(IFixture fixture)
                => nameof(A3TestMethodFixtureCustomization);
        }

        public class A3TestsFixtureCustomization : ICustomizeFixture<string>
        {
            public bool ShouldCustomize(string? scope)
                => scope == nameof(A3TestsFixtureCustomization);

            public string Customize(IFixture fixture)
                => nameof(A3TestsFixtureCustomization);
        }
    }
}
