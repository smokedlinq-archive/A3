namespace AutoFixture
{
    public interface ICustomizeFixture
    {
        public bool ShouldCustomize(string? scope)
            => scope == null;

        void Customize(IFixture fixture);
    }

    public interface ICustomizeFixture<out T>
    {
        public bool ShouldCustomize(string? scope)
            => scope == null;

        T Customize(IFixture fixture);
    }
}
