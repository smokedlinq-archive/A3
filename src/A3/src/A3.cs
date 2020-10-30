using A3.Act;
using A3.Arrange;
using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("A3.Tests")]

namespace A3
{
    public static class A3<T>
        where T : class
    {
        public static ActStep<T> Arrange(Action<ArrangeBuilder<T>> arrange, A3Options? options = null)
            => new ArrangeStep<T>(context => ArrangeSutFactory<T>.Create(context, typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault()), ArrangeOptions.From(options ?? new A3Options()))
            .Arrange(arrange);

        public static ArrangeStep<T> Constructor(Func<Type, ConstructorInfo?> selector, A3Options? options = null)
            => new ArrangeStep<T>(context => ArrangeSutFactory<T>.Create(context, selector(typeof(T))), ArrangeOptions.From(options ?? new A3Options()));

        public static ArrangeStep<T> Sut(Func<ArrangeContext, T> factory, A3Options? options = null)
            => new ArrangeStep<T>(factory, ArrangeOptions.From(options ?? new A3Options()));
    }
}
