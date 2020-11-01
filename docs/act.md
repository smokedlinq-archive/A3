# Act

The act step is where you execute the action against the SUT and optionally return a result for use in the assert step.

[For more infomation](arrange.md) on preparing the SUT for the act step click on the link.

The final step is to [assert](assert.md) the result to verify the test completed as expected.

## SUT

The SUT is instantied, in most cases, after the [arrange](arrange.md) step and before the act step executes. During the [arrange](arrange.md) you have the option to define how the SUT is instantiated.

If an explicit SUT isn't provided then mocks and fixtures are used to generate values for the specified constructors parameters.

## Usage

The first parameter on the Act delegate is the SUT specified by the `A3<T>` definition. Use this to verify the SUT is behaving as expected.

```csharp
public void AddShouldNotThrow()
  => A3<Calculator>
    .Arrange(_ ={ })
    .Act(sut => sut.Invoking(x => x.Add(1, 1)))
    .Assert(result => result.Should().NotThrow());
```

> Note: The `Act` delegate is not required to return a result.

### Parameters

During the [arrange](arrange.md) step you can specify the parameters to use during the act step.

```csharp
public void AddOnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => (1, 1))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

If an unconstrained parameter type was used then you need to explicitly define the parameter types.

```csharp
public void AddOnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange(setup => setup.Parameter((1, 1)))
    .Act((Calculator sut, (int x, int y) parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

### Async methods

Both the act and [assert](assert.md) steps support async methods.

```csharp
public Task AddOnePlusOneShouldBeTwoAsync()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => (1, 1))
    .Act(async (sut, parameter) => await sut.AddAsync(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

Beyond testing async methods, this can be useful when using the awaiting feature of [FluentAssertions](https://github.com/fluentassertions/fluentassertions/).

```csharp
public Task AddOnePlusOneShouldCompleteWithinOneSecondAsync()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => (1, 1))
    .Act((sut, parameter) => sut.Awaiting(x => x.AddAsync(parameter.x, parameter.y)))
    .Assert(result => result.Should().CompleteWithinAsync(TimeSpan.FromSeconds(1)));
```
