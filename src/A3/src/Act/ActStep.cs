using A3.Arrange;
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

        public AssertResultStep<T, TResult> Act<TParameter, TResult>(Func<T, TParameter, TResult> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            var result = ActInternal(act, out var sut);
            return new AssertResultStep<T, TResult>(sut, result, mocks);
        }

        public AssertResultStep<T, TResult> Act<TResult>(Func<T, TResult> act)
        {
            var result = ActInternal(act, out var sut);
            return new AssertResultStep<T, TResult>(sut, result, mocks);
        }

        public AssertStep<T> Act<TParameter>(Action<T, TParameter> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            return ActInternal<TParameter, AssertStep<T>>((sut, parameter) =>
            {
                act(sut, parameter);
                return new AssertStep<T>(sut, mocks);
            }, out var _);
        }

        public AssertStep<T> Act(Action<T> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            return ActInternal(sut =>
            {
                act(sut);
                return new AssertStep<T>(sut, mocks);
            }, out var _);
        }

        public AsyncAssertResultStep<T, TResult> Act<TParameter, TResult>(Func<T, TParameter, Task<TResult>> act)
        {
            var result = ActInternal(act, out var sut);
            return new AsyncAssertResultStep<T, TResult>(result, sut, mocks);
        }

        public AsyncAssertResultStep<T, TResult> Act<TResult>(Func<T, Task<TResult>> act)
        {
            var result = ActInternal(act, out var sut);
            return new AsyncAssertResultStep<T, TResult>(result, sut, mocks);
        }

        public AsyncAssertStep<T> Act<TParameter>(Func<T, TParameter, Task> act)
        {
            var result = ActInternal(act, out var sut);
            return new AsyncAssertStep<T>(result, sut, mocks);
        }

        public AsyncAssertStep<T> Act(Func<T, Task> act)
        {
            var result = ActInternal(act, out var sut);
            return new AsyncAssertStep<T>(result, sut, mocks);
        }

        private TResult ActInternal<TResult>(Func<T, TResult> act, out T sut)
            => ActInternal<object, TResult>((sut, _) => act(sut), out sut);

        private TResult ActInternal<TParameter, TResult>(Func<T, TParameter, TResult> act, out T sut)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));

            try
            {
                sut = CreateSut();
                return act(sut, (TParameter)parameter?.Value!);
            }
            catch (ArrangeException)
            {
                throw;
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
                catch (ArrangeException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    throw new ActException($"SUT creation failed with exception {ex.GetType()}.", ex);
                }
            }
        }
    }
}
