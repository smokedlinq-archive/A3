using A3.Fixtures;
using AutoFixture;
using AutoFixture.Xunit2;

namespace A3.Xunit
{
    public class AutoFixtureDataAttribute : AutoDataAttribute
    {
        public AutoFixtureDataAttribute()
            : base(() => Customize(new Fixture()))
        {
        }

        private static IFixture Customize(IFixture fixture)
            => fixture.Customize(new AutoFixtureCustomization());
    }
}
