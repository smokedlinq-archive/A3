using A3.Assert;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Act
{
    public sealed class ActStep<T>
        where T : class
    {
        private readonly Func<T> factory;
        private readonly IEnumerable<Mock> mocks;

        internal ActStep(Func<T> factory, IEnumerable<Mock> mocks)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.mocks = mocks ?? throw new ArgumentNullException(nameof(mocks));
        }

        public AssertResultStep<TResult> Act<TResult>(Func<T, TResult> act)
            => new AssertResultStep<TResult>(ActInternal(act), mocks);

        public AssertStep Act(Action<T> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            return ActInternal(sut =>
            {
                act(sut);
                return new AssertStep(mocks);
            });
        }

        public AsyncAssertResultStep<TResult> Act<TResult>(Func<T, Task<TResult>> act)
            => new AsyncAssertResultStep<TResult>(ActInternal(act), mocks);

        public AsyncAssertStep Act(Func<T, Task> act)
            => new AsyncAssertStep(ActInternal(act), mocks);

        private TResult ActInternal<TResult>(Func<T, TResult> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));

            try
            {
                var sut = CreateSut();
                return act(sut);
            }
            catch (ActException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ActException($"Act step failed with exception {ex.GetType()}.", ex);
            }

            T CreateSut()
            {
                try
                {
                    return factory();
                }
                catch (Exception ex)
                {
                    throw new ActException($"SUT creation failed with exception {ex.GetType()}.", ex);
                }
            }
        }
    }
}
