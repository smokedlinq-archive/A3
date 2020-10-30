using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Assert
{
    public class AsyncAssertStep
    {
        private readonly Task task;
        private readonly AssertContext context;

        internal AsyncAssertStep(Task task, IEnumerable<Mock> mocks)
        {
            this.task = task ?? throw new System.ArgumentNullException(nameof(task));
            context = new AssertContext(mocks);
        }

        public async Task Assert(Action<AssertContext> assert)
        {
            await task.ConfigureAwait(false);
            assert(context);
        }
    }
}