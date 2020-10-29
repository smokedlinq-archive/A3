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

        public async Task Assert(Action<AssertContext, T> assert)
        {
            var result = await task.ConfigureAwait(false);
            assert(context, result);
        }
    }
}