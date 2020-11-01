using A3.Act;
using System;

namespace A3.Arrange
{
    public sealed class ArrangeStep<TSut, TParameter>
    {

        private readonly Func<ArrangeContext, TSut> factory;
        private readonly ArrangeOptions options;

        internal ArrangeStep(Func<ArrangeContext, TSut> factory, ArrangeOptions options)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public ActStep<TSut, TParameter> Arrange(Action<ArrangeBuilder<TSut, TParameter>> arrange)
        {
            var setup = new ArrangeBuilder<TSut, TParameter>(factory, options);

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

        public ActStep<TSut, TParameter> Arrange(Func<ArrangeBuilder<TSut, TParameter>, TParameter> arrange)
        {
            var setup = new ArrangeBuilder<TSut, TParameter>(factory, options);

            try
            {
                var parameter = arrange(setup);
                setup.Parameter(parameter);
                return setup.Build();
            }
            catch (Exception ex)
            {
                throw new ArrangeException($"Arrange step failed with exception {ex.GetType()}.", ex);
            }
        }
    }
}
