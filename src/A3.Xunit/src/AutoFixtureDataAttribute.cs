﻿using AutoFixture;
using AutoFixture.Xunit2;

namespace Xunit
{
    public class AutoFixtureDataAttribute : AutoDataAttribute
    {
        public AutoFixtureDataAttribute(string? scope = null)
            : base(() => Customize(new Fixture(), scope))
        {
        }

        private static IFixture Customize(IFixture fixture, string? scope)
            => fixture.Customize(new AutoFixtureCustomization(scope));
    }
}
