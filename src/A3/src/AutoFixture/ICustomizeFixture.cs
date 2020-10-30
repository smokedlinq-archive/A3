namespace AutoFixture
{
    public interface ICustomizeFixture<out T>
    {
        public bool ShouldCustomize(string? scope)
            => scope == null;

        T Customize(IFixture fixture);
    }
}
