using A3.Arrange;
using A3.Assert;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace A3.Act
{
    public sealed class ActStep<TSut, TParameter>
    {
        private readonly Func<TSut> factory;
        private readonly TParameter? parameter;
        private readonly IEnumerable<Mock> mocks;

        internal ActStep(Func<TSut> factory, TParameter? parameter, IEnumerable<Mock> mocks)
        {
            this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
            this.parameter = parameter;
            this.mocks = mocks ?? throw new ArgumentNullException(nameof(mocks));
        }

        public AssertResultStep<TSut, TResult> Act<TResult>(Func<TSut, TParameter?, TResult> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            var result = ActInternal(act, out var sut);
            return new AssertResultStep<TSut, TResult>(sut, result, mocks);
        }

        public AssertResultStep<TSut, TResult> Act<TResult, TSpecificParameter>(Func<TSut, TSpecificParameter?, TResult> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            var result = ActInternal((sut, parameter) => act(sut, (TSpecificParameter?)(object?)parameter), out var sut);
            return new AssertResultStep<TSut, TResult>(sut, result, mocks);
        }

        public AssertResultStep<TSut, TResult> Act<TResult>(Func<TSut, TResult> act)
        {
            var result = ActInternal(act, out var sut);
            return new AssertResultStep<TSut, TResult>(sut, result, mocks);
        }

        public AssertStep<TSut> Act(Action<TSut, TParameter?> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            return ActInternal((sut, parameter) =>
            {
                act(sut, parameter);
                return new AssertStep<TSut>(sut, mocks);
            }, out var _);
        }

        public AssertStep<TSut> Act<TSpecificParameter>(Action<TSut, TSpecificParameter?> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            return ActInternal((sut, parameter) =>
            {
                act(sut, (TSpecificParameter?)(object?)parameter!);
                return new AssertStep<TSut>(sut, mocks);
            }, out var _);
        }

        public AssertStep<TSut> Act(Action<TSut> act)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));
            return ActInternal(sut =>
            {
                act(sut);
                return new AssertStep<TSut>(sut, mocks);
            }, out var _);
        }

        public AsyncAssertResultStep<TSut, TResult> Act<TResult>(Func<TSut, TParameter?, Task<TResult>> act)
        {
            var result = ActInternal(act, out var sut);
            return new AsyncAssertResultStep<TSut, TResult>(result, sut, mocks);
        }

        public AsyncAssertResultStep<TSut, TResult> Act<TSpecificParameter, TResult>(Func<TSut, TSpecificParameter?, Task<TResult>> act)
        {
            var result = ActInternal((sut, parameter) => act(sut, (TSpecificParameter?)(object?)parameter), out var sut);
            return new AsyncAssertResultStep<TSut, TResult>(result, sut, mocks);
        }

        public AsyncAssertResultStep<TSut, TResult> Act<TResult>(Func<TSut, Task<TResult>> act)
        {
            var result = ActInternal(act, out var sut);
            return new AsyncAssertResultStep<TSut, TResult>(result, sut, mocks);
        }

        public AsyncAssertStep<TSut> Act(Func<TSut, TParameter?, Task> act)
        {
            var result = ActInternal(act, out var sut);
            return new AsyncAssertStep<TSut>(result, sut, mocks);
        }

        public AsyncAssertStep<TSut> Act<TSpecificParameter>(Func<TSut, TSpecificParameter?, Task> act)
        {
            var result = ActInternal((sut, parameter) => act(sut, (TSpecificParameter?)(object?)parameter), out var sut);
            return new AsyncAssertStep<TSut>(result, sut, mocks);
        }

        public AsyncAssertStep<TSut> Act(Func<TSut, Task> act)
        {
            var result = ActInternal(act, out var sut);
            return new AsyncAssertStep<TSut>(result, sut, mocks);
        }

        private TResult ActInternal<TResult>(Func<TSut, TResult> act, out TSut sut)
            => ActInternal((sut, _) => act(sut), out sut);

        private TResult ActInternal<TResult>(Func<TSut, TParameter?, TResult> act, out TSut sut)
        {
            _ = act ?? throw new ArgumentNullException(nameof(act));

            try
            {
                sut = CreateSut();
                return act(sut, parameter);
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

            TSut CreateSut()
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