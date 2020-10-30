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
        public static ActStep<T> Arrange(Action<ArrangeBuilder<T>> arrange)
            => new ArrangeStep<T>(typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault()).Arrange(arrange);

        public static ArrangeStep<T> Constructor(Func<Type, ConstructorInfo?> selector)
            => new ArrangeStep<T>(selector(typeof(T)));
    }
}
