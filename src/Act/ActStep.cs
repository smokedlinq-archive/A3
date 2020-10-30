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
        private readonly SutParameter? parameter;
        private readonly IEnumerable<Mock> mocks;

        internal ActStep(Func<T> factory, SutParameter? parameter, IEnumerable<Mock> mocks)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.parameter = parameter;
            this.mocks = mocks ?? throw new ArgumentNullException(nameof(mocks));
        }

        public AssertResultStep<TResult> Act<TParameter, TResult>(Func<T, TParameter, TResult> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            return new AssertResultStep<TResult>(ActInternal(act), mocks);
        }

        public AssertResultStep<TResult> Act<TResult>(Func<T, TResult> act)
            => new AssertResultStep<TResult>(ActInternal(act), mocks);

        public AssertStep Act<TParameter>(Action<T, TParameter> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            return ActInternal<TParameter, AssertStep>((sut, parameter) =>
            {
                act(sut, parameter);
                return new AssertStep(mocks);
            });
        }

        public AssertStep Act(Action<T> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            return ActInternal(sut =>
            {
                act(sut);
                return new AssertStep(mocks);
            });
        }

        public AsyncAssertResultStep<TResult> Act<TParameter, TResult>(Func<T, TParameter, Task<TResult>> act)
            => new AsyncAssertResultStep<TResult>(ActInternal(act), mocks);

        public AsyncAssertResultStep<TResult> Act<TResult>(Func<T, Task<TResult>> act)
            => new AsyncAssertResultStep<TResult>(ActInternal(act), mocks);

        public AsyncAssertStep Act<TParameter>(Func<T, TParameter, Task> act)
            => new AsyncAssertStep(ActInternal(act), mocks);

        public AsyncAssertStep Act(Func<T, Task> act)
            => new AsyncAssertStep(ActInternal(act), mocks);

        private TResult ActInternal<TResult>(Func<T, TResult> act)
            => ActInternal<object, TResult>((sut, _) => act(sut));

        private TResult ActInternal<TParameter, TResult>(Func<T, TParameter, TResult> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));

            try
            {
                var sut = CreateSut();
                return act(sut, (TParameter)parameter?.Value!);
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
