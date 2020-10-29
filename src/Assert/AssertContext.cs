using Moq;
using System.Collections.Generic;
using System.Linq;

namespace A3.Assert
{
    public class AssertContext
    {
        private readonly IEnumerable<Mock> mocks;

        internal AssertContext(IEnumerable<Mock> mocks)
            => this.mocks = mocks ?? throw new System.ArgumentNullException(nameof(mocks));

        public Mock<T> Mock<T>()
            where T : class
        {
            var mock = (Mock<T>)mocks.FirstOrDefault(x => x is Mock<T>)!;

            if (mock is null)
            {
                throw new AssertException($"Mock {typeof(Mock<T>)} was not added during the arrange step.");
            }

            return mock;
        }
    }
}