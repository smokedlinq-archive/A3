using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Assert
{
    public class AsyncAssertResultStep<T>
    {
        private readonly Task<T> task;
        private readonly AssertContext context;

        internal AsyncAssertResultStep(Task<T> task, IEnumerable<Mock> mocks)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            context = new AssertContext(mocks);
        }

        public async Task Assert(Action<T> assert)
        {
            var result = await task.ConfigureAwait(false);
            assert(result);
        }

        public async Task Assert(Func<T, Task> assert)
        {
            var result = await task.ConfigureAwait(false);
            await assert(result).ConfigureAwait(false);
        }

        public async Task Assert(Action<T, AssertContext> assert)
        {
            var result = await task.ConfigureAwait(false);
            assert(result, context);
        }

        public async Task Assert(Func<T, AssertContext, Task> assert)
        {
            var result = await task.ConfigureAwait(false);
            await assert(result, context).ConfigureAwait(false);
        }
    }
}