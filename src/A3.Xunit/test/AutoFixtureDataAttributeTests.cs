using AutoFixture;
using FluentAssertions;
using Xunit;

namespace A3.Xunit.Tests
{
    public class AutoFixtureDataAttributeTests
    {
        [Theory]
        [AutoFixtureData]
        public void AutoFixtureDataAttributeUsesAutoFixtureCustomization(string value)
            => A3<string>
            .Arrange(setup => setup.Sut(_ => value))
            .Act(sut => sut)
            .Assert(result => result.Should().Be(nameof(StringFixtureCustomization)));

        public class StringFixtureCustomization : ICustomizeFixture<string>
        {
            public string Customize(IFixture fixture)
                => nameof(StringFixtureCustomization);
        }
    }
}
