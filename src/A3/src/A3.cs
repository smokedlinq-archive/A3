using A3.Act;
using A3.Arrange;
using System;
using System.Diagnostics;
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
            => new ArrangeStep<T, TParameter>(CreateSutFromConstructor(), A3Options(options).Arrange)
            .Arrange(arrange);

        public static ActStep<T, TParameter> Arrange<TParameter>(Action<ArrangeBuilder<T, TParameter>> arrange, A3Options? options = null)
            => new ArrangeStep<T, TParameter>(CreateSutFromConstructor(), A3Options(options).Arrange)
            .Arrange(arrange);

        public static ArrangeStep<T, object> Constructor(Func<Type, ConstructorInfo?> selector, A3Options? options = null)
            => Constructor<object>(selector, options);

        public static ArrangeStep<T, TParameter> Constructor<TParameter>(Func<Type, ConstructorInfo?> selector, A3Options? options = null)
            => new ArrangeStep<T, TParameter>(context => ArrangeSutFactory<T>.Create(context, selector(typeof(T))), A3Options(options).Arrange);

        public static ArrangeStep<T, object> Sut(Func<ArrangeContext, T> factory, A3Options? options = null)
            => Sut<object>(factory, options);

        public static ArrangeStep<T, TParameter> Sut<TParameter>(Func<ArrangeContext, T> factory, A3Options? options = null)
            => new ArrangeStep<T, TParameter>(factory, A3Options(options).Arrange);

        public static ArrangeStep<T, object> Sut(T sut, A3Options? options = null)
            => Sut<object>(sut, options);

        public static ArrangeStep<T, TParameter> Sut<TParameter>(T sut, A3Options? options = null)
            => new ArrangeStep<T, TParameter>(_ => sut, A3Options(options).Arrange);

        private static Func<ArrangeContext, T> CreateSutFromConstructor()
            => context => ArrangeSutFactory<T>.Create(context, typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault());

        private static A3Options A3Options(A3Options? options)
        {
            options ??= new A3Options();

            if (options.Arrange.Scope is null)
            {
                var stack = new StackTrace(0);
                var method = stack.GetFrames()
                    .Select(x => x.GetMethod())
                    .FirstOrDefault(x => x.DeclaringType.Assembly != typeof(A3<T>).Assembly);

                if (!(method is null))
                {
                    // Find the scope on the method
                    var scopeAttribute = method.GetCustomAttribute<A3ScopeAttribute>(true);

                    // Find the scope on the class
                    if (scopeAttribute is null)
                    {
                        scopeAttribute = method.DeclaringType.GetCustomAttribute<A3ScopeAttribute>(true);
                    }

                    // Find the scope on the assembly
                    if (scopeAttribute is null)
                    {
                        scopeAttribute = (A3ScopeAttribute)method.DeclaringType.Assembly.GetCustomAttribute(typeof(A3ScopeAttribute));
                    }

                    if (!(scopeAttribute is null))
                    {
                        options.Arrange.Scope = scopeAttribute.Name;
                    }
                }
            }

            return options;
        }
    }
}
