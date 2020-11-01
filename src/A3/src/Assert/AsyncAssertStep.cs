using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Assert
{
    public class AsyncAssertStep<T>
    {
        private readonly Task task;
        private readonly AssertContext<T> context;

        internal AsyncAssertStep(Task task, T sut, IEnumerable<Mock> mocks)
        {
            this.task = task ?? throw new ArgumentNullException(nameof(task));
            context = new AssertContext<T>(sut, mocks);
        }

        public async Task Assert(Action<AssertContext<T>> assert)
        {
            await task.ConfigureAwait(false);
            assert(context);
        }
    }
}