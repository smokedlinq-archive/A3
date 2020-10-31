using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Assert
{
    public class AssertResultStep<T>
    {
        private readonly T result;
        private readonly AssertContext context;

        internal AssertResultStep(T result, IEnumerable<Mock> mocks)
        {
            this.result = result;
            this.context = new AssertContext(mocks);
        }

        public void Assert(Action<T> assert)
            => assert(result);

        public Task Assert(Func<T, Task> assert)
            => assert(result);

        public void Assert(Action<T, AssertContext> assert)
            => assert(result, context);

        public Task Assert(Func<T, AssertContext, Task> assert)
            => assert(result, context);
    }
}