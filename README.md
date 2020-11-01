# Arrange, Act, Assert

![Build](https://github.com/smokedlinq/A3/workflows/Build/badge.svg)
[![SonarCloud Status](https://sonarcloud.io/api/project_badges/measure?project=smokedlinq_A3&metric=alert_status)](https://sonarcloud.io/dashboard?id=smokedlinq_A3)
[![NuGet](https://img.shields.io/nuget/dt/A3.svg)](https://www.nuget.org/packages/A3)
[![NuGet](https://img.shields.io/nuget/vpre/A3.svg)](https://www.nuget.org/packages/A3)

This is a package that leverages [Moq]() and [AutoFixture]() to make unit testing with the arrange, act, assert pattern simple.

## Getting started

This is a simple example that shows the basic structure of A3 using the classic `Calculator` class.

```csharp
[Fact]
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => (1, 1))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

To start an A3 flow, use the [A3&lt;T&gt;](src/A3/src/A3.cs) class, the generic type parameter `T` is the system under test (SUT).

> Note: The below assert steps use [FluentAssertions](https://github.com/fluentassertions/fluentassertions).

### Arrange

Next perform the arrange step with the `Arrange` method. This provides a [builder](src/A3/src/Arrange/ArrangeBuilder.cs) to configure all the things needed to perform the act step, such as setting up mocks, defining fixtures, specifying the instance of the SUT to test, and providing parameters.

> Note: This step is usually the most work intensive so you will see more features here than on the act or assert steps.

The following examples were hand-crafted outside of IDE validation, so if they don't build I apologize. If you don't care for the examples and want to see two other projects that leverage this package you can check out [Extensions.System.Text.Json](https://github.com/smokedlinq/Extensions.System.Text.Json) or [AzureFunctions.Extensions](https://github.com/smokedlinq/AzureFunctions.Extensions)

#### Mock example

The `Mock<T>()` and `Mock<T>(Action<T>)` methods both return the mocked object. The latter method allows you to configure the mocked object.

```csharp
[Fact]
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<Widget>(setup => setup.Mock<Widget>(widget => widget.SetupGet(x => x.Value).Returns(1).Object))
    .Act((sut, widget) => sut.Add(widget.Value, widget.Value))
    .Assert(result => result.Should().Be(2));
```

#### Fixture example

Currently, the package leverages AutoFixture for all fixtures. The `IFixture` is provided on the [builder](src/A3/src/Arrange/ArrangeBuilder.cs) via the `Fixture` property.

> Note: The AutoMoq feature of AutoFixture is already added to the fixture so no need to add it.

```csharp
[Fact]
public void WhenAddingTwoValuesShouldBeGreaterThanOrEqualZero()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => (setup.Fixture.Create<int>(), setup.Fixture.Create<int>()))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().BeGreaterThanOrEqualTo(0));
```

##### Fixture customization

You can customize the fixture either during arrange or using the [ICustomizeFixture](src/A3/src/AutoFixture/ICustomizeFixture.cs) interface or register a specific type using the [ICustomizeFixture&lt;T&gt;](src/A3/src/AutoFixture/ICustomizeFixture.cs) interface.

> Note: These two examples are equivalent, you should use one or the other.

```csharp
public class CustomizeFixture : ICustomizeFixture
{
    public void Customize(IFixture fixture)
        => fixture.Register<IWidget>(() => WidgetFactory.Create(fixture.Create<int>()));
}

public class CustomizeWidgetFixture : ICustomizeFixture<IWidget>
{
    public IWidget Customize(IFixture fixture)
        => WidgetFactory.Create(fixture.Create<int>());   
}
```

##### Scoped fixture customization

If a fixture shouldn't be used globally, you can leverage scoped fixtures by overriding the `ShouldCustomize` method on either interface.

> Note: These two examples are equivalent, you should use one or the other.

```csharp
public static class FixtureScopes
{
    public const string? Default = null;
    public const string NonDefault = nameof(Default);
}

public class CustomizeFixtureWhenScoped : ICustomizeFixture
{
    public bool ShouldCustomize(string? scope)
        => scope == FixtureScopes.NonDefault;

    public void Customize(IFixture fixture)
        => fixture.Register<IWidget>(() => WidgetFactory.Create(fixture.Create<int>()));
}

public class CustomizeWidgetFixtureWhenScoped : ICustomizeFixture<IWidget>
{
    public bool ShouldCustomize(string? scope)
        => scope == FixtureScopes.NonDefault;

    public IWidget Customize(IFixture fixture)
        => WidgetFactory.Create(fixture.Create<int>());   
}
```

By default the scope is null for all tests. To override the scope for a particular test use the [A3Options.Scope()](src/A3/src/A3Options.cs) method as the options parameter on the `Arrange` method.

```csharp
[Fact]
public void WhenAddingTwoValuesShouldBeGreaterThanOrEqualZero()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => (1, 1), A3Options.Scope(FixtureScopes.Default))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

#### SUTs

By default the SUT is created using the first public constructor found via reflection. If a mock is registered that can be used with an argument it will, otherwise AutoFixture will be used to generate a value.

##### Specify the SUT constructor

If you want to specify the constructor, you can use the `Constructor` method on either the [A3 class](src/A3/src/A3.cs) before calling arrange or the [builder](src/A3/src/Arrange/ArrangeBuilder.cs) during setup.

> Note: The two examples are equivalent, you should use one or the other.

```csharp
[Fact]
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Constructor(type => type.GetConstructor(new[] { typeof(string) }))
    .Arrange(setup => (1, 1))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));

[Fact]
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => 
    {
        setup.Constructor(type => type.GetConstructor(new[] { typeof(string) }));
        return (1, 1);
    })
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

##### Specify the SUT during arrangement

For more advance control on the creation of the SUT, you can use the [builder](src/A3/src/Arrange/ArrangeBuilder.cs) `Sut` method during setup.

> Note: The examples are equivalent, you should use one two use based on your scenario.

```csharp
[Fact]
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => 
    {
        // The SUT is created before the act step runs but after arrange has completed
        //   this defers the instantiation until after arrange has completed
        setup.Sut(context => new Calculator(context.Fixture<string>()));
        return (1, 1);
    })
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));

[Fact]
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => 
    {
        // The SUT is created before the act step runs but after arrange has completed
        //   this defers the instantiation until after arrange has completed
        //   but doesn't use the context
        setup.Sut(() => new Calculator(context.Fixture<string>()));
        return (1, 1);
    })
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));

[Fact]
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => 
    {
        // The SUT is created at this point in the arrange method
        setup.Sut(new Calculator(context.Fixture<string>()));
        return (1, 1);
    })
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

#### Parameters

The SUT may need some input or parameters to execute, though this is optional. If your SUT does not need a parameter, you do not need to specify one.

You can specify the parameter either using the [builder](src/A3/src/Arrange/ArrangeBuilder.cs) `Parameter` method during setup or by returning a value from the arrange method.

> Note: If you return a value from arrange then the parameter type must be specified on the `Arrange` method but provides automatic type detection for the `Act` method.

```csharp
[Fact]
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange(setup => setup.Parameter((1, 1)))
    .Act((Calculator sut, (int x, int y)parameter) => sut.Add(x, y))
    .Assert(result => result.Should().Be(2));

[Fact]
public void OnePlusOneShouldBeTwo()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => ((1, 1)))
    .Act((sut, parameter) => sut.Add(parameter.x, parameter.y))
    .Assert(result => result.Should().Be(2));
```

### Act and assert

Next perform the act step with the `Act` method. This supports act steps that are async or synchronous and either return a value or not. You must at a minimum take the SUT as a parameter but can also specify the parameter provided during the arrange step. Finally, you need to assert either the result or a mock was called as expected.

> Note: There are plenty of examples in the arrange section, the ones not used there will be shown below.

```csharp
[Fact]
public void AddShouldBeCalled()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => setup.Sut(setup.Mock<Calculator>(calc => calc.Setup(x => x.Add(It.IsAny<int>(), It.isAny<int>())).Returns(2)).Object))
    .Act(sut => sut.Add(1, 1))
    .Assert((_, context) => context.Mock<Calculator>().Verify(x => x.Add(It.IsAny<int>(), It.isAny<int>()));

public void AddAsyncShouldBeCalled()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => setup.Sut(setup.Mock<Calculator>(calc => calc.Setup(x => x.AddAsync(It.IsAny<int>(), It.isAny<int>())).ReturnsAsync(2)).Object))
    .Act(sut => sut.AddAsync(1, 1))
    .Assert((_, context) => context.Mock<Calculator>().Verify(x => x.Add(It.IsAny<int>(), It.isAny<int>()));

public void AddAsyncShouldBeCalled()
  => A3<Calculator>
    .Arrange<(int x, int y)>(setup => setup.Sut(setup.Mock<Calculator>(calc => calc.Setup(x => x.AddAsync(It.IsAny<int>(), It.isAny<int>())).ReturnsAsync(2)).Object))
    .Act(async sut => 
    {
        var result = await sut.AddAsync(1, 1);
        return result;
    })
    .Assert((_, context) => context.Mock<Calculator>().Verify(x => x.Add(It.IsAny<int>(), It.isAny<int>()));
```

## A3.Xunit

This package provides support for AutoFixture.Xunit2 using the AutoData attribute but leveraging the same fixture customization used in A3. With this you can use the fixture customization features of A3 to generate theory data.

```csharp
[Theory]
[A3Data]
public void AddXToYShouldBeXPlusY(int x, int y)
  => A3<Calculator>
    .Arrange(setup => { })
    .Act(sut => sut.Add(x, y))
    .Assert(result => result.Should().Be(x+y));
```