# Assert

The assert step is where you verify the action against the SUT. This can include asserting the result of the action or validation that a particular mock was or was not invoked.

[For more infomation](act.md) on the act step click on the link.

## Usage

The first parameter on the Assert delegate is the result of the act step.

```csharp
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => (1, 1))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

### Verifying mocks

The second parameter provides access to the [AssertContext](../src/A3/src/Assert/AssertContext.cs) which allows you to access the mock created in the arrange method or used to instantiate the SUT.

```csharp
public void AddShouldBeCalled()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => setup.Sut(setup.Mock<Calculator>(calc => calc.Setup(x => x.Add(It.IsAny<int>(), It.isAny<int>())).Returns(2)).Object))
    .Act(sut => sut.Add(1, 1))
    .Assert((_, context) => context.Mock<Calculator>().Verify(x => x.Add(It.IsAny<int>(), It.isAny<int>()));
```
