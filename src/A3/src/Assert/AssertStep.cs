using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Assert
{
    public class AssertStep
    {
        private readonly AssertContext context;

        internal AssertStep(IEnumerable<Mock> mocks)
            => context = new AssertContext(mocks);

        public void Assert(Action<AssertContext> assert)
            => assert(context);

        public Task Assert(Func<AssertContext, Task> assert)
            => assert(context);

    }
}