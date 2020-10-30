using A3.Act;
using System;
using System.Reflection;

namespace A3.Arrange
{
    public sealed class ArrangeStep<T>
        where T : class
    {

        private readonly ConstructorInfo? constructor;

        internal ArrangeStep(ConstructorInfo? constructor)
            => this.constructor = constructor;

        public ActStep<T> Arrange(Action<ArrangeBuilder<T>> arrange)
        {
            var setup = new ArrangeBuilder<T>(constructor);

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
