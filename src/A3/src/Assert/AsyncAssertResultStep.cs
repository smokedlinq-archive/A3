using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Assert
{
    public class AsyncAssertResultStep<TSut, TResult>
    {
        private readonly Task<TResult> task;
        private readonly AssertContext<TSut> context;

        internal AsyncAssertResultStep(Task<TResult> task, TSut sut, IEnumerable<Mock> mocks)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            context = new AssertContext<TSut>(sut, mocks);
        }

        public async Task Assert(Action<TResult> assert)
        {
            var result = await task.ConfigureAwait(false);
            assert(result);
        }

        public async Task Assert(Func<TResult, Task> assert)
        {
            var result = await task.ConfigureAwait(false);
            await assert(result).ConfigureAwait(false);
        }

        public async Task Assert(Action<TResult, AssertContext<TSut>> assert)
        {
            var result = await task.ConfigureAwait(false);
            assert(result, context);
        }

        public async Task Assert(Func<TResult, AssertContext<TSut>, Task> assert)
        {
            var result = await task.ConfigureAwait(false);
            await assert(result, context).ConfigureAwait(false);
        }
    }
}