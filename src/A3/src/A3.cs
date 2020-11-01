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
    {
        public static ActStep<T, object> Arrange(Action<ArrangeBuilder<T, object>> arrange, A3Options? options = null)
            => Arrange<object>(arrange, options);

        public static ActStep<T, TParameter> Arrange<TParameter>(Func<ArrangeBuilder<T, TParameter>, TParameter> arrange, A3Options? options = null)
            => new ArrangeStep<T, TParameter>(CreateSutFromConstructor(), (options ?? new A3Options()).Arrange)
            .Arrange(arrange);

        public static ActStep<T, TParameter> Arrange<TParameter>(Action<ArrangeBuilder<T, TParameter>> arrange, A3Options? options = null)
            => new ArrangeStep<T, TParameter>(CreateSutFromConstructor(), (options ?? new A3Options()).Arrange)
            .Arrange(arrange);

        public static ArrangeStep<T, object> Constructor(Func<Type, ConstructorInfo?> selector, A3Options? options = null)
            => Constructor<object>(selector, options);

        public static ArrangeStep<T, TParameter> Constructor<TParameter>(Func<Type, ConstructorInfo?> selector, A3Options? options = null)
            => new ArrangeStep<T, TParameter>(context => ArrangeSutFactory<T>.Create(context, selector(typeof(T))), (options ?? new A3Options()).Arrange);

        public static ArrangeStep<T, object> Sut(Func<ArrangeContext, T> factory, A3Options? options = null)
            => Sut<object>(factory, options);

        public static ArrangeStep<T, TParameter> Sut<TParameter>(Func<ArrangeContext, T> factory, A3Options? options = null)
            => new ArrangeStep<T, TParameter>(factory, (options ?? new A3Options()).Arrange);

        public static ArrangeStep<T, object> Sut(T sut, A3Options? options = null)
            => Sut<object>(sut, options);

        public static ArrangeStep<T, TParameter> Sut<TParameter>(T sut, A3Options? options = null)
            => new ArrangeStep<T, TParameter>(_ => sut, (options ?? new A3Options()).Arrange);

        private static Func<ArrangeContext, T> CreateSutFromConstructor()
            => context => ArrangeSutFactory<T>.Create(context, typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault());
    }
}
