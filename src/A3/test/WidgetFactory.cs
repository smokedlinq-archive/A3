namespace A3.Tests
{
    public class WidgetFactory
    {
        public virtual Widget Create(string? name = null, Widget? parent = null)
            => parent is null
            ? new Widget(name)
            : new Widget(name, parent);
    }
}
