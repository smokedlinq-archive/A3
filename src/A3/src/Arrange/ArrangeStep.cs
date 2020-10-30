using A3.Act;
using System;

namespace A3.Arrange
{
    public sealed class ArrangeStep<T>
        where T : class
    {

        private readonly Func<ArrangeContext, T> factory;
        private readonly ArrangeOptions options;

        internal ArrangeStep(Func<ArrangeContext, T> factory, ArrangeOptions options)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ActStep<T> Arrange(Action<ArrangeBuilder<T>> arrange)
        {
            var setup = new ArrangeBuilder<T>(factory, options);

            try
            {
                arrange(setup);
                return setup.Build();
            }
            catch (Exception ex)
            {
                throw new ArrangeException($"Arrange step failed with exception {ex.GetType()}.", ex);
            }
        }
    }
}
