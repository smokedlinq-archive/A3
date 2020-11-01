using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Assert
{
    public class AssertStep<T>
    {
        private readonly AssertContext<T> context;

        internal AssertStep(T sut, IEnumerable<Mock> mocks)
            => context = new AssertContext<T>(sut, mocks);

        public void Assert(Action<AssertContext<T>> assert)
            => assert(context);

        public Task Assert(Func<AssertContext<T>, Task> assert)
            => assert(context);

    }
}