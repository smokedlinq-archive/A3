namespace A3.Tests
{
    public class WidgetFactory
    {
        public virtual Widget Create(string name = null, Widget parent = null)
            => new Widget(name, parent);
    }
}
