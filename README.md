# Arrange, Act, Assert

![Build](https://github.com/smokedlinq/A3/workflows/Build/badge.svg)
[![SonarCloud Status](https://sonarcloud.io/api/project_badges/measure?project=smokedlinq_A3&metric=alert_status)](https://sonarcloud.io/dashboard?id=smokedlinq_A3)

## A3

[![NuGet](https://img.shields.io/nuget/dt/A3.svg)](https://www.nuget.org/packages/A3)
[![NuGet](https://img.shields.io/nuget/vpre/A3.svg)](https://www.nuget.org/packages/A3)

A3 leverages [Moq](https://github.com/moq/moq4), [AutoFixture](https://github.com/AutoFixture/AutoFixture), and [FluentAssertions](https://github.com/fluentassertions/fluentassertions/) to make .NET unit testing with the arrange, act, assert pattern with easier and fun by reducing a lot of the overhead with setting up unit tests.

## A3.Xunit

[![NuGet](https://img.shields.io/nuget/dt/A3.Xunit.svg)](https://www.nuget.org/packages/A3.Xunit)
[![NuGet](https://img.shields.io/nuget/vpre/A3.Xunit.svg)](https://www.nuget.org/packages/A3.Xunit)

A3.Xunit leverages AutoFixture.Xunit2 to provide theory data via the A3 fixture customizations.

## Getting started

These are a few simple example that shows the basic structure of A3 using the classic `Calculator` class. The generic type parameter for `A3<T>` is the system under test (SUT).

```csharp
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => (1, 1))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));

public void WhenCalculatorIsMockedThenVerifyAddIsCalled()
  => A3<Calculator>
    .Arrange(setup => setup.Sut(
        setup.Mock<Calculator>(calc => calc.Setup(x => x.Add(It.IsAny<int>(), It.IsAny<int>()).Returns(setup.Fixture.Create<int>())))
    ))
    .Act(sut => sut.Add(1, 1))
    .Assert((_, context) => context.Mock<Calculator>().Verify(x => x.Add(It.IsAny<int>(), It.IsAny<int>()));

public Task WhenAddMultipleValuesThenShouldCompleteInLessThanOneSecondAsync()
  => A3<Calculator>
    .Arrange<IEnumerable<int[]>>(setup => setup.Fixture.CreateMany<int[]>())
    .Act((sut, parameter) => sut.Awaiting(x => Task.WhenAll(parameter.Select(x => sut.AddAsync(x)))))
    .Assert(result => result.Should().CompleteWithinAsync(TimeSpan.FromSeconds(1)));
```

For more information can be found on [Arrange](docs/arrange.md), [Act](docs/act.md), and [Assert](docs/assert.md) by clicking on the link.
