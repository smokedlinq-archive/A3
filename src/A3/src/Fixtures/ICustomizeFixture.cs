namespace AutoFixture
{
    public interface ICustomizeFixture<out T>
    {
        T Customize(IFixture fixture);
    }
}
