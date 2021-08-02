using AutoFixture;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace A3.Arrange
{
    public sealed class ArrangeContext
    {
        private readonly IEnumerable<Mock> mocks;

        internal ArrangeContext(IFixture fixture, IEnumerable<Mock> mocks)
        {
            Fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            this.mocks = mocks ?? throw new ArgumentNullException(nameof(mocks));
        }

        public IFixture Fixture { get; }

        public bool TryGetMock(Type type, out Mock mock)
        {
            var mockType = typeof(Mock<>).MakeGenericType(type);
            mock = mocks.FirstOrDefault(x => x.GetType() == mockType)!;
            return !(mock is null);
        }

        public bool TryGetMock<T>(out Mock<T> mock)
            where T : class
        {
            mock = (Mock<T>)mocks.FirstOrDefault(x => x is Mock<T>)!;
            return !(mock is null);
        }
    }
}