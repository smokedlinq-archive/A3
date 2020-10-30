namespace A3.Arrange
{
    internal class ArrangeOptions
    {
        public string? AutoFixtureCustomizationScope { get; set; }

        public static ArrangeOptions From(A3Options options)
            => new ArrangeOptions { AutoFixtureCustomizationScope = options.AutoFixtureCustomizationScope };
    }
}