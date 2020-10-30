﻿using AutoFixture;
using FluentAssertions;
using Xunit;

namespace A3.Tests.Fixtures
{
    public class AutoFixtureCustomizationTests
    {
        [Fact]
        public void DiscoversAndCallsAndInjectsIntoFixtureOfA3Context()
            => A3<IFixture>
            .Arrange(setup => setup.Sut(context => (Fixture)context.Fixture))
            .Act(sut => sut.Create<WidgetFixture>())
            .Assert((context, result) => result.Name.Should().Be(nameof(WidgetFixture)));

        [Fact]
        public void SupportsMultipleImplementationsOnTheSameType()
            => A3<IFixture>
            .Arrange(setup => setup.Sut(context => (Fixture)context.Fixture))
            .Act(sut => sut.Create<string>())
            .Assert((context, result) => result.Should().Be(nameof(WidgetFixtureCustomization.Customize)));

        public class WidgetFixture
        {
            public string Name { get; set; }
        }

        public class WidgetFixtureCustomization : ICustomizeFixture<WidgetFixture>, ICustomizeFixture<string>
        {
            public WidgetFixture Customize(IFixture fixture)
                => new WidgetFixture
                {
                    Name = nameof(WidgetFixture)
                };

            string ICustomizeFixture<string>.Customize(IFixture fixture)
                => nameof(Customize);
        }
    }
}
