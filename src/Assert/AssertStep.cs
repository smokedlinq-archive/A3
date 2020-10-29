using Moq;
using System;
using System.Collections.Generic;

namespace A3.Assert
{
    public class AssertStep
    {
        private readonly AssertContext context;

        internal AssertStep(IEnumerable<Mock> mocks)
            => context = new AssertContext(mocks);

        public void Assert(Action<AssertContext> assert)
            => assert(context);

    }
}