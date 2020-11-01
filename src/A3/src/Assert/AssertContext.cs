using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace A3.Assert
{
    public class AssertContext<T>
    {
        private readonly IEnumerable<Mock> mocks;

        internal AssertContext(T sut, IEnumerable<Mock> mocks)
        {
            Sut = sut ?? throw new ArgumentNullException(nameof(sut));
            this.mocks = mocks ?? throw new System.ArgumentNullException(nameof(mocks));
        }

        public T Sut { get; }

        public Mock<TMock> Mock<TMock>()
            where TMock : class
        {
            var mock = (Mock<TMock>)mocks.FirstOrDefault(x => x is Mock<TMock>)!;

            if (mock is null)
            {
                throw new AssertException($"Mock {typeof(Mock<TMock>)} was not added during the arrange step.");
            }

            return mock;
        }
    }
}