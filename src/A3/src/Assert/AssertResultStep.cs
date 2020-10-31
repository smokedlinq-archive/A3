﻿using Moq;
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

        public void Assert(Action<AssertContext, T> assert)
            => assert(context, result);

        public Task Assert(Func<AssertContext, T, Task> assert)
            => assert(context, result);
    }
}