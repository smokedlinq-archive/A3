using System;
using System.Threading.Tasks;

namespace A3.Tests
{
    public class WidgetService
    {
        private readonly Widget widget;

        public WidgetService(WidgetFactory factory, Widget widget = null)
            => this.widget = (factory ?? throw new ArgumentNullException(nameof(factory))).Create(null, widget);

        public string Name => widget.Name;

        public Task<bool> ExecuteAsync()
            => widget.ExecuteAsync();

        public bool Execute()
            => widget.Execute();
    }
}
