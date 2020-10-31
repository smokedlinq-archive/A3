using AutoFixture;
using FluentAssertions;
using Xunit;

namespace A3.Tests.Fixtures
{
    public class AutoFixtureCustomizationTests
    {
        [Fact]
        public void CustomizeFixtureOfTDiscoversAndCallsAndInjectsIntoFixtureContext()
            => A3<IFixture>
            .Arrange(setup => setup.Sut(context => (Fixture)context.Fixture))
            .Act(sut => sut.Create<WidgetFixture>())
            .Assert(result => result.Name.Should().Be(nameof(WidgetFixture)));

        [Fact]
        public void CustomizeFixtureOfTSupportsMultipleImplementationsOnTheSameType()
            => A3<IFixture>
            .Arrange(setup => setup.Sut(context => (Fixture)context.Fixture))
            .Act(sut => sut.Create<string>())
            .Assert(result => result.Should().Be(nameof(WidgetFixtureCustomization.Customize)));

        [Fact]
        public void CustomizeFixtureOfTSupportsScope()
            => A3<IFixture>
            .Arrange(setup => setup.Sut(context => new Fixture().Customize(new AutoFixtureCustomization(nameof(ScopedWidgetFixtureCustomization)))))
            .Act(sut => sut.Create<WidgetFixture>())
            .Assert(result => result.Name.Should().Be(nameof(ScopedWidgetFixtureCustomization)));

        [Fact]
        public void CustomizeFixtureDiscoversAndCallsAndInjectsIntoFixtureContext()
            => A3<IFixture>
            .Arrange(setup => setup.Sut(context => (Fixture)context.Fixture))
            .Act(sut => sut.Create<bool?>())
            .Assert(result => result.Should().BeTrue());

        [Fact]
        public void CustomizeFixtureSupportsScope()
            => A3<IFixture>
            .Arrange(setup => setup.Sut(context => new Fixture().Customize(new AutoFixtureCustomization(nameof(ScopedFixtureCustomization)))))
            .Act(sut => sut.Create<bool?>())
            .Assert(result => result.Should().BeFalse());

        public class WidgetFixture
        {
            public string? Name { get; set; }
        }

        public class FixtureCustomizations : ICustomizeFixture
        {
            public void Customize(IFixture fixture)
                => fixture.Register(() => (bool?)true);
        }

        public class WidgetFixtureCustomization : ICustomizeFixture<WidgetFixture>, ICustomizeFixture<string>
        {
            public WidgetFixture Customize(IFixture fixture)
                => new WidgetFixture { Name = nameof(WidgetFixture) };

            string ICustomizeFixture<string>.Customize(IFixture fixture)
                => nameof(Customize);
        }

        public class ScopedWidgetFixtureCustomization : ICustomizeFixture<WidgetFixture>
        {
            public bool ShouldCustomize(string? scope)
                => scope == nameof(ScopedWidgetFixtureCustomization);

            public WidgetFixture Customize(IFixture fixture)
                => new WidgetFixture { Name = nameof(ScopedWidgetFixtureCustomization) };
        }

        public class ScopedFixtureCustomization : ICustomizeFixture
        {
            public bool ShouldCustomize(string? scope)
                => scope == nameof(ScopedFixtureCustomization);

            public void Customize(IFixture fixture)
                => fixture.Register(() => (bool?)false);
        }
    }
}
