using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Assert
{
    public class AssertResultStep<TSut, TResult>
    {
        private readonly TResult result;
        private readonly AssertContext<TSut> context;

        internal AssertResultStep(TSut sut, TResult result, IEnumerable<Mock> mocks)
        {
            this.result = result;
            this.context = new AssertContext<TSut>(sut, mocks);
        }

        public void Assert(Action<TResult> assert)
            => assert(result);

        public Task Assert(Func<TResult, Task> assert)
            => assert(result);

        public void Assert(Action<TResult, AssertContext<TSut>> assert)
            => assert(result, context);

        public Task Assert(Func<TResult, AssertContext<TSut>, Task> assert)
            => assert(result, context);
    }
}